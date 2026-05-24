@echo off
setlocal EnableExtensions

set LOCK_FILE=Backup_Lock_MarketUEES\backup.lock

if exist "%LOCK_FILE%" (
  echo No se pueden borrar datos porque hay un backup en proceso.
  echo Espera a que termine el backup y vuelve a ejecutar este comando.
  exit /b 1
)

docker exec cassandra-node1 cqlsh -e "TRUNCATE marketuees.actividad_usuario;"
if errorlevel 1 exit /b 1
docker exec cassandra-node1 cqlsh -e "TRUNCATE marketuees.vistas_contenido;"
if errorlevel 1 exit /b 1

echo Datos eliminados para simulacion de perdida.
docker exec cassandra-node1 cqlsh -e "SELECT * FROM marketuees.vistas_contenido LIMIT 5; SELECT * FROM marketuees.actividad_usuario LIMIT 5;"
