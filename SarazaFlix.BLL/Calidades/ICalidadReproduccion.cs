using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Calidades;

/// <summary>
/// PATRÓN STRATEGY: familia de formas de reproducir un contenido. Cada estrategia encapsula
/// una calidad y el reproductor las intercambia en plena reproducción sin saber cuál usa.
/// </summary>
public interface ICalidadReproduccion
{
    CalidadReproduccion Calidad { get; }

    string Reproducir(Contenido contenido);
}
