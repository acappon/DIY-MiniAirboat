"""Small linear-elastic 3D frame solver (Euler-Bernoulli beams, 6 DOF per node), inch-pound units.

Used for the drive tower, prop cage and driver side frame in structure.py. Members are straight
two-node elements; curved tubes are split into short chords. A member can be a truss (axial only,
for pinned clevis braces) or have a moment release at either end (pinned end)."""
import math
import numpy as np


# --- sections ------------------------------------------------------------------------------------
def round_tube(od, t):
    di = od - 2 * t
    A = math.pi / 4 * (od ** 2 - di ** 2)
    I = math.pi / 64 * (od ** 4 - di ** 4)
    return dict(kind='round', od=od, t=t, A=A, Iy=I, Iz=I, J=2 * I, Sy=I / (od / 2), Sz=I / (od / 2), c=od / 2)


def rect_tube(b, h, t):
    """b along the member's local y, h along local z (corner radii ignored)."""
    A = b * h - (b - 2 * t) * (h - 2 * t)
    Iy = (b * h ** 3 - (b - 2 * t) * (h - 2 * t) ** 3) / 12   # bending about local y (depth h)
    Iz = (h * b ** 3 - (h - 2 * t) * (b - 2 * t) ** 3) / 12
    am = (b - t) * (h - t)
    J = 4 * am ** 2 * t / (2 * (b - t) + 2 * (h - t))
    return dict(kind='rect', b=b, h=h, t=t, A=A, Iy=Iy, Iz=Iz, J=J, Sy=Iy / (h / 2), Sz=Iz / (b / 2), c=max(b, h) / 2)


def rigid():
    return dict(kind='rigid', A=50.0, Iy=500.0, Iz=500.0, J=500.0, Sy=1e9, Sz=1e9, c=1.0)


class Frame:
    def __init__(self):
        self.nodes, self.names, self.els = [], {}, []
        self.fix = {}       # node -> set of fixed dofs (0..5)
        self.springs = []   # (node, dof, k)

    def node(self, name, xyz):
        if name in self.names:
            return self.names[name]
        self.names[name] = len(self.nodes)
        self.nodes.append(np.array(xyz, float))
        return self.names[name]

    def member(self, group, a, b, sec, mat, up=(0, 0, 1), truss=False, release=(False, False), weld=(True, True)):
        """Element from node a to node b. up: a vector in the local x-z plane (sets local z)."""
        ia, ib = (self.names[a] if isinstance(a, str) else a), (self.names[b] if isinstance(b, str) else b)
        self.els.append(dict(group=group, a=ia, b=ib, sec=sec, mat=mat, up=np.array(up, float), truss=truss,
                             release=release, weld=weld))

    def chain(self, group, names, sec, mat, **kw):
        for a, b in zip(names[:-1], names[1:]):
            self.member(group, a, b, sec, mat, **kw)

    def support(self, name, dofs='xyzrst'):
        self.fix.setdefault(self.names[name], set()).update('xyzrst'.index(d) for d in dofs)

    # --- element matrices -----------------------------------------------------------------------
    def _frame(self, e):
        p, q = self.nodes[e['a']], self.nodes[e['b']]
        L = np.linalg.norm(q - p)
        ex = (q - p) / L
        up = e['up'] if abs(np.dot(e['up'] / np.linalg.norm(e['up']), ex)) < 0.99 else np.array([1.0, 0, 0]) if abs(ex[0]) < 0.9 else np.array([0, 1.0, 0])
        ez = up - np.dot(up, ex) * ex
        ez /= np.linalg.norm(ez)
        ey = np.cross(ez, ex)
        return L, np.vstack([ex, ey, ez])

    def _klocal(self, e, L):
        s, m = e['sec'], e['mat']
        E = m['E'] * (1000 if s['kind'] == 'rigid' else 1)
        G = E / 2.6
        A, Iy, Iz, J = s['A'], s['Iy'], s['Iz'], s['J']
        k = np.zeros((12, 12))
        ea = E * A / L
        k[0, 0] = k[6, 6] = ea
        k[0, 6] = k[6, 0] = -ea
        if e['truss']:
            return k
        gj = G * J / L
        k[3, 3] = k[9, 9] = gj
        k[3, 9] = k[9, 3] = -gj
        for (I, v, r, sg) in ((Iz, 1, 5, 1), (Iy, 2, 4, -1)):   # bending in local xy (about z) and xz (about y)
            a, b_, c = 12 * E * I / L ** 3, 6 * E * I / L ** 2, E * I / L
            idx = [v, r, v + 6, r + 6]
            kk = np.array([[a, sg * b_, -a, sg * b_],
                           [sg * b_, 4 * c, -sg * b_, 2 * c],
                           [-a, -sg * b_, a, -sg * b_],
                           [sg * b_, 2 * c, -sg * b_, 4 * c]])
            for i in range(4):
                for j in range(4):
                    k[idx[i], idx[j]] = kk[i, j]
        # moment releases by static condensation of the released rotations
        rel = [d for end, on in zip((0, 6), e['release']) if on for d in (end + 4, end + 5)]
        if rel:
            keep = [d for d in range(12) if d not in rel]
            krr = k[np.ix_(rel, rel)]
            kkr = k[np.ix_(keep, rel)]
            kc = k[np.ix_(keep, keep)] - kkr @ np.linalg.solve(krr, kkr.T)
            k = np.zeros((12, 12))
            k[np.ix_(keep, keep)] = kc
        return k

    def solve(self, loads):
        """loads: {node name: (Fx, Fy, Fz, Mx, My, Mz)}. Returns displacements and element end forces."""
        n = len(self.nodes)
        K = np.zeros((6 * n, 6 * n))
        F = np.zeros(6 * n)
        cache = []
        for e in self.els:
            L, R = self._frame(e)
            T = np.zeros((12, 12))
            for i in range(4):
                T[3 * i:3 * i + 3, 3 * i:3 * i + 3] = R
            kl = self._klocal(e, L)
            kg = T.T @ kl @ T
            dofs = list(range(6 * e['a'], 6 * e['a'] + 6)) + list(range(6 * e['b'], 6 * e['b'] + 6))
            K[np.ix_(dofs, dofs)] += kg
            cache.append((L, T, kl, dofs))
        for name, f in loads.items():
            i = self.names[name]
            F[6 * i:6 * i + 6] += np.array(f, float)
        for i, d, k in self.springs:
            K[6 * i + d, 6 * i + d] += k
        fixed = sorted(6 * i + d for i, ds in self.fix.items() for d in ds)
        # rotational dofs touched only by truss members get a tiny spring (no rotational stiffness)
        diag = np.diag(K).copy()
        for d in range(6 * n):
            if d % 6 >= 3 and diag[d] == 0 and d not in fixed:
                K[d, d] = 1.0
        free = [d for d in range(6 * n) if d not in set(fixed)]
        U = np.zeros(6 * n)
        U[free] = np.linalg.solve(K[np.ix_(free, free)], F[free])
        R = K @ U - F
        out = []
        for e, (L, T, kl, dofs) in zip(self.els, cache):
            f = kl @ (T @ U[dofs])
            out.append(dict(e=e, L=L, f=f))
        react = {nm: R[6 * i:6 * i + 6] for nm, i in self.names.items() if i in self.fix}
        disp = {nm: U[6 * i:6 * i + 6] for nm, i in self.names.items()}
        return dict(U=U, members=out, react=react, disp=disp)


def stress(r):
    """Largest von Mises-type stress at either end of an element (ksi when E is in ksi and loads in kip;
    here lb and in, so psi). Returns (sigma, N) with N positive in tension."""
    e, f = r['e'], r['f']
    s = e['sec']
    N = -f[0]                       # axial at end a, positive tension
    best = 0.0
    for o in (0, 6):
        sgn = 1 if o == 6 else -1
        T, My, Mz = f[o + 3], f[o + 4], f[o + 5]
        if s['kind'] == 'round':
            sb = math.hypot(My, Mz) / s['Sy']
        else:
            sb = abs(My) / s['Sy'] + abs(Mz) / s['Sz']
        tau = abs(T) * s['c'] / s['J'] if s['J'] else 0
        sig = abs(N) / s['A'] + sb
        best = max(best, math.sqrt(sig ** 2 + 3 * tau ** 2))
    return best, N
