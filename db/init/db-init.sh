#!/bin/sh
set -e

echo "Waiting for MySQL (TCP)..."
until mysqladmin --protocol=TCP -h mysql -uroot -proot ping --silent; do
  sleep 1
done

mysql --protocol=TCP -h mysql -uroot -proot photosi < /init/02-fk.sql
mysql --protocol=TCP -h mysql -uroot -proot photosi < /init/03-seed.sql

echo "DB init done."