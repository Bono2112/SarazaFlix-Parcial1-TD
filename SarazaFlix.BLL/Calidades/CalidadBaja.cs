using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Calidades;

/// <summary>Estrategia concreta: conexión mala se reproduce en baja definición.</summary>
public class CalidadBaja : ICalidadReproduccion
{
    public CalidadReproduccion Calidad => CalidadReproduccion.Baja;

    public string Reproducir(Contenido contenido) => $"Reproduciendo '{contenido.Titulo}' en baja definición";
}
