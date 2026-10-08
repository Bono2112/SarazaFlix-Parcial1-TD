using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Estados;

/// <summary>
/// Estado de ahorro de datos: reproduce siempre en baja definición, sin importar la conexión.
/// Mantiene adentro el estado de conexión para poder volver a él cuando el usuario apaga el modo
/// y para seguir actualizándolo con cada medición que llega mientras el ahorro está activo.
/// </summary>
public class AhorroDeDatos : IEstadoReproduccion
{
    public AhorroDeDatos(IEstadoReproduccion estadoDeConexion) => EstadoDeConexion = estadoDeConexion;

    /// <summary>Estado de conexión que quedó debajo del modo ahorro.</summary>
    public IEstadoReproduccion EstadoDeConexion { get; }

    public CalidadReproduccion Calidad => CalidadReproduccion.Baja;

    public string Reproducir(Contenido contenido)
        => $"Reproduciendo '{contenido.Titulo}' en baja definición (modo ahorro de datos activo)";

    /// <summary>
    /// Con el ahorro activo la medición no cambia la calidad, pero sí el estado de conexión que
    /// hay adentro: así, al apagar el modo, se vuelve a la calidad que corresponde en ese momento.
    /// </summary>
    public IEstadoReproduccion Siguiente(CalidadConexion medicion)
        => new AhorroDeDatos(FabricaEstado.Crear(medicion));
}
