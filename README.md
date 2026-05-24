# MarketUEES
Proyecto de catedra - Base de datos no relacionales I

# Descripcion
Proyecto de MarketUees, red social en linea para compra y venta de articulos o servicios dentro de la universidad evangelica, para estudiantes y profesores.

# Base de datos y registros
- Mongodb: Account, Producto, Compra, Reseña
- Cassandra ActividadUsuario y VistaContenido

# MongoDb + Identity
- Account
  POST/api/account/register - Registro de usuario
  POST/api/account/login - Login → JWT

- Producto
  GET/api/producto - Listar con filtros + paginación
  GET/api/producto/{id} - Obtener por ID
  GET/api/producto/categorias - Listar categorías únicas
  GET/api/producto/vendedor/{vendedorId} - Productos de un vendedor
  POST/api/producto - Crear producto → JWT
  PUT/api/producto/{id} - Actualizar producto → JWT
  DELETE/api/producto/{id} - Eliminar (Admin) → JWT

- Compra
  GET/api/compra - Todas las compras (Admin) → JWT
  GET/api/compra/mis-compras - Compras del usuario autenticado → JWT
  GET/api/compra/{id} - Compra por ID → JWT
  POST/api/compra - Crear compra → JWT

- Reseña
  GET/api/resena/producto/{productoId} - Reseñas de un producto
  GET/api/resena/usuario/{usuarioId} - Reseñas de un usuario
  POST/api/resena - Crear reseña → JWT 
  DELETE/api/resena/{id} - Eliminar reseña (Admin)

# Cassandra
- Actividad de usuario
  POST/api/actividadusuario - Registrar actividad (vista, like…) → JWT
  GET/api/actividadusuario/mi-actividad - Historial del usuario autenticado → JWT

- Vista de contenido
  POST/api/vistacontenido/{contenidoId} - Registrar que se vio un contenido → JWT
  GET/api/vistacontenido/mis-vistas - Historial de vistas del usuario → JWT
