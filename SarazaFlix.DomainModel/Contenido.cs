namespace SarazaFlix.DomainModel;

/// <summary>
/// Contenido del catálogo. El catálogo vive en el servidor remoto.
/// </summary>
public class Contenido
{
    public Contenido(string id, string titulo, Plan planMinimo)
    {
        Id = id;
        Titulo = titulo;
        PlanMinimo = planMinimo;
    }

    public string Id { get; }

    public string Titulo { get; }

    /// <summary>Plan mínimo que necesita el usuario para poder verlo.</summary>
    public Plan PlanMinimo { get; }

    /// <summary>
    /// Regla del enunciado: el contenido se puede ver sólo si está incluido en el plan del usuario.
    /// </summary>
    public bool EstaIncluidoEn(Plan plan) => plan >= PlanMinimo;

    /// <summary>Se usa en la exportación genérica de listados.</summary>
    public override string ToString() => Titulo;
}
