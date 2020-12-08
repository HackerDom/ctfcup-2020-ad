#!/usr/bin/env python3.7

import sys
argv = sys.argv[1:]
from gornilo import Checker, CheckRequest, PutRequest, GetRequest, Verdict
from api import Api
from utils import generate_cube, generate_info, generate_user, check_encryption_correctnes
from cube.cube import Cube

checker = Checker()


@checker.define_check
def check(check_request: CheckRequest) -> Verdict:
    user = generate_user()
    cube = generate_cube()
    info = generate_info()

    api = Api(check_request.hostname)

    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")
    register_pub_cube = api.register(user, cube, info)
    api.disconnect()
    if register_pub_cube is None:
        return Verdict.MUMBLE("Can't register")

    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")
    login_pub_cube = api.login(user, cube)
    api.disconnect()
    if login_pub_cube is None:
        return Verdict.MUMBLE("Can't login")

    if register_pub_cube != login_pub_cube:
        return Verdict.MUMBLE("Wrong public cube")

    return Verdict.OK()

@checker.define_put(vuln_num=1, vuln_rate=1)
def put(put_request: PutRequest) -> Verdict:
    user = generate_user()
    cube = generate_cube()
    info = put_request.flag

    api = Api(put_request.hostname)

    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")
    pub_cube = api.register(user, cube, info)
    api.disconnect()
    if pub_cube is None:
        return Verdict.MUMBLE("Can't register")
    return Verdict.OK(':'.join([user, cube.as_str(), pub_cube.as_str()]))


@checker.define_get(vuln_num=1)
def get(get_request: GetRequest) -> Verdict:
    user, cube, pub_cube = get_request.flag_id.split(":")
    cube, pub_cube = map(Cube.from_str, (cube, pub_cube))
    info = get_request.flag

    api = Api(get_request.hostname)

    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")
    login_pub_cube = api.login(user, cube)
    if login_pub_cube is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't login")
    if login_pub_cube != pub_cube:
        api.disconnect()
        return Verdict.MUMBLE("Wrong public cube")

    server_info = api.get_info()
    if server_info is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't get info")

    api.disconnect()

    if server_info != info:
        return Verdict.CORRUPT("Wrong user info")

    return Verdict.OK()


@checker.define_put(vuln_num=2, vuln_rate=1)
def put(put_request: PutRequest) -> Verdict:
    user = generate_user()
    cube = generate_cube()
    info = generate_info()

    api = Api(put_request.hostname)

    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")
    pub_cube = api.register(user, cube, info)

    if pub_cube is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't register")

    res = api.encrypt(put_request.flag)
    if res is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't encrypt")
    ct, priv_key = res
    api.disconnect()

    try:
        is_correct = check_encryption_correctnes(ct, pub_cube, priv_key, put_request.flag)
    except Exception:
        is_correct = False
    if not is_correct:
        return Verdict.CORRUPT("Wrong encryption")

    return Verdict.OK(':'.join([user, cube.as_str(), pub_cube.as_str(), ct.decode(), str(priv_key)]))

@checker.define_get(vuln_num=2)
def get(get_request: GetRequest) -> Verdict:
    user, cube, pub_cube, ct, priv_key = get_request.flag_id.split(":")
    cube, pub_cube = map(Cube.from_str, (cube, pub_cube))
    priv_key = int(priv_key)
    ct = ct.encode()

    api = Api(get_request.hostname)


    user2 = generate_user()
    cube2 = generate_cube()
    info2 = generate_info()

    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")

    if api.register(user2, cube2, info2) is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't register")

    users_list = api.list_users()
    if not users_list:
        api.disconnect()
        return Verdict.MUMBLE("Can't get user's list")
    
    if user not in users_list:
        api.disconnect()
        return Verdict.MUMBLE("Can't find user in user's list")

    server_users_pub_cube = api.get_users_pub_cube(user)
    if server_users_pub_cube is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't get user's pub_cube")

    if server_users_pub_cube != pub_cube:
        api.disconnect()
        return Verdict.MUMBLE("Wrong user's pub_cube")

    server_users_ciphercubes = api.get_users_ciphercubes(user)
    if server_users_ciphercubes is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't get user's ciphercubes")

    if server_users_ciphercubes != ct:
        api.disconnect()
        return Verdict.MUMBLE("Wrong user's ciphercubes")

    api.disconnect()


    try:
        api.connect()
    except Exception:
        return Verdict.DOWN("DOWN")
    login_pub_cube = api.login(user, cube)
    if login_pub_cube is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't login")
    if login_pub_cube != pub_cube:
        api.disconnect()
        return Verdict.MUMBLE("Wrong public cube")

    res = api.decrypt(priv_key)
    if res is None:
        api.disconnect()
        return Verdict.MUMBLE("Can't decrypt")

    if res != get_request.flag:
        api.disconnect()
        return Verdict.CORRUPT("Wrong decryption")

    api.disconnect()

    return Verdict.OK()

checker.run(*argv)