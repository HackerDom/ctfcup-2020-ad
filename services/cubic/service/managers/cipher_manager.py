from json import dumps, loads
from base64 import b64encode, b64decode
from cipher import Cipher

from cube.cube import Cube


ENC_MENU = """0: back
1: encrypt
"""

DEC_MENU = """0: back
1: decrypt
"""

class CipherManager:
    def __init__(self, db, login):
        self.db = db
        self.login = login

    def encrypt(self):
        pub_cube = self.db.get_user_data(self.login)[0]
        cipher = Cipher(pub_cube)

        print("Give me something to encrypt")
        data = input("> ")
        if not data:
            return
        try:
            data = bytes.fromhex(data)
        except Exception:
            return
        
        encrypted = b64encode(dumps([(c1.as_str(), c2.as_str()) for c1, c2 in cipher.encrypt(data)]).encode())
        self.db.set_ciphercube(self.login, encrypted)

        print(f"Your ciphercubes: {encrypted.decode()}")
        print(f"Your private key: {cipher.x}")

    def decrypt(self):
        pub_cube = self.db.get_user_data(self.login)[0]
        print("Give me your private key")
        try:
            key = int(input("> "))
        except Exception:
            return

        ct = [tuple(map(Cube.from_str, x)) for x in loads(b64decode(self.db.get_ciphercube(self.login).decode()))]
        cipher = Cipher(pub_cube, key)
        
        print(f"Your decrypted data: {cipher.decrypt(ct).hex()}")

    def process(self):
        has_ciphercubes = self.db.check_ciphercube(self.login)

        print(DEC_MENU if has_ciphercubes else ENC_MENU)
        try:
            _input = int(input("> "))
        except Exception:
            return

        if _input == 0:
            print("OK")
            return
        elif _input == 1:
            if has_ciphercubes:
                self.decrypt()
            else:
                self.encrypt()
        else:
            return