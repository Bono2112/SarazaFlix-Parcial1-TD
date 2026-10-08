using SarazaFlix.DAL.Contenidos;
using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Contenidos;

/// <summary>
/// PATRÓN PROXY (proxy de caché): representa el dispositivo del usuario. Mantiene los últimos 5
/// contenidos vistos y los sirve de ahí para no volver a descargarlos.
/// Implementa la misma interfaz que el servidor remoto, así el cliente no distingue el origen.
/// </summary>
public class ProxyCacheLocal : IFuenteContenido
{
    private const int CapacidadDelDispositivo = 5;

    private readonly IFuenteContenido _servidorRemoto;
    private readonly LinkedList<Contenido> _enDispositivo = new();

    public ProxyCacheLocal(IFuenteContenido servidorRemoto) => _servidorRemoto = servidorRemoto;

    /// <summary>Lo que hoy está guardado en el dispositivo (nunca más de 5).</summary>
    public IReadOnlyCollection<Contenido> ContenidosEnDispositivo => _enDispositivo;

    /// <summary>Origen del contenido devuelto en la última llamada a Obtener.</summary>
    public bool UltimoSirvioDesdeDispositivo { get; private set; }

    public Contenido? Obtener(string idContenido)
    {
        var guardado = _enDispositivo.FirstOrDefault(c => c.Id == idContenido);
        if (guardado != null)
        {
            _enDispositivo.Remove(guardado);
            _enDispositivo.AddLast(guardado); // vuelve a ser el último visto
            UltimoSirvioDesdeDispositivo = true;
            return guardado;
        }

        var descargado = _servidorRemoto.Obtener(idContenido);
        UltimoSirvioDesdeDispositivo = false;

        if (descargado == null)
            return null;

        _enDispositivo.AddLast(descargado);
        if (_enDispositivo.Count > CapacidadDelDispositivo)
            _enDispositivo.RemoveFirst(); // sale el contenido visto hace más tiempo

        return descargado;
    }
}
