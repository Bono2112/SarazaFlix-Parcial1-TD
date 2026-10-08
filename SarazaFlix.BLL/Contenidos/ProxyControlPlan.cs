using SarazaFlix.DAL.Contenidos;
using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Contenidos;

/// <summary>
/// PATRÓN PROXY (proxy de protección): impide reproducir contenido que no esté incluido en el plan
/// del usuario y registra el intento rechazado. Sólo delega en la fuente siguiente si el plan lo
/// habilita, por eso el contenido rechazado ni siquiera se descarga.
/// </summary>
public class ProxyControlPlan : IFuenteContenido
{
    private readonly IFuenteContenido _siguiente;
    private readonly IReadOnlyCollection<Contenido> _catalogo;
    private readonly Usuario _usuario;

    public ProxyControlPlan(IFuenteContenido siguiente, IReadOnlyCollection<Contenido> catalogo, Usuario usuario)
    {
        _siguiente = siguiente;
        _catalogo = catalogo;
        _usuario = usuario;
    }

    /// <summary>Contenidos que el usuario quiso ver y su plan no incluye (van a la persistencia).</summary>
    public List<string> IntentosRechazados { get; } = new();

    public Contenido? Obtener(string idContenido)
    {
        var delCatalogo = _catalogo.FirstOrDefault(c => c.Id == idContenido);
        if (delCatalogo == null)
            return null;

        if (!delCatalogo.EstaIncluidoEn(_usuario.Plan))
        {
            IntentosRechazados.Add(delCatalogo.Titulo);
            return null;
        }

        return _siguiente.Obtener(idContenido);
    }
}
