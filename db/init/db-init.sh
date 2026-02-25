#!/bin/sh
set -eu

echo "Waiting for MySQL (TCP)..."
until mysqladmin --protocol=TCP --host=mysql --port=3306 -uroot -proot ping --silent; do
  sleep 1
done

echo "Running FK + seed..."
mysql --protocol=TCP --host=mysql --port=3306 -uroot -proot photosi < /init/02-fk.sql
mysql --protocol=TCP --host=mysql --port=3306 -uroot -proot photosi < /init/03-seed.sql

echo "DB init done."