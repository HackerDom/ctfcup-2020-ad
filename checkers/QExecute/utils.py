import random
import uuid
import string
from random import randrange

UserAgents = None
Names = None


def get_user_agent():
    global UserAgents
    if UserAgents is None:
        with open('user-agents') as fin:
            UserAgents = [line.strip() for line in fin]
    return random.choice(UserAgents)


def get_name():
    global Names
    if Names is None:
        with open('user-agents') as fin:
            Names = [line.strip() for line in fin]
    return str(uuid.uuid4())
        #random.choice(Names)



def get_executor_id():
    return get_random_string(randrange(5, 30))

def get_command_name():
    return get_random_string(randrange(5, 30))

def get_victim_name():
    return get_random_string(randrange(5, 10))

def get_token():
    return str(uuid.uuid4())

def get_port():
    return ''

def get_random_string(N):
    return ''.join(random.choice(string.ascii_uppercase + string.digits) for _ in range(N))


def get_telemetry():
    return {
        "modelSeries": str(uuid.uuid4()),
        "revision": "1",
        "BodyFamilyUUID": str(uuid.uuid4()),
        "referenceValues": {
            "heart tem.": 50.0,
            "oil pressure": 30.0,
        }
    }


def get_body_model():
    return {
        "modelSeries": str(uuid.uuid4()),
        "revision": "1",
        "BodyFamilyUUID": str(uuid.uuid4()),
        "referenceValues": {
            "heart tem.": 50.0,
            "oil pressure": 30.0,
        }
    }
