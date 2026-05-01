#!/bin/bash

# Backup MongoDB database
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_DIR="./backup"

docker exec skylink_mongodb mongodump --username admin --password admin123 --authenticationDatabase admin --db AirlineDB --out /backup/dump_$TIMESTAMP

# Copy backup from container to host
docker cp skylink_mongodb:/backup/dump_$TIMESTAMP $BACKUP_DIR/

echo "Backup created at $BACKUP_DIR/dump_$TIMESTAMP"