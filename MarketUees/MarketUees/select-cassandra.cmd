@echo off
docker exec cassandra-node1 cqlsh -e "DESCRIBE KEYSPACE marketuees" >nul 2>nul
if errorlevel 1 (
  echo Cassandra todavia no tiene el keyspace marketuees.
  echo Primero levanta la API para que cree automaticamente el keyspace y las tablas.
  echo Luego prueba otra vez este comando.
  exit /b 1
)
docker exec cassandra-node1 cqlsh -e "SELECT * FROM marketuees.vistas_contenido LIMIT 5; SELECT * FROM marketuees.actividad_usuario LIMIT 5;"
