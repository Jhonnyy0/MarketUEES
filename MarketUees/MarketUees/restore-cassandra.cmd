@echo off
setlocal EnableExtensions

if "%~1"=="" (
  echo Uso: restore-cassandra.cmd Backup_Manual_MarketUEES\backup_manual_YYYYMMDD_HHMMSS
  echo Tambien puedes usar una carpeta de Backups_Minio_MarketUEES.
  exit /b 1
)

set SRC=%~f1

if not exist "%SRC%\vistas_contenido.csv" (
  echo No existe "%SRC%\vistas_contenido.csv"
  exit /b 1
)

if not exist "%SRC%\actividad_usuario.csv" (
  echo No existe "%SRC%\actividad_usuario.csv"
  exit /b 1
)

docker exec cassandra-node1 rm -rf /tmp/restore_marketuees
docker exec cassandra-node1 mkdir -p /tmp/restore_marketuees
docker cp "%SRC%\." cassandra-node1:/tmp/restore_marketuees
docker exec cassandra-node1 cqlsh -e "COPY marketuees.vistas_contenido FROM '/tmp/restore_marketuees/vistas_contenido.csv' WITH HEADER = TRUE;"
docker exec cassandra-node1 cqlsh -e "COPY marketuees.actividad_usuario FROM '/tmp/restore_marketuees/actividad_usuario.csv' WITH HEADER = TRUE;"
docker exec cassandra-node1 rm -rf /tmp/restore_marketuees

echo.
echo Restore terminado desde: %SRC%
echo.
docker exec cassandra-node1 cqlsh -e "SELECT * FROM marketuees.vistas_contenido LIMIT 5; SELECT * FROM marketuees.actividad_usuario LIMIT 5;"
