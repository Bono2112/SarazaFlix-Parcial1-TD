using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Conexion;

/// <summary>
/// PATRÓN OBSERVER (el "subject"): el sistema mide la conexión cada cierto tiempo y avisa a los
/// suscriptores. El monitor no conoce al reproductor, sólo la interfaz de los observadores.
/// </summary>
public class MonitorConexion
{
    private readonly List<IObservadorConexion> _observadores = new();

    public MonitorConexion(CalidadConexion medicionInicial) => UltimaMedicion = medicionInicial;

    /// <summary>Último resultado medido.</summary>
    public CalidadConexion UltimaMedicion { get; private set; }

    public void Suscribir(IObservadorConexion observador) => _observadores.Add(observador);

    public void Desuscribir(IObservadorConexion observador) => _observadores.Remove(observador);

    /// <summary>
    /// Simula una medición periódica: guarda el resultado y notifica a todos los suscriptores.
    /// </summary>
    public void Medir(CalidadConexion medicion)
    {
        UltimaMedicion = medicion;

        foreach (var observador in _observadores.ToList())
            observador.AlCambiarConexion(medicion);
    }
}
