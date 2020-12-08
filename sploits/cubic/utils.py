from base64 import b64decode
from json import dumps, loads
from random import choice
from string import ascii_letters, digits

from cube.cube import Cube


ALPHA = ascii_letters + digits


def generate_user():
    return ''.join(choice(ALPHA) for _ in range(15))

def generate_info():
    return ''.join(choice(ALPHA) for _ in range(32))

def generate_cube():
    return Cube.generate()

def register(r, user:str, cube:Cube, info:str):
    r.sendlineafter(b'> ', b'1')
    r.sendlineafter(b'> ', user)
    r.sendlineafter(b'> ', cube.as_str())
    r.sendlineafter(b'> ', info)

    r.recvuntil(b'Your public cube is ')
    pub_cube = r.recvline(False).decode()
    try:
        return Cube.from_str(pub_cube)
    except Exception:
        return None

def login(r, user:str, cube:Cube):
    r.sendlineafter(b'> ', b'2')
    r.sendlineafter(b'> ', user)
    r.sendlineafter(b'> ', cube.as_str())

    r.recvuntil(b'Your public cube is ')
    pub_cube = r.recvline(False).decode()
    try:
        return Cube.from_str(pub_cube)
    except Exception:
        return None

def _exit(r):
    r.sendlineafter(b'> ', b'0')

def get_info(r):
    r.sendlineafter(b'> ', b'1')
    r.recvuntil(b'Your info: ')
    return r.recvline(False).decode()

def encrypt(r, flag):
    r.sendlineafter(b'> ', b'5')
    r.sendlineafter(b'> ', b'1')
    r.sendlineafter(b'>', flag.encode().hex())
    r.recvuntil(b'Your ciphercubes: ')
    ct = r.recvline(False)
    r.recvuntil(b'Your private key: ')
    priv_key = int(r.recvline(False).decode())
    return ct, priv_key

def decrypt(r, priv_key):
    r.sendlineafter(b'> ', b'5')
    r.sendlineafter(b'> ', b'1')
    r.sendlineafter(b'> ', str(priv_key))
    r.recvuntil(b"Your decrypted data: ")
    return bytes.fromhex(r.recvline(False).decode()).decode()

def list_users(r):
    r.sendlineafter(b'> ', b'2')
    r.recvuntil(b"Found ")
    users_count = int(r.recvline(False).decode().split()[0])
    users = set()
    for _ in range(users_count):
        user = r.recvline(False).decode().strip()
        if not user:
            break
        users.add(user[2:])
    return users

def get_users_pub_cube(r, user):
    r.sendlineafter(b'> ', b'3')
    r.sendlineafter(b"> ", user)
    r.recvuntil(b"User's public cube: ")
    pub_cube = r.recvline(False).decode()
    if pub_cube:
        return Cube.from_str(pub_cube)


def get_users_ciphercubes(r, user):
    r.sendlineafter(b'> ', b'4')
    r.sendlineafter(b'> ', user)
    data = r.recvline(False)
    if b'yet' in data:
        return None
    data = data.split(b": ")[1]
    return data