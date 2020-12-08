#!/usr/bin/env python3
import sys

from managers.auth_manager import AuthManager
from managers.cipher_manager import CipherManager
from managers.data_manager import DataManager
from db import DB


def main():
    db = DB()

    auth_manager = AuthManager(db)
    user = auth_manager.process()

    if user is None:
        return
    login, pub_cube, info = user

    cipher_manager = CipherManager(db, login)

    data_manager = DataManager(db, cipher_manager)
    data_manager.process(login, info)


if __name__ == '__main__':
    main()
    sys.exit(0)