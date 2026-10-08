using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Conexion;

/// <summary>
/// PATRÓN OBSERVER: contrato de los interesados en enterarse de las mediciones de conexión.
/// </summary>
public interface IObservadorConexion
{
    void AlCambiarConexion(CalidadConexion conexion);
}
