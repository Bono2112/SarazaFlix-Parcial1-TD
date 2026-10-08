namespace SarazaFlix.DomainModel;

/// <summary>
/// Usuario de la plataforma con su plan contratado.
/// </summary>
public class Usuario
{
    public Usuario(string id, string nombre, Plan plan)
    {
        Id = id;
        Nombre = nombre;
        Plan = plan;
    }

    public string Id { get; }

    public string Nombre { get; }

    public Plan Plan { get; }

    /// <summary>Se usa en la exportación genérica de listados.</summary>
    public override string ToString() => Nombre;
}
