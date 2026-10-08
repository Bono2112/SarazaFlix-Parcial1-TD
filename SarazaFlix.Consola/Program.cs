using SarazaFlix.BLL;
using SarazaFlix.BLL.Conexion;
using SarazaFlix.BLL.Exportacion;
using SarazaFlix.DAL.Contenidos;
using SarazaFlix.DAL.Repositorios;
using SarazaFlix.DomainModel;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ---------------------------------------------------------------- datos del sistema
var catalogo = new List<Contenido>
{
    new("C1", "Tutorial de C# desde cero", Plan.Basico),
    new("C2", "Serie: Los Simuladores", Plan.Basico),
    new("C3", "Recital en vivo en Obras", Plan.Estandar),
    new("C4", "Documental: historia del rock", Plan.Estandar),
    new("C5", "Curso de patrones de diseño", Plan.Estandar),
    new("C6", "Final de la Champions", Plan.Premium)
};

var usuarios = new List<Usuario>
{
    new("U1", "Ana", Plan.Basico),
    new("U2", "Beto", Plan.Premium)
};

var servidor = new ServidorRemotoContenido(catalogo);
var monitor = new MonitorConexion(CalidadConexion.Buena);

var carpetaDeDatos = Path.Combine(AppContext.BaseDirectory, "datos");
var repositorio = new RepositorioReproduccionArchivo(Path.Combine(carpetaDeDatos, "reproducciones.txt"));
var servicio = new ServicioReproduccion(servidor, repositorio, monitor);

var ana = usuarios[0];

// ---------------------------------------------------------------- 1) calidad según la conexión (Strategy)
var sesion = servicio.Iniciar(ana, "C1")!;
Console.WriteLine($"[1] conexión buena -> {sesion.Reproducir()}");

// ---------------------------------------------------------------- 2) el monitor mide de nuevo (Observer)
monitor.Medir(CalidadConexion.Media);
Console.WriteLine($"[2] la conexión empeoró a media, en plena reproducción -> {sesion.Reproducir()}");

// ---------------------------------------------------------------- 3) botón de ahorro de datos (Decorator)
sesion.ActivarModoAhorro(true);
Console.WriteLine($"[3] modo ahorro activado -> {sesion.Reproducir()}");
monitor.Medir(CalidadConexion.Buena);
Console.WriteLine($"[3] volvió la buena conexión, pero el ahorro sigue -> {sesion.Reproducir()}");

// ---------------------------------------------------------------- 4) control de plan (Proxy de protección)
var rechazado = sesion.CambiarContenido("C6");
Console.WriteLine($"[4] intenta ver 'Final de la Champions' (Premium) con plan Básico -> " +
                  $"{(rechazado ? "permitido" : "RECHAZADO")}");

// ---------------------------------------------------------------- 5) caché del dispositivo (Proxy de caché)
sesion.ActivarModoAhorro(false);
servicio.Detener(sesion);

var segundaVez = servicio.Iniciar(ana, "C1")!;
Console.WriteLine($"[5] ve de nuevo el mismo contenido -> viene del " +
                  $"{(segundaVez.Registro.ServidoDesdeDispositivo ? "dispositivo" : "servidor")} " +
                  $"(descargas del servidor: {servidor.Descargas})");
servicio.Detener(segundaVez);

// ---------------------------------------------------------------- 6) persistencia (Repository)
Console.WriteLine();
Console.WriteLine("--- reproducciones persistidas en " + repositorio.Ruta + " ---");
foreach (var reproduccion in repositorio.ObtenerTodas())
{
    Console.WriteLine($"    {reproduccion.Usuario.Nombre} | {reproduccion.Contenido.Titulo} | " +
                      $"calidades: {string.Join(" -> ", reproduccion.CalidadesUsadas)} | " +
                      $"origen: {(reproduccion.ServidoDesdeDispositivo ? "dispositivo" : "servidor")} | " +
                      $"rechazados por plan: {string.Join(", ", reproduccion.IntentosRechazados)}");
}

// ---------------------------------------------------------------- 7) exportación genérica (Template Method + Reflexión)
Console.WriteLine();
Console.WriteLine("--- exportación de listados (una única rutina para cualquier clase) ---");
new ExportadorTexto<Contenido>().Exportar(Path.Combine(carpetaDeDatos, "contenidos.txt"), catalogo);
new ExportadorTexto<Usuario>().Exportar(Path.Combine(carpetaDeDatos, "usuarios.txt"), usuarios);
new ExportadorTexto<Reproduccion>().Exportar(Path.Combine(carpetaDeDatos, "reproducciones-export.txt"),
    repositorio.ObtenerTodas());

Console.WriteLine(new ExportadorTexto<Contenido>().Exportar(catalogo));
Console.WriteLine($"Archivos generados en {carpetaDeDatos}");
