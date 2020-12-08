from random import randint

from cube.const import ORDER
from cube.cube import Cube


MENU = """0: exit
1: register
2: login"""


class AuthManager:
    def __init__(self, db):
        self.db = db
        self.base_cube = Cube().move("F U R U'")

    def login(self):
        print("Your name:")
        login = input("> ")
        if not self.db.check_user(login):
            print('There are no user with your login! Register first!')
            return
        pub_cube, priv_key, info = self.db.get_user_data(login)
        print(f'Give me your cube:')
        try:
            priv_cube = Cube.from_str(input("> ").strip())
        except Exception:
            print("This is not a cube!")
            return

        if priv_cube.multiply(self.base_cube.pow(priv_key)) != pub_cube:
            print('Wrong cube!')
            return

        print(f'Hello!\nYour public cube is {pub_cube.as_str()}')

        return login, pub_cube, info

    def register(self):
        print("Your name:")
        login = input("> ")
        if self.db.check_user(login):
            print('There exist user with your login!')
            return

        print('Give me your cube:')
        try:
            priv_cube = Cube.from_str(input("> ").strip())
        except Exception:
            print("This is not a cube!")
            return
        priv_key = randint(1, ORDER)
        pub_cube = priv_cube.multiply(self.base_cube.pow(priv_key))

        print('Anything about yourself:')
        info = input("> ")

        self.db.set_user_data(login, pub_cube, priv_key, info)
        print(f'Hello!\nYour public cube is {pub_cube.as_str()}')

        return login, pub_cube, info

    def process(self):
        print(MENU)
        try:
            _input = int(input("> ").strip())
        except Exception:
            return

        if _input == 0:
            print("Bye bye!")
            return
        elif _input == 1:
            user = self.register()
        elif _input == 2:
            user = self.login()
        else:
            return

        if user is None:
            return

        return user