using SarazaFlix.BLL.Conexion;
using SarazaFlix.BLL.Contenidos;
using SarazaFlix.DAL.Contenidos;
using SarazaFlix.DAL.Repositorios;
using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL;

/// <summary>
/// Punto de entrada de la capa de negocio: arma la cadena de proxies, inicia la reproducción y
/// persiste el registro cuando termina.
/// </summary>
public class ServicioReproduccion
{
    private readonly ServidorRemotoContenido _servidor;
    private readonly IRepositorioReproduccion _repositorio;
    private readonly MonitorConexion _monitor;

    /// <summary>El dispositivo es uno solo y sobrevive entre reproducciones: por eso vive acá.</summary>
    private readonly ProxyCacheLocal _dispositivo;

    public ServicioReproduccion(ServidorRemotoContenido servidor, IRepositorioReproduccion repositorio,
        MonitorConexion monitor)
    {
        _servidor = servidor;
        _repositorio = repositorio;
        _monitor = monitor;
        _dispositivo = new ProxyCacheLocal(servidor);
    }

    /// <summary>Contenidos guardados hoy en el dispositivo.</summary>
    public IReadOnlyCollection<Contenido> ContenidosEnDispositivo => _dispositivo.ContenidosEnDispositivo;

    /// <summary>
    /// Inicia una reproducción. Cadena de acceso: pantalla -> control de plan -> dispositivo -> servidor.
    /// Devuelve null si el contenido no existe o si el plan del usuario no lo incluye.
    /// </summary>
    public Reproductor? Iniciar(Usuario usuario, string idContenido)
    {
        var controlPlan = new ProxyControlPlan(_dispositivo, _servidor.Catalogo, usuario);
        var contenido = controlPlan.Obtener(idContenido);

        if (contenido == null)
            return null;

        var registro = new Reproduccion(usuario, contenido)
        {
            ServidoDesdeDispositivo = _dispositivo.UltimoSirvioDesdeDispositivo
        };

        return new Reproductor(registro, controlPlan, _dispositivo, _monitor);
    }

    /// <summary>Fin de la reproducción: se persiste el registro.</summary>
    public void Detener(Reproductor reproductor)
    {
        reproductor.Detener();
        _repositorio.Guardar(reproductor.Registro);
    }
}
