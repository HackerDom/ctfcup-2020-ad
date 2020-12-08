from enum import IntEnum

from cube.const import center_facelet, corner_facelet, edge_facelet, center_color, corner_color, edge_color, face2num ,face2move, num2face
from cube.utils import parse_moves, generate_valid_random_permutation, generate_valid_random_orientation

class Cube:
    def __init__(self, other=None):
        if other is None:
            self._identity()
        else:
            self.clone(other)

        self._new_center = [0]*6
        self._new_cp = [0]*8
        self._new_co = [0]*8
        self._new_ep = [0]*12
        self._new_eo = [0]*12

    def __eq__(self, other):
        return self.as_dict() == other.as_dict()

    def _identity(self):
        self.center = list(range(6))
        self.cp = list(range(8))
        self.co = [0]*8
        self.ep = list(range(12))
        self.eo = [0]*12

    def clone(self, other):
        self.center = other.center[:]
        self.cp = other.cp[:]
        self.co = other.co[:]
        self.ep = other.ep[:]
        self.eo = other.eo[:]

    def as_dict(self):
        return {
            'center': self.center,
            'cp': self.cp,
            'co': self.co,
            'ep': self.ep,
            'eo': self.eo
        }

    @staticmethod
    def from_dict(d):
        cube = Cube()
        cube.center = d['center'][:]
        cube.cp = d['cp'][:]
        cube.co = d['co'][:]
        cube.ep = d['ep'][:]
        cube.eo = d['eo'][:]
        return cube

    def as_str(self):
        result = [None]*54
        for i in range(6):
            result[center_facelet[i]] = center_color[self.center[i]]
        for i in range(8):
            for n in range(3):
                result[corner_facelet[i][(n + self.co[i]) % 3]] = corner_color[self.cp[i]][n]
        for i in range(12):
            for n in range(2):
                result[edge_facelet[i][(n + self.eo[i]) % 2]] = edge_color[self.ep[i]][n]
        return ''.join(result)

    @staticmethod
    def from_str(s):
        cube = Cube()
        for i in range(6):
            cube.center[i] = center_color.index(s[center_facelet[i]])

        for i in range(8):
            corner = ''.join(s[x] for x in corner_facelet[i])
            orientation = corner.index('U') if 'U' in corner else corner.index('D')
            corner = corner[orientation:] + corner[:orientation]

            cube.cp[i] = corner_color.index(corner)
            cube.co[i] = orientation % 3

        for i in range(12):
            edge = ''.join(s[x] for x in edge_facelet[i])

            if edge in edge_color:
                cube.ep[i] = edge_color.index(edge)
                cube.eo[i] = 0
            else:
                cube.ep[i] = edge_color.index(edge[::-1])
                cube.eo[i] = 1

        return cube

    def _center_multiply(self, other):
        for i in range(6):
            self._new_center[i] = self.center[other.center[i]]
        self.center, self._new_center = self._new_center, self.center
        return self

    def _corner_multiply(self, other):
        for i in range(8):
            self._new_cp[i] = self.cp[other.cp[i]]
            self._new_co[i] = (self.co[other.cp[i]] + other.co[i]) % 3
        self.cp, self._new_cp = self._new_cp, self.cp
        self.co, self._new_co = self._new_co, self.co
        return self

    def _edge_multiply(self, other):
        for i in range(12):
            self._new_ep[i] = self.ep[other.ep[i]]
            self._new_eo[i] = (self.eo[other.ep[i]] + other.eo[i]) % 2
        self.ep, self._new_ep = self._new_ep, self.ep
        self.eo, self._new_eo = self._new_eo, self.eo
        return self

    def multiply(self, other):
        return self._center_multiply(other)._corner_multiply(other)._edge_multiply(other)

    def pow(self, n):
        r = Cube()
        x = Cube(self)
        while n:
            if n % 2:
                r.multiply(x)
            x.multiply(x)
            n //= 2

        return r

    def move(self, arg):
        moves = parse_moves(arg)
        if moves is None:
            return None

        for move in moves:
            face = move // 3
            power = move % 3
            for _ in range(power+1):
                self.multiply(Cube.from_dict(face2move[face]))
        return self

    def _upright(self):
        clone = Cube(self)

        result = []
        move = {0: "x'", 1: "y", 3: "x", 4: "y'", 5: "x2"}.get(clone.center.index(2))
        if move is not None:
            result.append(move)
            clone.move(move)

        move = {1: "z'", 3: "z2", 4: "z"}.get(clone.center.index(0))
        if move is not None:
            result.append(move)

        return ' '.join(result)

    def is_solved(self):
        clone = Cube(self)
        clone.move(clone._upright())

        return (
            clone.center == list(range(6)) and
            clone.cp == list(range(8)) and
            not any(clone.co) and
            clone.ep == list(range(12)) and
            not any(clone.eo)
        )

    @staticmethod
    def inverse(moves):
        result = []
        for move in parse_moves(moves):
            result.append(num2face[move // 3] + {0: "'", 1: "2", 2: ""}[move % 3])
        return ' '.join(result[::-1])

    @staticmethod
    def generate():
        cube = Cube()
        generate_valid_random_permutation(cube.cp, cube.ep)
        generate_valid_random_orientation(cube.co, cube.eo)
        return cube
