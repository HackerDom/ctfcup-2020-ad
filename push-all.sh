#!/bin/bash

for s in services/*; do
    (cd $s && docker-compose build);
done

./push-image.py chessbase qexecute cubic

