using SarazaFlix.DomainModel;
using Xunit;

namespace SarazaFlix.Tests;

/// <summary>
/// Escenario de persistencia: cada reproducción guarda usuario, contenido, calidades usadas,
/// origen y los intentos rechazados por plan.
/// </summary>
public class PersistenciaTests
{
    [Fact]
    public void CadaReproduccionSePersisteCompleta()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;

        escenario.Monitor.Medir(CalidadConexion.Media); // cambio de calidad en plena reproducción
        sesion.ActivarModoAhorro(true);
        sesion.CambiarContenido("C7");                  // intento rechazado por plan
        escenario.Servicio.Detener(sesion);

        var guardada = Assert.Single(escenario.Repositorio.ObtenerTodas());
        Assert.Equal("Ana", guardada.Usuario.Nombre);
        Assert.Equal(Plan.Basico, guardada.Usuario.Plan);
        Assert.Equal("C1", guardada.Contenido.Id);
        Assert.Equal(new[] { CalidadReproduccion.CuatroK, CalidadReproduccion.HD, CalidadReproduccion.Baja },
            guardada.CalidadesUsadas);
        Assert.False(guardada.ServidoDesdeDispositivo);
        Assert.Equal(new[] { "Final de la Champions" }, guardada.IntentosRechazados);
    }

    [Fact]
    public void ElArchivoUsaUnFormatoPersonalizadoConEncabezado()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;
        escenario.Servicio.Detener(sesion);

        var lineas = File.ReadAllLines(escenario.Repositorio.Ruta);

        Assert.StartsWith("#", lineas[0]);
        Assert.Contains("calidades", lineas[0]);
        Assert.Equal("U1;Ana;Basico;C1;Tutorial de C#;Basico;CuatroK;SERVIDOR;", lineas[1]);
    }

    [Fact]
    public void CadaReproduccionSeAgregaComoUnaLinea()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);

        foreach (var id in new[] { "C1", "C2", "C1" })
        {
            var sesion = escenario.Servicio.Iniciar(escenario.UsuarioBasico, id)!;
            escenario.Servicio.Detener(sesion);
        }

        Assert.Equal(3, escenario.Repositorio.ObtenerTodas().Count);
    }

    [Fact]
    public void ElOrigenSePersisteSegunDeDondeVinoElContenido()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);

        var primera = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;
        escenario.Servicio.Detener(primera);

        var segunda = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;
        escenario.Servicio.Detener(segunda);

        var guardadas = escenario.Repositorio.ObtenerTodas();
        Assert.False(guardadas[0].ServidoDesdeDispositivo);
        Assert.True(guardadas[1].ServidoDesdeDispositivo);
    }
}
