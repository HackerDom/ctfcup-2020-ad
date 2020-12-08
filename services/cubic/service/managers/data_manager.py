from cipher import Cipher

MENU = """0: exit
1: my info
2: list users
3: get user's public cube
4: get user's ciphercubes
5: go to cipher
"""

class DataManager:
    def __init__(self, db, cipher_manager):
        self.db = db
        self.cipher_manager = cipher_manager

    def list_users(self):
        users = self.db.get_users()
        print(f"Found {len(users)} users:")
        for user in users:
            print(f"- {user}")
        print()

    def get_users_public_cube(self):
        print("Give me user's login")
        login = input("> ")
        if not self.db.check_user(login):
            print("No user with this login!")
            return
        pub_cube = self.db.get_user_data(login)[0]
        print(f"User's public cube: {pub_cube.as_str()}")

    def get_users_ciphercubes(self):
        print("Give me user's login")
        login = input("> ")
        if not self.db.check_user(login):
            print("No user with this login!")
            return
        ciphercubes = self.db.get_ciphercube(login)
        if ciphercubes:
            print(f"User's ciphercubes: {ciphercubes.decode()}")
        else:
            print("User has no ciphercubes yet")

    def process(self, login, info):
        while True:
            print(MENU)
            try:
                _input = int(input("> "))
            except Exception:
                return

            if _input == 0:
                print("Bye bye!")
                return
            elif _input == 1:
                print(f"Your info: {info}")
            elif _input == 2:
                self.list_users()
            elif _input == 3:
                self.get_users_public_cube()
            elif _input == 4:
                self.get_users_ciphercubes()
            elif _input == 5:
                self.cipher_manager.process()
            else:
                return