using SarazaFlix.DomainModel;

namespace SarazaFlix.DAL.Contenidos;

/// <summary>
/// Servidor remoto donde están alojados los contenidos.
/// </summary>
public class ServidorRemotoContenido : IFuenteContenido
{
    private readonly Dictionary<string, Contenido> _contenidos;

    public ServidorRemotoContenido(IEnumerable<Contenido> catalogo)
        => _contenidos = catalogo.ToDictionary(c => c.Id);

    /// <summary>Catálogo disponible en el servidor (título y plan mínimo de cada contenido).</summary>
    public IReadOnlyCollection<Contenido> Catalogo => _contenidos.Values;

    /// <summary>Cantidad de descargas realizadas. Sirve para verificar que el dispositivo evita redescargar.</summary>
    public int Descargas { get; private set; }

    public Contenido? Obtener(string idContenido)
    {
        if (!_contenidos.TryGetValue(idContenido, out var contenido))
            return null;

        Descargas++; // simula la descarga desde el servidor remoto
        return contenido;
    }
}
