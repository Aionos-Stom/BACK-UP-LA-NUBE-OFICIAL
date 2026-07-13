#!/bin/bash
set -e

echo "Esperando a SQL Server..."
until /opt/mssql-tools18/bin/sqlcmd -S db -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -C &> /dev/null
do
  sleep 2
done

echo "Comprobando si la base de datos ya existe..."
DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S db -U sa -P "$SA_PASSWORD" -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = 'n'" -h -1 -C | tr -d '[:space:]')

if [ "$DB_EXISTS" = "0" ]; then
  echo "Creando base de datos..."
  /opt/mssql-tools18/bin/sqlcmd -S db -U sa -P "$SA_PASSWORD" -C -Q "CREATE DATABASE n;"

  echo "Ejecutando script de esquema y datos..."
  tail -n +3 /init.sql > /tmp/schema.sql
  /opt/mssql-tools18/bin/sqlcmd -S db -U sa -P "$SA_PASSWORD" -C -d n -i /tmp/schema.sql

  echo "Base de datos inicializada correctamente."
else
  echo "La base de datos 'n' ya existe, se omite la inicializacion."
fi
