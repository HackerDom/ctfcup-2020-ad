#!/bin/bash

SERVICE_DIR="/srv/chessbase/"

mkdir -p $SERVICE_DIR
cat > $SERVICE_DIR/docker-compose.yaml <<EOF
version: '3'
services:
  chessbase:
    image: ${team_registry}/chessbase:latest
    ports:
     - "8284:8284"
    volumes:
      - ./storage:/app/storage
    restart: unless-stopped

EOF

systemctl enable ctfcup@chessbase
systemctl start ctfcup@chessbase

