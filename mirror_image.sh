#!/bin/bash
docker pull $1 \
  && docker tag $1 cr.yandex/crpbppm3hiurm5j1lcfo/$1 \
  && docker push cr.yandex/crpbppm3hiurm5j1lcfo/$1

echo cr.yandex/crpbppm3hiurm5j1lcfo/$1
