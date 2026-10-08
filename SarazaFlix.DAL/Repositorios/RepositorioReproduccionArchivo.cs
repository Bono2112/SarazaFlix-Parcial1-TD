using SarazaFlix.DomainModel;

namespace SarazaFlix.DAL.Repositorios;

/// <summary>
/// Persistencia de cada reproducción en un archivo de texto con formato personalizado
/// (campos separados por ';', listas separadas por ',').
/// </summary>
public class RepositorioReproduccionArchivo : IRepositorioReproduccion
{
    private const string Encabezado = "# usuarioId;usuarioNombre;planUsuario;contenidoId;contenidoTitulo;planMinimo;calidades;origen;rechazos";

    private readonly string _ruta;

    public RepositorioReproduccionArchivo(string ruta) => _ruta = ruta;

    public string Ruta => _ruta;

    public void Guardar(Reproduccion reproduccion)
    {
        if (!File.Exists(_ruta))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_ruta))!);
            File.WriteAllText(_ruta, Encabezado + Environment.NewLine);
        }

        File.AppendAllText(_ruta, Serializar(reproduccion) + Environment.NewLine);
    }

    public IReadOnlyList<Reproduccion> ObtenerTodas()
    {
        if (!File.Exists(_ruta))
            return Array.Empty<Reproduccion>();

        var reproducciones = new List<Reproduccion>();
        foreach (var linea in File.ReadAllLines(_ruta))
        {
            if (string.IsNullOrWhiteSpace(linea) || linea.StartsWith('#'))
                continue;

            reproducciones.Add(Deserializar(linea));
        }

        return reproducciones;
    }

    private static string Serializar(Reproduccion reproduccion) => string.Join(';',
        reproduccion.Usuario.Id,
        reproduccion.Usuario.Nombre,
        reproduccion.Usuario.Plan,
        reproduccion.Contenido.Id,
        reproduccion.Contenido.Titulo,
        reproduccion.Contenido.PlanMinimo,
        string.Join(',', reproduccion.CalidadesUsadas),
        reproduccion.ServidoDesdeDispositivo ? "DISPOSITIVO" : "SERVIDOR",
        string.Join(',', reproduccion.IntentosRechazados));

    private static Reproduccion Deserializar(string linea)
    {
        var campos = linea.Split(';');
        var usuario = new Usuario(campos[0], campos[1], Enum.Parse<Plan>(campos[2]));
        var contenido = new Contenido(campos[3], campos[4], Enum.Parse<Plan>(campos[5]));

        var reproduccion = new Reproduccion(usuario, contenido)
        {
            ServidoDesdeDispositivo = campos[7] == "DISPOSITIVO"
        };

        foreach (var calidad in campos[6].Split(',', StringSplitOptions.RemoveEmptyEntries))
            reproduccion.RegistrarCalidad(Enum.Parse<CalidadReproduccion>(calidad));

        if (campos.Length > 8)
            reproduccion.IntentosRechazados.AddRange(
                campos[8].Split(',', StringSplitOptions.RemoveEmptyEntries));

        return reproduccion;
    }
}
