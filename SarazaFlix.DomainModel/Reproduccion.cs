namespace SarazaFlix.DomainModel;

/// <summary>
/// Registro de una reproducción: es lo que se persiste de cada una.
/// </summary>
public class Reproduccion
{
    private readonly List<CalidadReproduccion> _calidadesUsadas = new();

    public Reproduccion(Usuario usuario, Contenido contenido)
    {
        Usuario = usuario;
        Contenido = contenido;
    }

    public Usuario Usuario { get; }

    public Contenido Contenido { get; private set; }

    /// <summary>Calidad inicial y cada cambio de calidad hecho en plena reproducción.</summary>
    public IReadOnlyList<CalidadReproduccion> CalidadesUsadas => _calidadesUsadas;

    /// <summary>True si el contenido se sirvió desde el dispositivo en vez del servidor remoto.</summary>
    public bool ServidoDesdeDispositivo { get; set; }

    /// <summary>Contenidos que el usuario intentó ver y su plan no incluye.</summary>
    public List<string> IntentosRechazados { get; } = new();

    /// <summary>
    /// Registra la calidad usada. Ignora repeticiones consecutivas para que la secuencia
    /// ("4K -> Baja") refleje los cambios reales y no cada medición.
    /// </summary>
    public void RegistrarCalidad(CalidadReproduccion calidad)
    {
        if (_calidadesUsadas.Count == 0 || _calidadesUsadas[^1] != calidad)
            _calidadesUsadas.Add(calidad);
    }

    /// <summary>El usuario pasó a otro contenido durante la reproducción.</summary>
    public void CambiarContenido(Contenido contenido) => Contenido = contenido;
}
