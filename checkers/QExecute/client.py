from sys import stderr
import requests
import utils
import json

from gornilo import Verdict


def close(code, public="", private=""):
    if public:
        print(public)
    if private:
        print(private, file=stderr)
    print('Exit with code %d' % code, file=stderr)
    exit(code)


def add_executor(addr, id, apikey):
    url = 'http://%s/executor' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {"ExecutorId": id,
           'ExecutorApiKey': apikey
           }
    r = ensure_success(lambda: requests.put(url, headers=headers, data=json.dumps(payload), verify=False))
    return payload


def get_executor(addr, id, apikey):
    url = 'http://%s/executor' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey}
    r = ensure_success(lambda: requests.get(url, headers=headers, data=json.dumps(payload), verify=False))

    return json.loads(r.content.decode("UTF-8"))


def add_command(addr, id, apikey, command_name):
    url = 'http://%s/command' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey,
        'CommandName': command_name
    }
    r = ensure_success(lambda: requests.put(url, headers=headers, data=json.dumps(payload), verify=False))


def get_command(addr, id, apikey, command_name):
    url = 'http://%s/command' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey,
        'CommandName': command_name
    }
    r = ensure_success(lambda: requests.get(url, headers=headers, data=json.dumps(payload), verify=False))

    return json.loads(r.content.decode("UTF-8"))


def get_commands(addr):
    url = 'http://%s/commandList' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    r = ensure_success(lambda: requests.get(url, headers=headers, verify=False))

    return json.loads(r.content.decode("UTF-8"))


def add_command_admin(addr, id, apikey, command_name, admin):
    url = 'http://%s/addAdmin' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey,
        'CommandName': command_name,
        'AdminApiKey': admin
    }
    r = ensure_success(lambda: requests.put(url, headers=headers, data=json.dumps(payload), verify=False))


def add_to_command(addr, id, apikey, command_name):
    url = 'http://%s/joinToCommand' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey,
        'CommandName': command_name
    }
    r = ensure_success(lambda: requests.put(url, headers=headers, data=json.dumps(payload), verify=False))

    return json.loads(r.content.decode("UTF-8"))


def add_victim(addr, id, apikey, name, informer):
    url = 'http://%s/victim' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey,
        'VictimName': name,
        'InformerName': informer
    }
    r = ensure_success(lambda: requests.put(url, headers=headers, data=json.dumps(payload), verify=False))


def get_victim(addr, id, apikey, name):
    url = 'http://%s/victim' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey,
        'VictimName': name
    }
    r = ensure_success(lambda: requests.get(url, headers=headers, data=json.dumps(payload), verify=False))

    return json.loads(r.content.decode("UTF-8"))


def get_victims(addr, id, apikey):
    url = 'http://%s/victims' % addr
    headers = {'User-Agent': utils.get_user_agent()}
    payload = {
        'ExecutorId': id,
        'ExecutorApiKey': apikey}
    r = ensure_success(lambda: requests.get(url, headers=headers, data=json.dumps(payload), verify=False))

    return json.loads(r.content.decode("UTF-8"))


def ensure_success(request):
    try:
        r = request()
    except Exception as e:
        raise HTTPException(Verdict.DOWN("HTTP error"))
    if r.status_code != 200:
        raise HTTPException(Verdict.MUMBLE("Invalid status code: %s %s" % (r.url, r.status_code)))
    return r


class HTTPException(Exception):
    def __init__(self, verdict=None):
        self.verdict = verdict  # you could add more args

    def __str__(self):
        return str(self.verdict)
