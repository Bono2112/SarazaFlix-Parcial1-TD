using SarazaFlix.BLL.Exportacion;
using SarazaFlix.DomainModel;
using Xunit;

namespace SarazaFlix.Tests;

/// <summary>
/// Escenario de marketing: una única rutina de exportación que sirve para cualquier listado,
/// incluso para clases que todavía no existen.
/// </summary>
public class ExportacionTests
{
    [Fact]
    public void ExportaUnListadoConEncabezados()
    {
        var contenidos = new[]
        {
            new Contenido("C1", "Tutorial de C#", Plan.Basico),
            new Contenido("C2", "Recital en vivo", Plan.Estandar)
        };

        var lineas = Lineas(new ExportadorTexto<Contenido>().Exportar(contenidos));

        Assert.Equal("Id|Titulo|PlanMinimo", lineas[0]);
        Assert.Equal("C1|Tutorial de C#|Basico", lineas[1]);
        Assert.Equal("C2|Recital en vivo|Estandar", lineas[2]);
    }

    [Fact]
    public void ExportaUsuariosYReproduccionesConLaMismaRutina()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;
        escenario.Monitor.Medir(CalidadConexion.Mala);
        escenario.Servicio.Detener(sesion);

        var usuarios = Lineas(new ExportadorTexto<Usuario>().Exportar(new[] { escenario.UsuarioBasico }));
        var reproducciones = Lineas(new ExportadorTexto<Reproduccion>().Exportar(escenario.Repositorio.ObtenerTodas()));

        Assert.Equal("Id|Nombre|Plan", usuarios[0]);
        Assert.Equal("U1|Ana|Basico", usuarios[1]);
        Assert.Equal("Usuario|Contenido|CalidadesUsadas|ServidoDesdeDispositivo|IntentosRechazados", reproducciones[0]);
        Assert.Equal("Ana|Tutorial de C#|CuatroK,Baja|False|", reproducciones[1]);
    }

    [Fact]
    public void ExportaClasesQueSeAgreguenEnElFuturoSinTocarLaRutina()
    {
        var promociones = new[]
        {
            new PromocionFutura { Codigo = "PROMO10", Descuento = 10, Vigente = true }
        };

        var lineas = Lineas(new ExportadorTexto<PromocionFutura>().Exportar(promociones));

        Assert.Equal("Codigo|Descuento|Vigente", lineas[0]);
        Assert.Equal("PROMO10|10|True", lineas[1]);
    }

    [Fact]
    public void ExportaElListadoAUnArchivo()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"export-{Guid.NewGuid():N}.txt");
        try
        {
            new ExportadorTexto<Contenido>().Exportar(ruta, new[] { new Contenido("C9", "Otro contenido", Plan.Premium) });

            var lineas = File.ReadAllLines(ruta);
            Assert.Equal("Id|Titulo|PlanMinimo", lineas[0]);
            Assert.Equal("C9|Otro contenido|Premium", lineas[1]);
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    private static string[] Lineas(string texto)
        => texto.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
}

/// <summary>
/// Clase que en el escenario del enunciado "se agrega en el futuro": la rutina de exportación
/// la exporta igual porque trabaja por reflexión.
/// </summary>
internal class PromocionFutura
{
    public string Codigo { get; init; } = string.Empty;

    public int Descuento { get; init; }

    public bool Vigente { get; init; }
}
