#!/usr/bin/env python3.7

from gornilo import Checker, CheckRequest, PutRequest, GetRequest, Verdict
from api import Api
from uuid import uuid4
from requests.exceptions import RequestException

checker = Checker()


def randomize() -> str:
    return str(uuid4())


@checker.define_check
def check(check_request: CheckRequest) -> Verdict:
    api = Api()
    try:
        rand_uuid = api.add_new_chess_unit(check_request.hostname, randomize(), randomize())
        if len(rand_uuid) > 0:
            return Verdict.OK()
    except RequestException as e:
        print(f"can't connect due to {e}")
        return Verdict.DOWN("can't connect to host")
    except Exception as e:
        print(e)
        return Verdict.MUMBLE("bad proto")


@checker.define_put(vuln_num=1, vuln_rate=1)
def put(put_request: PutRequest) -> Verdict:
    api = Api()
    try:
        unit_name = randomize()
        id_of_basement = api.add_new_chess_unit(put_request.hostname, unit_name, put_request.flag)
        armory_id = randomize()
        new_armory_id = api.add_armory_unit(put_request.hostname, armory_id)
        result = api.add_unit_to_chess(put_request.hostname, unit_name, new_armory_id)
        last_50_objects = api.get_latest_objects(put_request.hostname, 50)
        if "Armory" not in str(api.object_info(put_request.hostname, unit_name)):
            print("Armory not in object")
            return Verdict.MUMBLE("Bad object")
        if unit_name not in last_50_objects or result not in last_50_objects or armory_id not in last_50_objects:
            print("last 50 object doesnt contain needed info")
            return Verdict.MUMBLE("bad objects listing")
        if result != unit_name:
            print("result != unit name")
            return Verdict.MUMBLE("bad object id after adding")
        if armory_id != new_armory_id:
            print("bad armory id")
            return Verdict.MUMBLE("bad object id after adding")
        return Verdict.OK(f"{unit_name}:{id_of_basement}")
    except RequestException as e:
        print(f"timeout on connect {e}")
        return Verdict.DOWN("timeout")
    except Exception as e:
        print(f"possible mumble, {e}")
        return Verdict.MUMBLE("bad proto")


@checker.define_get(vuln_num=1)
def get(get_request: GetRequest) -> Verdict:
    api = Api()
    try:
        addr, secret = get_request.flag_id.strip().split(":")
        try:
            resulting_info = api.basement_info(get_request.hostname, addr, secret)
            if get_request.flag in resulting_info:
                return Verdict.OK()
            else:
                print(resulting_info, get_request.flag)
                return Verdict.CORRUPT("bad flag")
        except Exception as e:
            print(f"bad access {e}")
            return Verdict.CORRUPT("can't reach flag")
    except RequestException as e:
        print(e)
        return Verdict.DOWN("seems to be down")
    except Exception as e:
        print(f"ex {e}")
        return Verdict.MUMBLE("bad proto")


checker.run()
