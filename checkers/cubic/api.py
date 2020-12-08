import os
os.environ["PWNLIB_NOTERM"] = "True"
from pwn import remote, context
import traceback

from utils import encrypt, decrypt, get_info, get_users_pub_cube, get_users_ciphercubes, list_users, login, register, _exit

context.log_level = 'error'


class Api:
    def __init__(self, host):
        host, port = host.split(":")
        self.addr = host, int(port)
        self.r = None

    def connect(self):
        self.r = remote(*self.addr)

    def disconnect(self):
        try:
            _exit(self.r)
        except Exception:
            print("Can't exit :(")
        if self.r:
            self.r.close()
        self.r = None

    def login(self, user, cube):
        try:
            pub_cube = login(self.r, user, cube)
            return pub_cube
        except Exception as ex:
            print(ex)

    def register(self, user, cube, info):
        try:
            pub_cube = register(self.r, user, cube, info)
            return pub_cube
        except Exception as ex:
            print(ex)

    def get_info(self):
        try:
            return get_info(self.r)
        except Exception as ex:
            print(ex)

    def encrypt(self, data):
        try:
            return encrypt(self.r, data)
        except Exception as ex:
            print(ex)

    def decrypt(self, priv_key):
        try:
            return decrypt(self.r, priv_key)
        except Exception as ex:
            print(ex)

    def list_users(self):
        try:
            return list_users(self.r)
        except Exception as ex:
            print(ex)

    def get_users_pub_cube(self, user):
        try:
            return get_users_pub_cube(self.r, user)
        except Exception as ex:
            print(ex)

    def get_users_ciphercubes(self, user):
        try:
            return get_users_ciphercubes(self.r, user)
        except Exception as ex:
            print(ex)