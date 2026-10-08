using SarazaFlix.BLL.Conexion;
using SarazaFlix.BLL.Contenidos;
using SarazaFlix.BLL.Estados;
using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL;

/// <summary>
/// Pantalla de reproducción.
/// Es el contexto del State (delega la forma de reproducir en su estado actual y le pide la
/// transición ante cada medición) y un observador del monitor de conexión, así puede cambiar la
/// forma de reproducir en plena reproducción.
/// </summary>
public class Reproductor : IObservadorConexion
{
    private readonly ProxyControlPlan _fuente;
    private readonly ProxyCacheLocal _dispositivo;
    private readonly MonitorConexion _monitor;
    private IEstadoReproduccion _estado;

    internal Reproductor(Reproduccion registro, ProxyControlPlan fuente, ProxyCacheLocal dispositivo,
        MonitorConexion monitor)
    {
        Registro = registro;
        _fuente = fuente;
        _dispositivo = dispositivo;
        _monitor = monitor;

        Conexion = monitor.UltimaMedicion;
        _estado = FabricaEstado.Crear(Conexion);
        Registro.RegistrarCalidad(_estado.Calidad);

        _monitor.Suscribir(this); // Observer: queda escuchando las mediciones de conexión
    }

    /// <summary>Reproducción en curso (lo que después se persiste).</summary>
    public Reproduccion Registro { get; }

    public CalidadConexion Conexion { get; private set; }

    /// <summary>El modo ahorro no es un flag aparte: es el estado de ahorro el que lo determina.</summary>
    public bool ModoAhorro => _estado is AhorroDeDatos;

    public CalidadReproduccion CalidadActual => _estado.Calidad;

    public string Reproducir() => _estado.Reproducir(Registro.Contenido);

    /// <summary>
    /// El sistema midió la conexión otra vez: el estado actual decide hacia qué estado pasar,
    /// así se cambia la forma de reproducir en plena reproducción.
    /// </summary>
    public void AlCambiarConexion(CalidadConexion conexion)
    {
        Conexion = conexion;
        CambiarEstado(_estado.Siguiente(conexion));
    }

    /// <summary>Botón manual: el usuario prende o apaga el modo ahorro de datos.</summary>
    public void ActivarModoAhorro(bool activo)
    {
        if (activo == ModoAhorro)
            return;

        CambiarEstado(activo
            ? new AhorroDeDatos(_estado)
            : ((AhorroDeDatos)_estado).EstadoDeConexion); // vuelve al estado de conexión que quedó debajo
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

    private void CambiarEstado(IEstadoReproduccion estado)
    {
        _estado = estado;
        Registro.RegistrarCalidad(_estado.Calidad); // queda registrado cada cambio de calidad real
    }
}
