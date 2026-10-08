using SarazaFlix.DomainModel;

namespace SarazaFlix.DAL.Repositorios;

/// <summary>
/// Repository: acceso a la persistencia de reproducciones.
/// </summary>
public interface IRepositorioReproduccion
{
    void Guardar(Reproduccion reproduccion);

    IReadOnlyList<Reproduccion> ObtenerTodas();
}
