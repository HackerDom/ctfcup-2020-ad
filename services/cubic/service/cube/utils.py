from random import shuffle, randint

from cube.const import face2num


BLOCK_SIZE = 8


def pad_msg(msg):
    padding = (- len(msg)) % BLOCK_SIZE
    return msg + bytes([padding]*padding)

def unpad_msg(msg):
    return msg[:-msg[-1]] if msg[-1] <= BLOCK_SIZE else msg

def split_msg(msg):
    return [msg[i:i+BLOCK_SIZE] for i in range(0, len(msg), BLOCK_SIZE)]

def join_msg(blocks):
    return b''.join(blocks)

def parse_moves(moves):
    if type(moves) is list:
        return moves
    if type(moves) is not str:
        return None

    moves = moves.strip()
    if not moves:
        return []

    res = []
    for move in moves.split(" "):
        if len(move) > 2:
            return None
        face = face2num.get(move[0])
        if face is None:
            return None

        if len(move) == 1:
            power = 0
        elif move[1] == '2':
            power = 1
        elif move[1] == "'":
            power = 2
        else:
            return None
        res.append(face*3+power)
    return res

def count_swaps(arr):
    seen = [False]*len(arr)
    count = 0
    while True:
        if all(seen):
            break
        cur = seen.index(False)
        cycle = 0
        while not seen[cur]:
            seen[cur] = True
            cycle += 1
            cur = arr[cur]
        count += cycle + 1
    return count

def are_permutations_valid(cp, ep):
    return not ((count_swaps(ep) + count_swaps(cp)) % 2)

def generate_valid_random_permutation(cp, ep):
    shuffle(cp)
    shuffle(ep)
    while not are_permutations_valid(cp, ep):
        shuffle(cp)
        shuffle(ep)

def randomize_orientation(arr, num_orientations):
    for i in range(len(arr)):
        arr[i] = randint(0, num_orientations) % num_orientations

def is_orientation_valid(arr, num_orientations):
    return not (sum(arr) % num_orientations)

def generate_valid_random_orientation(co, eo):
    randomize_orientation(co, 3)
    while not is_orientation_valid(co, 3):
        randomize_orientation(co, 3)
    
    randomize_orientation(eo, 2)
    while not is_orientation_valid(eo, 2):
        randomize_orientation(eo, 2)