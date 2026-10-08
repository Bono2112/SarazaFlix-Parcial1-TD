using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Calidades;

/// <summary>
/// PATRÓN FACTORY METHOD: crea la estrategia de calidad a partir de la medición de conexión y del
/// modo ahorro. La tabla conexión -> calidad queda en un solo lugar y el reproductor nunca
/// instancia las estrategias concretas.
/// </summary>
public static class FabricaCalidad
{
    public static ICalidadReproduccion Crear(CalidadConexion conexion, bool modoAhorro)
    {
        ICalidadReproduccion calidad = conexion switch
        {
            CalidadConexion.Buena => new Calidad4K(),
            CalidadConexion.Media => new CalidadHD(),
            _ => new CalidadBaja()
        };

        return modoAhorro ? new AhorroDatosDecorator(calidad) : calidad;
    }
}
