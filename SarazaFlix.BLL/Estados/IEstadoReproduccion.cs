using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Estados;

/// <summary>
/// PATRÓN STATE: cada estado sabe en qué calidad se reproduce y hacia qué estado pasar cuando el
/// sistema mide la conexión otra vez. La transición la decide el propio estado, no el contexto.
/// </summary>
public interface IEstadoReproduccion
{
    CalidadReproduccion Calidad { get; }

    string Reproducir(Contenido contenido);

    /// <summary>Transición: devuelve el estado que corresponde a la nueva medición de conexión.</summary>
    IEstadoReproduccion Siguiente(CalidadConexion medicion);
}
