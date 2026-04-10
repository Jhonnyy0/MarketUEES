namespace MarketUees.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Fullname => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;

        // Perfil embebido (del modelo MongoDB)
        public UsuarioPerfil Perfil { get; set; } = new();

        // Referencias a productos listados por este vendedor
        public List<string> ProductosListados { get; set; } = new();
    }

    public class UsuarioPerfil
    {
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public Direccion Direccion { get; set; } = new();
    }

    public class Direccion
    {
        public string Calle { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
    }
}
