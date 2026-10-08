using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Calidades;

/// <summary>Estrategia concreta: buena conexión se reproduce en 4K.</summary>
public class Calidad4K : ICalidadReproduccion
{
    public CalidadReproduccion Calidad => CalidadReproduccion.CuatroK;

    public string Reproducir(Contenido contenido) => $"Reproduciendo '{contenido.Titulo}' en 4K";
}
