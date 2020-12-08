#!/usr/bin/python

from os import system
import sys

images = sys.argv[1:]

for image in images:
    for i in range(101, 111):
        new_name = '10.118.{}.10:5000/{}'.format(i, image)
        system('docker tag {} {}'.format(image, new_name))
        system('docker push {}'.format(new_name))
