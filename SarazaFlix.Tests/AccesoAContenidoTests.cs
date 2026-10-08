using SarazaFlix.DomainModel;
using Xunit;

namespace SarazaFlix.Tests;

/// <summary>
/// Escenarios de acceso a los contenidos: control por plan (proxy de protección) y
/// dispositivo con los últimos 5 vistos (proxy de caché).
/// </summary>
public class AccesoAContenidoTests
{
    [Fact]
    public void ContenidoFueraDelPlanNoSeReproduce()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);

        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C7"); // C7 es Premium

        Assert.Null(sesion);
    }

    [Fact]
    public void ContenidoFueraDelPlanNiSiquieraSeDescarga()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);

        escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C7");

        Assert.Equal(0, escenario.Servidor.Descargas);
    }

    [Fact]
    public void ContenidoFueraDelPlanQuedaRegistradoComoIntentoRechazado()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;

        var permitido = sesion.CambiarContenido("C7");

        Assert.False(permitido);
        Assert.Equal("C1", sesion.Registro.Contenido.Id);
        Assert.Equal(new[] { "Final de la Champions" }, sesion.Registro.IntentosRechazados);
    }

    [Fact]
    public void ElDispositivoMantieneComoMaximoLosUltimos5ContenidosVistos()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);

        // se ven los 7 contenidos del catálogo
        foreach (var contenido in escenario.Catalogo)
        {
            var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, contenido.Id)!;
            escenario.Servicio.Detener(sesion);
        }

        Assert.Equal(5, escenario.Servicio.ContenidosEnDispositivo.Count);
        Assert.DoesNotContain(escenario.Servicio.ContenidosEnDispositivo, c => c.Id == "C1"); // el más viejo salió
        Assert.Contains(escenario.Servicio.ContenidosEnDispositivo, c => c.Id == "C7");
    }

    [Fact]
    public void ContenidoYaVistoNoSeVuelveADescargarDelServidor()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var primera = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;
        escenario.Servicio.Detener(primera);
        var descargasTrasLaPrimeraVez = escenario.Servidor.Descargas;

        var segunda = escenario.Servicio.Iniciar(escenario.UsuarioBasico, "C1")!;

        Assert.True(segunda.Registro.ServidoDesdeDispositivo);
        Assert.Equal(descargasTrasLaPrimeraVez, escenario.Servidor.Descargas);
    }

    [Fact]
    public void UnContenidoInexistenteNoSeReproduce()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);

        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, "NO-EXISTE");

        Assert.Null(sesion);
    }
}
