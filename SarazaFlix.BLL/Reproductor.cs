using SarazaFlix.BLL.Calidades;
using SarazaFlix.BLL.Conexion;
using SarazaFlix.BLL.Contenidos;
using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL;

/// <summary>
/// Pantalla de reproducción.
/// Es el contexto del Strategy (la calidad con la que reproduce) y un observador del monitor de
/// conexión, así puede cambiar la forma de reproducir en plena reproducción.
/// </summary>
public class Reproductor : IObservadorConexion
{
    private readonly ProxyControlPlan _fuente;
    private readonly ProxyCacheLocal _dispositivo;
    private readonly MonitorConexion _monitor;
    private ICalidadReproduccion _calidad;

    internal Reproductor(Reproduccion registro, ProxyControlPlan fuente, ProxyCacheLocal dispositivo,
        MonitorConexion monitor)
    {
        Registro = registro;
        _fuente = fuente;
        _dispositivo = dispositivo;
        _monitor = monitor;

        Conexion = monitor.UltimaMedicion;
        _calidad = FabricaCalidad.Crear(Conexion, ModoAhorro);
        Registro.RegistrarCalidad(_calidad.Calidad);

        _monitor.Suscribir(this); // Observer: queda escuchando las mediciones de conexión
    }

    /// <summary>Reproducción en curso (lo que después se persiste).</summary>
    public Reproduccion Registro { get; }

    public CalidadConexion Conexion { get; private set; }

    public bool ModoAhorro { get; private set; }

    public CalidadReproduccion CalidadActual => _calidad.Calidad;

    public string Reproducir() => _calidad.Reproducir(Registro.Contenido);

    /// <summary>
    /// El sistema midió la conexión otra vez: se cambia la estrategia de calidad en caliente.
    /// </summary>
    public void AlCambiarConexion(CalidadConexion conexion)
    {
        Conexion = conexion;
        AplicarCalidad();
    }

    /// <summary>Botón manual: el usuario prende o apaga el modo ahorro de datos.</summary>
    public void ActivarModoAhorro(bool activo)
    {
        ModoAhorro = activo;
        AplicarCalidad();
    }

    /// <summary>
    /// El usuario intenta pasar a otro contenido durante la reproducción. Devuelve false si el plan
    /// no lo incluye (el proxy ya dejó registrado el intento rechazado).
    /// </summary>
    public bool CambiarContenido(string idContenido)
    {
        var contenido = _fuente.Obtener(idContenido);
        if (contenido == null)
        {
            SincronizarIntentosRechazados(); // el proxy registró el rechazo
            return false;
        }

        Registro.CambiarContenido(contenido);
        Registro.ServidoDesdeDispositivo = _dispositivo.UltimoSirvioDesdeDispositivo;
        return true;
    }

    /// <summary>Cierra la reproducción: se desuscribe del monitor y junta los intentos rechazados.</summary>
    public void Detener()
    {
        _monitor.Desuscribir(this);
        SincronizarIntentosRechazados();
    }

    /// <summary>Vuelca en el registro los intentos que el proxy de plan rechazó.</summary>
    private void SincronizarIntentosRechazados()
    {
        Registro.IntentosRechazados.Clear();
        Registro.IntentosRechazados.AddRange(_fuente.IntentosRechazados);
    }

    private void AplicarCalidad()
    {
        // Strategy: se reemplaza la estrategia según la conexión y el modo ahorro
        _calidad = FabricaCalidad.Crear(Conexion, ModoAhorro);
        Registro.RegistrarCalidad(_calidad.Calidad);
    }
}
