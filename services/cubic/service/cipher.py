from random import randint
from Crypto.Util.number import long_to_bytes, bytes_to_long


from cube.const import ORDER
from cube.cube import Cube
from cube.utils import pad_msg, unpad_msg, split_msg, join_msg


def get_parity(perm):
    parity = 0
    for _ in range(len(perm)):
        for j in range(len(perm)-1):
            if perm[j] > perm[j+1]:
                perm[j], perm[j+1] = perm[j+1], perm[j]
                parity ^= 1
    return parity

def long2cube(number):
    cp = []
    corners = list(range(8))    
    for i in range(8, 0, -1):
        number, index = number // i, number % i
        cp.append(corners.pop(index))

    ep = []
    edges = list(range(12))
    for i in range(12, 2, -1):
        number, index = number // i, number % i
        ep.append(edges.pop(index))

    if get_parity(cp[:]) ^ get_parity(ep[:] + edges[:]):
        ep.append(edges[1])
        ep.append(edges[0])
    else:
        ep.append(edges[0])
        ep.append(edges[1])

    co = []
    for i in range(7):
        number, index = number // 3, number % 3
        co.append(index)
    co.append((24 - sum(co)) % 3)

    eo = []
    for i in range(12):
        number, index = number // 2, number % 2
        eo.append(index)
    eo.append((24-sum(eo)) % 2)

    cube = Cube.from_dict({
        'center': list(range(8)),
        'cp': cp,
        'ep': ep,
        'co': co,
        'eo': eo
    })

    return cube

def cube2long(cube):
    number = 0
    cp, ep, co, eo = cube.cp, cube.ep, cube.co, cube.eo

    for x in eo[::-1]:
        number = number*2 + x

    for x in co[::-1][1:]:
        number = number*3 + x

    rands = []
    edges = list(range(12))
    for x in ep[:-2]:
        i = edges.index(x)
        rands.append(i)
        edges.pop(i)

    for i, x in enumerate(rands[::-1]):
        number = number*(i+3) + x

    rands = []
    corners = list(range(8))
    for x in cp:
        i = corners.index(x)
        rands.append(i)
        corners.pop(i)

    for i, x in enumerate(rands[::-1]):
        number = number*(i+1) + x

    return number

class Cipher:
    def __init__(self, g=None, x=None):
        self.g = g or Cube.generate()
        self.x = x or randint(1, ORDER-1)

    def priv_key(self):
        return self.x

    def pub_key(self):
        return self.g

    def _extend_keys(self, l):
        gs = [self.g]
        xs = [self.x]
        for _ in range(l - 1):
            gs.append(Cube(gs[-1]).move("U x'"))
            xs.append((xs[-1] + 0xDEADBEAF) % ORDER)
        return gs, xs

    def _encrypt(self, msg, g, x):
        m = long2cube(bytes_to_long(msg))
        y = randint(1, ORDER-1)

        return g.pow(y), Cube(m).multiply(g.pow(x*y))

    def _decrypt(self, c1, c2, g, x):
        return long_to_bytes(cube2long(Cube(c2).multiply(c1.pow(ORDER - x))))

    def encrypt(self, msg):
        msg = split_msg(msg)

        gs, xs = self._extend_keys(len(msg))
        result = []
        for block, g, x in zip(msg, gs, xs):
            result.append(self._encrypt(pad_msg(block), g, x))
        return result

    def decrypt(self, c):
        gs, xs = self._extend_keys(len(c))
        result = []
        for (c1, c2), g, x in zip(c, gs, xs):
            result.append(unpad_msg(self._decrypt(c1, c2, g, x)))
        return join_msg(result)