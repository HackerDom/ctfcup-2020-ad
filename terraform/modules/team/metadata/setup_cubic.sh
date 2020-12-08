#!/bin/bash

SERVICE_DIR="/srv/cubic/"

mkdir -p $SERVICE_DIR
cat > $SERVICE_DIR/docker-compose.yaml <<EOF
version: '2.2'
services:
  cubic:
    image: ${team_registry}/cubic:latest
    stop_grace_period: 5s
    ulimits:
      nofile:
        soft: 12000
        hard: 12000
    pids_limit: 100
    restart: always
    ports:
      - 4551:31337
    links:
      - db

  db:
    image: redis:alpine
    restart: always
    pids_limit: 100

EOF

systemctl enable ctfcup@cubic
systemctl start ctfcup@cubic

