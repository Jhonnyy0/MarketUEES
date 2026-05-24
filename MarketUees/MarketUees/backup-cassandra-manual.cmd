@echo off
setlocal EnableExtensions EnableDelayedExpansion

set LOCK_DIR=Backup_Lock_MarketUEES
set LOCK_FILE=%LOCK_DIR%\backup.lock
mkdir "%LOCK_DIR%" 2>nul

if exist "%LOCK_FILE%" (
  echo Ya hay un backup en proceso. Espera a que termine antes de crear otro.
  exit /b 1
)

echo backup_manual > "%LOCK_FILE%"

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set TAG=backup_manual_%%i
set DEST=Backup_Manual_MarketUEES\%TAG%

mkdir "%DEST%" 2>nul

docker exec cassandra-node1 mkdir -p /tmp/%TAG%
docker exec cassandra-node1 cqlsh -e "COPY marketuees.vistas_contenido TO '/tmp/%TAG%/vistas_contenido.csv' WITH HEADER = TRUE;"
if errorlevel 1 goto error
docker exec cassandra-node1 cqlsh -e "COPY marketuees.actividad_usuario TO '/tmp/%TAG%/actividad_usuario.csv' WITH HEADER = TRUE;"
if errorlevel 1 goto error
docker cp cassandra-node1:/tmp/%TAG%/. "%DEST%"
if errorlevel 1 goto error
docker exec cassandra-node1 rm -rf /tmp/%TAG%
del "%LOCK_FILE%" 2>nul

echo.
echo Backup manual creado en: %DEST%
echo.
docker exec cassandra-node1 cqlsh -e "SELECT * FROM marketuees.vistas_contenido LIMIT 5; SELECT * FROM marketuees.actividad_usuario LIMIT 5;"
exit /b 0

:error
docker exec cassandra-node1 rm -rf /tmp/%TAG% >nul 2>nul
del "%LOCK_FILE%" 2>nul
echo Error creando backup manual. No se borraron datos de Cassandra.
exit /b 1
