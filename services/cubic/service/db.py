import redis

from cube.cube import Cube

EXPIRE_TIME = 20*60*1000
CUBES_PREFIX = '/cubes/'
KEY_PREFIX = '/key/'
CIPHERCUBES_PREFIX = '/ciphercubes/'

class DB:
    def __init__(self):
        self.addr = 'db'
        self.db = None

    def _get_db(self):
        if self.db is None:
            self.db = redis.Redis(host=self.addr)
        return self.db


    def get_users(self):
        db = self._get_db()
        return [x.decode().replace(CUBES_PREFIX, '', 1) for x in db.keys(f'{CUBES_PREFIX}*')]

    def check_user(self, login):
        db = self._get_db()
        return db.exists(f'{CUBES_PREFIX}{login}')


    def set_user_data(self, login, pub_cube, priv_key, info):
        data = {
            'pub_cube': pub_cube.as_str(),
            'priv_key': priv_key,
            'info': info
        }
        db = self._get_db()
        db.hmset(f'{CUBES_PREFIX}{login}', data)
        db.pexpire(f'{CUBES_PREFIX}{login}', EXPIRE_TIME)

    def get_user_data(self, login):
        db = self._get_db()
        data = db.hgetall(f'{CUBES_PREFIX}{login}')

        pub_cube = Cube.from_str(data[b'pub_cube'].decode())
        priv_key = int(data[b'priv_key'].decode())
        info = data[b'info'].decode()
        if data:
            return pub_cube, priv_key, info


    def check_ciphercube(self, login):
        db = self._get_db()
        return db.exists(f'{CIPHERCUBES_PREFIX}{login}')

    def set_ciphercube(self, login, cubes):
        db = self._get_db()
        db.set(f'{CIPHERCUBES_PREFIX}{login}', cubes)
        db.pexpire(f'{CIPHERCUBES_PREFIX}{login}', EXPIRE_TIME)

    def get_ciphercube(self, login):
        db = self._get_db()
        return db.get(f'{CIPHERCUBES_PREFIX}{login}')
