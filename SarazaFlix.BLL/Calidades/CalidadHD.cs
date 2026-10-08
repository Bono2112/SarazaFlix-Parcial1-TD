using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Calidades;

/// <summary>Estrategia concreta: conexión media se reproduce en HD.</summary>
public class CalidadHD : ICalidadReproduccion
{
    public CalidadReproduccion Calidad => CalidadReproduccion.HD;

    public string Reproducir(Contenido contenido) => $"Reproduciendo '{contenido.Titulo}' en HD";
}
