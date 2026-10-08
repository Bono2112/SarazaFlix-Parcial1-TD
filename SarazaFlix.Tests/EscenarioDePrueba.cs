using SarazaFlix.BLL;
using SarazaFlix.BLL.Conexion;
using SarazaFlix.DAL.Contenidos;
using SarazaFlix.DAL.Repositorios;
using SarazaFlix.DomainModel;

namespace SarazaFlix.Tests;

/// <summary>
/// Arma un escenario completo en una carpeta temporal: catálogo, servidor remoto, monitor,
/// repositorio y servicio de reproducción. Se borra sola al terminar cada caso de prueba.
/// </summary>
internal sealed class EscenarioDePrueba : IDisposable
{
    private readonly string _carpeta;

    public EscenarioDePrueba(CalidadConexion conexionInicial)
    {
        _carpeta = Path.Combine(Path.GetTempPath(), "sarazaflix-" + Guid.NewGuid().ToString("N"));

        Catalogo = new List<Contenido>
        {
            new("C1", "Tutorial de C#", Plan.Basico),
            new("C2", "Los Simuladores", Plan.Basico),
            new("C3", "Recital en vivo", Plan.Estandar),
            new("C4", "Documental de historia", Plan.Estandar),
            new("C5", "Serie de misterio", Plan.Basico),
            new("C6", "Curso de patrones", Plan.Estandar),
            new("C7", "Final de la Champions", Plan.Premium)
        };

        Servidor = new ServidorRemotoContenido(Catalogo);
        Monitor = new MonitorConexion(conexionInicial);
        Repositorio = new RepositorioReproduccionArchivo(Path.Combine(_carpeta, "reproducciones.txt"));
        Servicio = new ServicioReproduccion(Servidor, Repositorio, Monitor);
    }

    public List<Contenido> Catalogo { get; }

    public ServidorRemotoContenido Servidor { get; }

    public MonitorConexion Monitor { get; }

    public RepositorioReproduccionArchivo Repositorio { get; }

    public ServicioReproduccion Servicio { get; }

    public Usuario UsuarioBasico => new("U1", "Ana", Plan.Basico);

    public Usuario UsuarioPremium => new("U2", "Beto", Plan.Premium);

    public void Dispose()
    {
        if (Directory.Exists(_carpeta))
            Directory.Delete(_carpeta, true);
    }
}
