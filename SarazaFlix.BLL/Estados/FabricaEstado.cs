using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Estados;

/// <summary>
/// PATRÓN FACTORY METHOD: arma el estado inicial a partir de la medición de conexión.
/// La tabla conexión -> estado queda en un solo lugar y los estados no conocen las clases concretas
/// de los otros (sólo piden la transición acá).
/// </summary>
public static class FabricaEstado
{
    public static IEstadoReproduccion Crear(CalidadConexion conexion) => conexion switch
    {
        CalidadConexion.Buena => new ConexionBuena(),
        CalidadConexion.Media => new ConexionMedia(),
        _ => new ConexionMala()
    };
}
