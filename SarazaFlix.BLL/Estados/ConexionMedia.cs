using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Estados;

/// <summary>Estado con conexión media: se reproduce en HD.</summary>
public class ConexionMedia : IEstadoReproduccion
{
    public CalidadReproduccion Calidad => CalidadReproduccion.HD;

    public string Reproducir(Contenido contenido) => $"Reproduciendo '{contenido.Titulo}' en HD";

    public IEstadoReproduccion Siguiente(CalidadConexion medicion) => FabricaEstado.Crear(medicion);
}
