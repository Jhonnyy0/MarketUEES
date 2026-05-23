# 📦 Implementación de Apache Cassandra en MarketUEES API

## Índice
1. [¿Por qué Cassandra?](#1-por-qué-cassandra)
2. [Arquitectura general](#2-arquitectura-general)
3. [Estructura de archivos](#3-estructura-de-archivos)
4. [Capa de Dominio](#4-capa-de-dominio)
5. [Capa de Infraestructura](#5-capa-de-infraestructura)
6. [Capa de Aplicación](#6-capa-de-aplicación)
7. [Capa de API](#7-capa-de-api)
8. [Registro de dependencias (DI)](#8-registro-de-dependencias-di)
9. [Configuración](#9-configuración)
10. [Tablas en Cassandra](#10-tablas-en-cassandra)
11. [Flujo completo de una petición](#11-flujo-completo-de-una-petición)
12. [Cómo probarlo](#12-cómo-probarlo)
13. [Coexistencia con MongoDB](#13-coexistencia-con-mongodb)

---

## 1. ¿Por qué Cassandra?

En MarketUEES se generan dos tipos de datos con **altísimo volumen de escritura**:

| Dato | Descripción | Frecuencia |
|------|-------------|------------|
| **Vistas de productos** | Cada vez que un usuario ve un producto | Muy alta |
| **Actividad de usuarios** | Acciones como "vista", "like", "compartido" | Muy alta |

### Ventajas de Cassandra para este caso

- ✅ **Escritura extremadamente rápida**: Cassandra está optimizada para insertar millones de registros sin degradar el rendimiento.
- ✅ **No bloquea otras operaciones**: Al ser una base de datos distribuida con escritura asíncrona, no frena a MongoDB.
- ✅ **Escalabilidad horizontal**: Si el sistema crece, se agregan más nodos sin cambiar el código.
- ✅ **Separación de responsabilidades**: MongoDB maneja la lógica de negocio (usuarios, productos, compras). Cassandra maneja el registro de eventos masivos.

---

## 2. Arquitectura general

El proyecto sigue una **arquitectura en capas limpia (Clean Architecture)**:

```
┌─────────────────────────────────────────────┐
│                   API Layer                  │  ← Controladores HTTP
│  VistaContenidoController                   │
│  ActividadUsuarioController                 │
├─────────────────────────────────────────────┤
│              Application Layer              │  ← Lógica de negocio
│  VistaContenidoService                      │
│  ActividadUsuarioService                    │
├─────────────────────────────────────────────┤
│               Domain Layer                  │  ← Contratos e Interfaces
│  VistaContenido (Entidad)                   │
│  ActividadUsuario (Entidad)                 │
│  IVistaContenidoRepository (Interface)      │
│  IActividadUsuarioRepository (Interface)    │
├─────────────────────────────────────────────┤
│            Infrastructure Layer             │  ← Implementación técnica
│  CassandraContext                           │
│  VistaContenidoRepository                  │
│  ActividadUsuarioRepository                │
└─────────────────────────────────────────────┘
					  │
					  ▼
		  ┌───────────────────────┐
		  │  Apache Cassandra     │
		  │  (Docker :9042)       │
		  │  keyspace: marketuees │
		  └───────────────────────┘
```

---

## 3. Estructura de archivos

```
MarketUees/
│
├── MarketUees.Domain/
│   ├── Entities/
│   │   ├── VistaContenido.cs           ← Entidad de vista
│   │   └── ActividadUsuario.cs         ← Entidad de actividad
│   └── Interfaces/
│       └── Repositories/
│           ├── IVistaContenidoRepository.cs
│           └── IActividadUsuarioRepository.cs
│
├── MarketUees.Infrastructure/
│   ├── Persistence/
│   │   └── Cassandra/
│   │       ├── CassandraContext.cs              ← Conexión y creación de tablas
│   │       ├── VistaContenidoRepository.cs      ← Implementación de consultas
│   │       └── ActividadUsuarioRepository.cs    ← Implementación de consultas
│   └── CassandraServiceExtensions.cs            ← Registro de dependencias
│
├── MarketUees.Application/
│   └── Services/
│       ├── VistaContenidoService.cs     ← Lógica de negocio para vistas
│       └── ActividadUsuarioService.cs   ← Lógica de negocio para actividad
│
└── MarketUees.API/
	├── Controllers/
	│   ├── VistaContenidoController.cs   ← Endpoints HTTP
	│   └── ActividadUsuarioController.cs ← Endpoints HTTP
	├── Program.cs                         ← Registro AddCassandra()
	└── appsettings.json                   ← Configuración ContactPoint/Port
```

---

## 4. Capa de Dominio

### 4.1 Entidades

Las entidades representan los datos tal como se almacenan en Cassandra. **No heredan de ninguna clase base** porque Cassandra no usa ORM.

#### `VistaContenido.cs`
```csharp
public class VistaContenido
{
	public Guid   UsuarioId   { get; set; }   // Partition Key en Cassandra
	public string ContenidoId { get; set; }   // ID del producto (ObjectId de MongoDB)
	public DateTimeOffset FechaVista { get; set; } // Clustering Key (orden DESC)
}
```

> **UsuarioId** es `Guid` porque viene del sistema de identidad (ASP.NET Identity sobre MongoDB).
> **ContenidoId** es `string` para aceptar el ObjectId de MongoDB (`6a120da36e3aea23a2f022a6`).

#### `ActividadUsuario.cs`
```csharp
public class ActividadUsuario
{
	public Guid   UsuarioId      { get; set; }
	public string TipoActividad  { get; set; }  // "vista", "like", "compartido"
	public DateTimeOffset FechaActividad { get; set; }
	public string ContenidoId    { get; set; }
}
```

---

### 4.2 Interfaces de Repositorio

Las interfaces definen **el contrato** que debe cumplir cualquier implementación. La capa de Dominio **no sabe** que existe Cassandra — solo sabe que hay un repositorio con estos métodos.

#### `IVistaContenidoRepository.cs`
```csharp
public interface IVistaContenidoRepository
{
	Task RegistrarVistaAsync(VistaContenido vista);
	Task<IEnumerable<VistaContenido>> ObtenerPorUsuarioAsync(Guid usuarioId);
}
```

#### `IActividadUsuarioRepository.cs`
```csharp
public interface IActividadUsuarioRepository
{
	Task RegistrarActividadAsync(ActividadUsuario actividad);
	Task<IEnumerable<ActividadUsuario>> ObtenerPorUsuarioAsync(Guid usuarioId);
}
```

> 🔑 **Principio clave**: Si mañana cambias Cassandra por Redis o PostgreSQL, solo cambias la implementación en Infrastructure. El resto del código no se toca.

---

## 5. Capa de Infraestructura

### 5.1 CassandraContext

Es la clase central que **gestiona la conexión** al clúster de Cassandra y **crea las tablas automáticamente** al arrancar la API.

```csharp
public class CassandraContext : IDisposable
{
	private readonly ICluster _cluster;
	public ISession Session { get; }
	private const string Keyspace = "marketuees";

	public CassandraContext(string contactPoint, int port = 9042)
	{
		// 1. Construir el cluster apuntando al servidor
		_cluster = Cluster.Builder()
			.AddContactPoint(contactPoint)
			.WithPort(port)
			.Build();

		// 2. Conectar sin keyspace para poder crearlo
		using var tempSession = _cluster.Connect();
		tempSession.Execute($@"
			CREATE KEYSPACE IF NOT EXISTS {Keyspace}
			WITH replication = {{'class': 'SimpleStrategy', 'replication_factor': 1}}");

		// 3. Conectar ya al keyspace y crear las tablas
		Session = _cluster.Connect(Keyspace);
		InicializarSchema();
	}
```

**¿Qué hace `InicializarSchema()`?**

```csharp
private void InicializarSchema()
{
	// Tabla 1: Registro de vistas
	Session.Execute(@"
		CREATE TABLE IF NOT EXISTS vistas_contenido (
			usuario_id   uuid,
			fecha_vista  timestamp,
			contenido_id text,
			PRIMARY KEY (usuario_id, fecha_vista)
		) WITH CLUSTERING ORDER BY (fecha_vista DESC)");

	// Tabla 2: Actividad del usuario
	Session.Execute(@"
		CREATE TABLE IF NOT EXISTS actividad_usuario (
			usuario_id       uuid,
			fecha_actividad  timestamp,
			tipo_actividad   text,
			contenido_id     text,
			PRIMARY KEY (usuario_id, fecha_actividad)
		) WITH CLUSTERING ORDER BY (fecha_actividad DESC)");
}
```

> ⚙️ **`IF NOT EXISTS`**: Las tablas solo se crean si no existen. Si la API se reinicia, no falla ni duplica tablas.
>
> ⚙️ **`CLUSTERING ORDER BY DESC`**: Los registros más recientes aparecen primero al consultar.

---

### 5.2 Diseño de las Tablas (PRIMARY KEY explicado)

```
PRIMARY KEY (usuario_id, fecha_vista)
	 │              │
	 │              └─ Clustering Key: ordena los datos dentro de la partición
	 └──────────────── Partition Key: determina en qué nodo se guarda el dato
```

Esto significa que:
- Todos los registros de un mismo usuario se guardan **en el mismo nodo** → consultas muy rápidas.
- Dentro de ese usuario, los registros se ordenan **por fecha descendente** → el más reciente primero.

---

### 5.3 VistaContenidoRepository

Implementa `IVistaContenidoRepository` usando **Prepared Statements** de Cassandra.

```csharp
public async Task RegistrarVistaAsync(VistaContenido vista)
{
	// PrepareAsync compila la query una vez y la reutiliza → más eficiente
	var statement = await _session.PrepareAsync(
		"INSERT INTO vistas_contenido (usuario_id, fecha_vista, contenido_id) VALUES (?, ?, ?)");

	var bound = statement.Bind(
		vista.UsuarioId,
		vista.FechaVista.UtcDateTime,
		vista.ContenidoId);

	await _session.ExecuteAsync(bound);
}
```

> 🔑 **Prepared Statements**: Cassandra compila la query la primera vez. En las siguientes ejecuciones solo envía los parámetros. Esto es **mucho más rápido** que enviar la query completa cada vez.

```csharp
public async Task<IEnumerable<VistaContenido>> ObtenerPorUsuarioAsync(Guid usuarioId)
{
	var statement = await _session.PrepareAsync(
		"SELECT usuario_id, fecha_vista, contenido_id FROM vistas_contenido WHERE usuario_id = ?");

	var rows = await _session.ExecuteAsync(statement.Bind(usuarioId));

	// Mapear cada fila de Cassandra a la entidad del dominio
	return rows.Select(row => new VistaContenido
	{
		UsuarioId   = row.GetValue<Guid>("usuario_id"),
		ContenidoId = row.GetValue<string>("contenido_id") ?? string.Empty,
		FechaVista  = row.GetValue<DateTimeOffset>("fecha_vista")
	});
}
```

---

### 5.4 ActividadUsuarioRepository

Mismo patrón que `VistaContenidoRepository` pero con los campos de actividad:

```csharp
public async Task RegistrarActividadAsync(ActividadUsuario actividad)
{
	var statement = await _session.PrepareAsync(
		"INSERT INTO actividad_usuario (usuario_id, fecha_actividad, tipo_actividad, contenido_id) VALUES (?, ?, ?, ?)");

	var bound = statement.Bind(
		actividad.UsuarioId,
		actividad.FechaActividad.UtcDateTime,
		actividad.TipoActividad,
		actividad.ContenidoId ?? string.Empty);

	await _session.ExecuteAsync(bound);
}
```

---

### 5.5 CassandraServiceExtensions

Clase estática que agrupa el registro de todas las dependencias de Cassandra. Se llama con un método de extensión en `Program.cs`.

```csharp
public static IServiceCollection AddCassandra(
	this IServiceCollection services,
	IConfiguration configuration)
{
	var contactPoint = configuration["Cassandra:ContactPoint"] ?? "localhost";
	var port = int.TryParse(configuration["Cassandra:Port"], out var p) ? p : 9042;

	// Singleton: una sola instancia de conexión durante toda la vida de la app
	services.AddSingleton(_ => new CassandraContext(contactPoint, port));

	// Scoped: una instancia por request HTTP
	services.AddScoped<IVistaContenidoRepository, VistaContenidoRepository>();
	services.AddScoped<IActividadUsuarioRepository, ActividadUsuarioRepository>();

	return services;
}
```

> 🔑 **Singleton para CassandraContext**: La conexión al clúster es costosa de crear. Al ser Singleton, se crea una sola vez y se reutiliza en todas las peticiones.

---

## 6. Capa de Aplicación

Los servicios contienen la **lógica de negocio**. No saben nada de Cassandra, solo usan las interfaces del dominio.

### `VistaContenidoService.cs`

```csharp
public class VistaContenidoService
{
	private readonly IVistaContenidoRepository _repository;

	public VistaContenidoService(IVistaContenidoRepository repository)
	{
		_repository = repository;
	}

	// Crea la entidad con la fecha actual y delega al repositorio
	public Task RegistrarVistaAsync(Guid usuarioId, string contenidoId)
	{
		var vista = new VistaContenido
		{
			UsuarioId   = usuarioId,
			ContenidoId = contenidoId,
			FechaVista  = DateTimeOffset.UtcNow  // ← Fecha automática
		};
		return _repository.RegistrarVistaAsync(vista);
	}

	public Task<IEnumerable<VistaContenido>> ObtenerVistasPorUsuarioAsync(Guid usuarioId)
		=> _repository.ObtenerPorUsuarioAsync(usuarioId);
}
```

### `ActividadUsuarioService.cs`

```csharp
public Task RegistrarActividadAsync(Guid usuarioId, string tipoActividad, string contenidoId)
{
	var actividad = new ActividadUsuario
	{
		UsuarioId      = usuarioId,
		TipoActividad  = tipoActividad,   // "vista", "like", "compartido"
		ContenidoId    = contenidoId,
		FechaActividad = DateTimeOffset.UtcNow
	};
	return _repository.RegistrarActividadAsync(actividad);
}
```

---

## 7. Capa de API

### `VistaContenidoController.cs`

```
POST /api/VistaContenido/{contenidoId}   → Registrar vista
GET  /api/VistaContenido/mis-vistas      → Ver historial de vistas
```

El `contenidoId` es el **ObjectId de MongoDB** del producto que se está viendo.

```csharp
[HttpPost("{contenidoId}")]
public async Task<IActionResult> Registrar(string contenidoId)
{
	// 1. Extraer el ID del usuario del JWT token
	var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
	if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
		return Unauthorized();

	// 2. Registrar en Cassandra
	await _service.RegistrarVistaAsync(usuarioId, contenidoId);
	return NoContent(); // 204 → éxito sin cuerpo
}
```

### `ActividadUsuarioController.cs`

```
POST /api/ActividadUsuario               → Registrar actividad
GET  /api/ActividadUsuario/mi-actividad  → Ver historial de actividad
```

```csharp
// DTO del body del request
public record RegistrarActividadRequest(
	[Required] string TipoActividad,  // "vista", "like", "compartido"
	[Required] string ContenidoId);   // ObjectId del producto
```

---

## 8. Registro de dependencias (DI)

En `Program.cs` se registra todo con una sola línea:

```csharp
// ── Cassandra ──────────────────────────────────────
builder.Services.AddCassandra(builder.Configuration);
builder.Services.AddScoped<VistaContenidoService>();
builder.Services.AddScoped<ActividadUsuarioService>();
```

**Cadena de inyección de dependencias completa:**

```
HTTP Request
	│
	▼
VistaContenidoController
	│  recibe inyectado
	▼
VistaContenidoService
	│  recibe inyectado
	▼
IVistaContenidoRepository  ←── resuelto como VistaContenidoRepository
	│  recibe inyectado
	▼
CassandraContext  (Singleton — misma instancia siempre)
	│
	▼
ISession (conexión activa al clúster)
```

---

## 9. Configuración

En `appsettings.json`:

```json
"Cassandra": {
  "ContactPoint": "localhost",
  "Port": "9042"
}
```

| Propiedad | Descripción | Valor en desarrollo |
|-----------|-------------|---------------------|
| `ContactPoint` | IP o hostname del servidor Cassandra | `localhost` (Docker) |
| `Port` | Puerto nativo de Cassandra | `9042` (estándar) |

> Para producción, cambia `ContactPoint` a la IP real del servidor Cassandra.

---

## 10. Tablas en Cassandra

### `vistas_contenido`

| Columna | Tipo | Rol |
|---------|------|-----|
| `usuario_id` | uuid | **Partition Key** |
| `fecha_vista` | timestamp | **Clustering Key** (DESC) |
| `contenido_id` | text | Dato adicional |

### `actividad_usuario`

| Columna | Tipo | Rol |
|---------|------|-----|
| `usuario_id` | uuid | **Partition Key** |
| `fecha_actividad` | timestamp | **Clustering Key** (DESC) |
| `tipo_actividad` | text | "vista", "like", "compartido" |
| `contenido_id` | text | ID del producto |

### Verificar tablas desde Docker

```bash
# Entrar al contenedor
docker exec -it cassandra cqlsh

# Dentro de cqlsh:
USE marketuees;
DESCRIBE TABLES;
SELECT * FROM vistas_contenido;
SELECT * FROM actividad_usuario;
```

---

## 11. Flujo completo de una petición

Ejemplo: **Usuario ve un producto**

```
1. Frontend hace:
   POST /api/VistaContenido/6a120da36e3aea23a2f022a6
   Headers: Authorization: Bearer eyJhbGci...

2. VistaContenidoController.Registrar()
   → Extrae usuarioId del JWT: "a1b2c3d4-..."
   → Llama: _service.RegistrarVistaAsync(usuarioId, "6a120da36e3aea23a2f022a6")

3. VistaContenidoService.RegistrarVistaAsync()
   → Crea entidad VistaContenido con FechaVista = UtcNow
   → Llama: _repository.RegistrarVistaAsync(vista)

4. VistaContenidoRepository.RegistrarVistaAsync()
   → Prepared Statement:
	 INSERT INTO vistas_contenido (usuario_id, fecha_vista, contenido_id)
	 VALUES (a1b2c3d4-..., 2026-05-23 10:30:00, '6a120da36e3aea23a2f022a6')

5. Cassandra escribe el registro en el nodo correspondiente

6. API responde: 204 No Content
```

---

## 12. Cómo probarlo

### Requisitos
- Docker Desktop corriendo con el contenedor `cassandra` iniciado
- API corriendo en Visual Studio (F5)
- MongoDB corriendo en `localhost:27017`

### Paso 1 — Registrar y hacer login
```http
POST http://localhost:5000/api/account/register
Content-Type: application/json

{
  "firstName": "Juan",
  "lastName": "Pérez",
  "phone": "0991234567",
  "email": "juan@test.com",
  "password": "Test1234!"
}
```

```http
POST http://localhost:5000/api/account/login
Content-Type: application/json

{
  "email": "juan@test.com",
  "password": "Test1234!",
  "rememberMe": false
}
```

### Paso 2 — Registrar vista (Cassandra)
```http
POST http://localhost:5000/api/VistaContenido/6a120da36e3aea23a2f022a6
Authorization: Bearer {token_del_login}
```

### Paso 3 — Registrar actividad (Cassandra)
```http
POST http://localhost:5000/api/ActividadUsuario
Authorization: Bearer {token_del_login}
Content-Type: application/json

{
  "tipoActividad": "like",
  "contenidoId": "6a120da36e3aea23a2f022a6"
}
```

### Paso 4 — Consultar historial
```http
GET http://localhost:5000/api/VistaContenido/mis-vistas
Authorization: Bearer {token_del_login}

GET http://localhost:5000/api/ActividadUsuario/mi-actividad
Authorization: Bearer {token_del_login}
```

---

## 13. Coexistencia con MongoDB

Ambas bases de datos corren **independientemente** y sin interferir:

| | MongoDB | Cassandra |
|---|---------|-----------|
| **Puerto** | 27017 | 9042 |
| **Usa** | Usuarios, Productos, Compras, Reseñas | Vistas, Actividad |
| **Fortaleza** | Consultas flexibles, documentos anidados | Escritura masiva y rápida |
| **Visible en** | MongoDB Compass | cqlsh / Docker exec |
| **Ciclo de vida** | Datos permanentes del negocio | Registro de eventos |

```
					┌──────────────┐
	 Petición ─────►│  ASP.NET API │
					└──────┬───────┘
						   │
			  ┌────────────┴────────────┐
			  │                         │
			  ▼                         ▼
	┌─────────────────┐      ┌─────────────────────┐
	│    MongoDB      │      │     Cassandra        │
	│  localhost:27017│      │   localhost:9042     │
	│                 │      │                      │
	│  • usuarios     │      │  • vistas_contenido  │
	│  • productos    │      │  • actividad_usuario │
	│  • compras      │      │                      │
	│  • resenas      │      │                      │
	└─────────────────┘      └─────────────────────┘
```
