using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Estados;

/// <summary>Estado con buena conexión: se reproduce en 4K.</summary>
public class ConexionBuena : IEstadoReproduccion
{
    public CalidadReproduccion Calidad => CalidadReproduccion.CuatroK;

    public string Reproducir(Contenido contenido) => $"Reproduciendo '{contenido.Titulo}' en 4K";

    public IEstadoReproduccion Siguiente(CalidadConexion medicion) => FabricaEstado.Crear(medicion);
}
