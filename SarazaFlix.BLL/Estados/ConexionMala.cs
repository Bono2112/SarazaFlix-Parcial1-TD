using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Estados;

/// <summary>Estado con conexión mala: se reproduce en baja definición.</summary>
public class ConexionMala : IEstadoReproduccion
{
    public CalidadReproduccion Calidad => CalidadReproduccion.Baja;

    public string Reproducir(Contenido contenido) => $"Reproduciendo '{contenido.Titulo}' en baja definición";

    public IEstadoReproduccion Siguiente(CalidadConexion medicion) => FabricaEstado.Crear(medicion);
}
