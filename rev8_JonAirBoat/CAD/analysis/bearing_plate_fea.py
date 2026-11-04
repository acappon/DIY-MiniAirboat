"""Linear FE check of the forward prop-shaft bearing plate (1/2 in 6061, 15.5 x 6 in, petal cutouts).
Run: python bearing_plate_fea.py  (needs numpy and scipy)

Mesh: 8-node brick voxels. A square-cell mesh cannot resolve a 0.5 in corner radius exactly, so the cutout corners are
stair-stepped. Peak values right at an edge are mesh-sensitive: this script reports the peak at two mesh sizes, and the
value to use is the fine-mesh one, read as an estimate.

Loads from etec_reduction.js and bearing_plate.js: thrust 497 lbf along the shaft (out of plane), radial 580 lbf in the
plane, both on the bearing bolt circle. Thrust is what the forward plate is there for. Ends: bolted to the stand side frames
over the solid 1.5 in zones, modeled two ways: simply supported (a line of support at the end bolts) and fixed (end zones
clamped). The real joint lies between them.
"""
import numpy as np
import scipy.sparse as sp
import scipy.sparse.linalg as spla

E, NU = 1.0e7, 0.33
HALF, DEPTH, T = 7.75, 6.0, 0.5
PAD, SLOT, RIB, EDGE, R = 3.1, 1.5, 0.5, 1.0, 0.5
THRUST, RADIAL = 497.0, 580.0
BOLT_R = 2.0          # 4-bolt flange, bolt circle radius about the bore
YIELD, ULT, FATIGUE = 35000.0, 42000.0, 13500.0


def solid(y, z):
    """True where the plate has metal. y along the plate, z across the depth, both from the bearing center."""
    zone = HALF - SLOT - PAD
    w = (zone - RIB) / 2
    h = DEPTH - 2 * EDGE
    ay = abs(y)
    if abs(z) > DEPTH / 2:
        return False
    for k in range(2):
        y0 = PAD + k * (w + RIB)
        y1 = y0 + w
        dy = max(y0 + R - ay, ay - (y1 - R), 0.0)
        dz = max(abs(z) - (h / 2 - R), 0.0)
        if dy * dy + dz * dz < R * R and y0 < ay < y1 and abs(z) < h / 2:
            return False
    return True


def ke(hx, hy, hz):
    """Brick element stiffness, 2x2x2 Gauss."""
    lam, mu = E * NU / ((1 + NU) * (1 - 2 * NU)), E / (2 * (1 + NU))
    D = np.zeros((6, 6))
    D[:3, :3] = lam
    D[np.arange(3), np.arange(3)] += 2 * mu
    D[np.arange(3, 6), np.arange(3, 6)] = mu
    nodes = np.array([[-1, -1, -1], [1, -1, -1], [1, 1, -1], [-1, 1, -1], [-1, -1, 1], [1, -1, 1], [1, 1, 1], [-1, 1, 1]], float)
    K = np.zeros((24, 24))
    g = 1 / np.sqrt(3)
    for a in (-g, g):
        for b in (-g, g):
            for c in (-g, g):
                B = bmat(nodes, a, b, c, hx, hy, hz)
                K += B.T @ D @ B * (hx * hy * hz / 8)
    return K, D, nodes


def bmat(nodes, a, b, c, hx, hy, hz):
    dN = np.zeros((3, 8))
    for i, n in enumerate(nodes):
        dN[0, i] = n[0] * (1 + n[1] * b) * (1 + n[2] * c) / 8 * 2 / hx
        dN[1, i] = n[1] * (1 + n[0] * a) * (1 + n[2] * c) / 8 * 2 / hy
        dN[2, i] = n[2] * (1 + n[0] * a) * (1 + n[1] * b) / 8 * 2 / hz
    B = np.zeros((6, 24))
    for i in range(8):
        B[0, 3 * i] = dN[0, i]
        B[1, 3 * i + 1] = dN[1, i]
        B[2, 3 * i + 2] = dN[2, i]
        B[3, 3 * i] = dN[1, i]
        B[3, 3 * i + 1] = dN[0, i]
        B[4, 3 * i + 1] = dN[2, i]
        B[4, 3 * i + 2] = dN[1, i]
        B[5, 3 * i] = dN[2, i]
        B[5, 3 * i + 2] = dN[0, i]
    return B


def run(h, nt, ends, fx, fz):
    """x through the thickness, y along, z across. fx thrust, fz radial. Returns peak von Mises and center deflection."""
    ny, nz = int(round(2 * HALF / h)), int(round(DEPTH / h))
    hy, hz, hx = 2 * HALF / ny, DEPTH / nz, T / nt
    ys = (np.arange(ny) + 0.5) * hy - HALF
    zs = (np.arange(nz) + 0.5) * hz - DEPTH / 2
    mask = np.array([[solid(y, z) for z in zs] for y in ys])
    Ke, D, nodes = ke(hx, hy, hz)
    nid = lambda i, j, k: (i * (nz + 1) + j) * (nt + 1) + k
    rows, cols, vals, elems = [], [], [], []
    for i in range(ny):
        for j in range(nz):
            if not mask[i, j]:
                continue
            for k in range(nt):
                # Local order: x=thickness(k), y=along(i), z=across(j)
                conn = [nid(i, j, k), nid(i, j, k + 1), nid(i + 1, j, k + 1), nid(i + 1, j, k), nid(i, j + 1, k), nid(i, j + 1, k + 1), nid(i + 1, j + 1, k + 1), nid(i + 1, j + 1, k)]
                dofs = np.array([[3 * n, 3 * n + 1, 3 * n + 2] for n in conn]).ravel()
                elems.append(dofs)
    elems = np.array(elems)
    rows = np.repeat(elems, 24, axis=1).ravel()
    cols = np.tile(elems, (1, 24)).ravel()
    vals = np.tile(Ke.ravel(), len(elems))
    N = 3 * (ny + 1) * (nz + 1) * (nt + 1)
    K = sp.coo_matrix((vals, (rows, cols)), shape=(N, N)).tocsc()
    used = np.zeros(N, bool)
    used[np.unique(elems)] = True

    def y_at(i):
        return -HALF + i * hy

    def z_at(j):
        return -DEPTH / 2 + j * hz

    fixed = np.zeros(N, bool)
    F = np.zeros(N)
    bolt = []
    for i in range(ny + 1):
        for j in range(nz + 1):
            yy, zz = y_at(i), z_at(j)
            for k in range(nt + 1):
                n = nid(i, j, k)
                if not used[3 * n]:
                    continue
                if ends == 'fixed' and abs(yy) >= HALF - SLOT:
                    fixed[[3 * n, 3 * n + 1, 3 * n + 2]] = True
                if ends == 'simple' and abs(abs(yy) - (HALF - 0.75)) < hy / 2:
                    fixed[3 * n] = True           # thrust support at the end bolts
                    if k == nt // 2:
                        fixed[3 * n + 2] = True   # radial support
                        if yy < 0 and abs(zz) < hz:
                            fixed[3 * n + 1] = True
            if abs(np.hypot(yy, zz) - BOLT_R) < 1.0 * max(hy, hz):
                bolt.append((i, j))
    for i, j in bolt:
        for k in (0, nt):
            n = nid(i, j, k)
            F[3 * n] += fx / (2 * len(bolt))
            F[3 * n + 2] += fz / (2 * len(bolt))
    free = np.where(used & ~fixed)[0]
    u = np.zeros(N)
    A = K[free][:, free]
    M = sp.diags(1 / A.diagonal())
    x, info = spla.cg(A, F[free], rtol=1e-7, maxiter=60000, M=M)
    u[free] = x if info == 0 else spla.spsolve(A, F[free])

    # Stress at the element centers.
    B = bmat(nodes, 0, 0, 0, hx, hy, hz)
    s = (D @ (B @ u[elems].T)).T
    vm = np.sqrt(0.5 * ((s[:, 0] - s[:, 1]) ** 2 + (s[:, 1] - s[:, 2]) ** 2 + (s[:, 2] - s[:, 0]) ** 2) + 3 * (s[:, 3] ** 2 + s[:, 4] ** 2 + s[:, 5] ** 2))
    # Skip elements under the bolt ring and at the supports: those are local to the model's point loads.
    cx = np.array([(e[0] // 3) // (nt + 1) // (nz + 1) for e in elems])
    cy = -HALF + (cx + 0.5) * hy
    keep = (np.abs(cy) > PAD - 0.2) & (np.abs(cy) < HALF - SLOT + 0.2)
    peak = vm[keep].max()
    mid = nid(ny // 2, nz // 2, nt // 2)
    return peak, abs(u[3 * mid]), len(elems), np.percentile(vm[keep], 95)


if __name__ == '__main__':
    print('Peak von Mises at the petal cutouts, psi. Cutout edges are stair-stepped.')
    for h, nt in [(0.25, 2), (0.125, 3), (0.0625, 4)]:
        for ends in ('simple', 'fixed'):
            for name, fx, fz in [('thrust only', THRUST, 0), ('thrust + radial', THRUST, RADIAL)]:
                peak, d, n, p95 = run(h, nt, ends, fx, fz)
                print(f'h {h:5.3f}  {n:6d} el  {ends:6s}  {name:15s}  peak {peak:7.0f}  95th pct {p95:6.0f}  center defl {d:.4f} in  yield factor {YIELD / peak:.1f}')
