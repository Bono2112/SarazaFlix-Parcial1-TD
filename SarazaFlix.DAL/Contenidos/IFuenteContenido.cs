using SarazaFlix.DomainModel;

namespace SarazaFlix.DAL.Contenidos;

/// <summary>
/// Fuente de contenidos. La implementan tanto el servidor remoto como los proxies de la capa
/// de negocio: por eso la pantalla de reproducción no distingue de dónde viene el contenido.
/// </summary>
public interface IFuenteContenido
{
    /// <summary>Devuelve el contenido o null si no existe / no se puede servir.</summary>
    Contenido? Obtener(string idContenido);
}
