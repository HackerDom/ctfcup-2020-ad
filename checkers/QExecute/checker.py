#!/usr/bin/env python3.7

from client import *

from gornilo import CheckRequest, Verdict, Checker, PutRequest, GetRequest

checker = Checker()


@checker.define_check
def check_service(request: CheckRequest) -> Verdict:
    try:
        hostname = request.hostname
        executor = add_executor(hostname, utils.get_executor_id(), utils.get_executor_id())
        executor_id = executor["ExecutorId"]
        executor_apikey = executor["ExecutorApiKey"]
        c_executor = get_executor(hostname, executor["ExecutorId"], executor_apikey)

        if compare_insensetive(executor, c_executor):
            return Verdict.MUMBLE(
                "GET /executor return %s, expected %s" % (json.dumps(c_executor), json.dumps(executor)))

        command_name = utils.get_command_name()
        command = {
            "Name": command_name,
            "Admins": [executor_apikey]
        }
        add_command(hostname, executor_id, executor_apikey, command_name)
        c_command = get_command(hostname, executor_id, executor_apikey, command_name)

        if compare_insensetive(command, c_command):
            return Verdict.MUMBLE("GET /command return %s, expected %s" % (json.dumps(command), json.dumps(c_command)))

        all_commands = get_commands(hostname)
        if command["Name"] not in all_commands:
            return Verdict.MUMBLE("GET /commandsList not contains %s" % command["Name"])

        admin_apikey = utils.get_executor_id()
        admin = add_executor(hostname, utils.get_executor_id(), admin_apikey)
        admin_id = admin["ExecutorId"]
        add_command_admin(hostname, executor_id, executor_apikey, command_name, admin_apikey)
        c_command = get_command(hostname, admin_id, admin_apikey, command_name)

        if admin_apikey not in c_command["Admins"] or executor_apikey not in c_command["Admins"]:
            return Verdict.MUMBLE("GET /command not contains all apikey")

        member_apikey = utils.get_executor_id()
        member = add_executor(hostname, utils.get_executor_id(), member_apikey)
        member_id = member["ExecutorId"]

        victim_name = utils.get_victim_name()
        informer_name = utils.get_victim_name()
        victim = {
            'VictimName': victim_name,
            'ExecutorId': member_id,
            'InformerName': informer_name
        }
        add_victim(hostname, member_id, member_apikey, victim_name, informer_name)
        victims = get_victims(hostname, member_id, member_apikey)

        if victim_name not in victims:
            return Verdict.MUMBLE("GET /victims not contains %s" % victim_name)

        c_victim = get_victim(hostname, member_id, member_apikey, victim_name)
        if victim != c_victim:
            return Verdict.MUMBLE("GET /victim return %s, expected %s" % (json.dumps(command), json.dumps(c_command)))

        return Verdict.OK()
    except HTTPException as e:
        print(e)
        return e.verdict
    except Exception as e:
        print(e)


@checker.define_put(vuln_num=1, vuln_rate=1)
def put_flag_into_the_service(request: PutRequest) -> Verdict:
    try:
        hostname = request.hostname
        flag = request.flag
        executor = add_executor(hostname, utils.get_executor_id(), flag)
        executor_id = executor["ExecutorId"]
        executor_apikey = executor["ExecutorApiKey"]

        command_name = utils.get_command_name()
        add_command(hostname, executor_id, executor_apikey, command_name)

        return Verdict.OK(f"{executor_id}:{flag}:{command_name}")

    except HTTPException as e:
        return e.verdict


@checker.define_put(vuln_num=2, vuln_rate=1)
def put_flag_into_the_service2(request: PutRequest) -> Verdict:
    try:
        hostname = request.hostname
        flag = request.flag
        executor_apikey = utils.get_executor_id()
        executor_id = utils.get_executor_id()
        add_executor(hostname, executor_id, executor_apikey)
        victim_name = utils.get_victim_name()
        add_victim(hostname, executor_id, executor_apikey, victim_name, flag)

        return Verdict.OK(f"{executor_id}:{executor_apikey}:{flag}:{victim_name}")
    except HTTPException as e:
        return e.verdict


@checker.define_get(vuln_num=1)
def get_flag_from_the_service(request: GetRequest) -> Verdict:
    try:
        executor_id, secret, command = request.flag_id.strip().split(":")
        hostname = request.hostname
        executor = get_executor(hostname, executor_id, secret)
        c_command = get_command(hostname, executor_id, secret, command)

        if secret in c_command["Admins"]:
            return Verdict.OK()
        else:
            return Verdict.CORRUPT("bad flag")
    except HTTPException as e:
        return e.verdict
    except Exception as e:
        print(f"bad access {e}")
        return Verdict.CORRUPT("can't reach flag")


@checker.define_get(vuln_num=2)
def get_flag_from_the_service2(request: GetRequest) -> Verdict:
    try:
        executor_id, executor_apikey, secret, victim_name = request.flag_id.strip().split(":")
        hostname = request.hostname
        victim = get_victim(hostname, executor_id, executor_apikey, victim_name)
        if victim["InformerName"] == secret:
            return Verdict.OK()
        else:
            return Verdict.CORRUPT("bad flag")
    except HTTPException as e:
        return e.verdict
    except Exception as e:
        print(f"bad access {e}")
        return Verdict.CORRUPT("can't reach flag")


def compare_insensetive(first, second):
    return json.loads(json.dumps(first).lower()) != json.loads(json.dumps(second).lower())


checker.run()
