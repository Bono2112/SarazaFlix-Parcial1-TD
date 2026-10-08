using SarazaFlix.DomainModel;
using Xunit;

namespace SarazaFlix.Tests;

/// <summary>
/// Escenarios de calidad de reproducción: conexión, cambios en plena reproducción y modo ahorro.
/// </summary>
public class CalidadDeReproduccionTests
{
    [Theory]
    [InlineData(CalidadConexion.Buena, CalidadReproduccion.CuatroK)]
    [InlineData(CalidadConexion.Media, CalidadReproduccion.HD)]
    [InlineData(CalidadConexion.Mala, CalidadReproduccion.Baja)]
    public void LaCalidadDependeDeLaConexionMedida(CalidadConexion conexion, CalidadReproduccion esperada)
    {
        using var escenario = new EscenarioDePrueba(conexion);

        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, "C1")!;

        Assert.Equal(esperada, sesion.CalidadActual);
    }

    [Theory]
    [InlineData(CalidadConexion.Buena)]
    [InlineData(CalidadConexion.Media)]
    [InlineData(CalidadConexion.Mala)]
    public void ElModoAhorroReproduceSiempreEnBajaDefinicion(CalidadConexion conexion)
    {
        using var escenario = new EscenarioDePrueba(conexion);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, "C1")!;

        sesion.ActivarModoAhorro(true);

        Assert.Equal(CalidadReproduccion.Baja, sesion.CalidadActual);
    }

    [Fact]
    public void CambiaLaCalidadEnPlenaReproduccionCuandoCambiaLaConexion()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, "C1")!;
        Assert.Equal(CalidadReproduccion.CuatroK, sesion.CalidadActual);

        escenario.Monitor.Medir(CalidadConexion.Mala); // el sistema vuelve a medir la conexión

        Assert.Equal(CalidadReproduccion.Baja, sesion.CalidadActual);
        Assert.Equal(new[] { CalidadReproduccion.CuatroK, CalidadReproduccion.Baja },
            sesion.Registro.CalidadesUsadas);
    }

    [Fact]
    public void AlApagarElModoAhorroVuelveLaCalidadDeLaConexion()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, "C1")!;

        sesion.ActivarModoAhorro(true);
        sesion.ActivarModoAhorro(false);

        Assert.Equal(CalidadReproduccion.CuatroK, sesion.CalidadActual);
    }

    [Fact]
    public void LaReproduccionSeCortaAlDetenerla()
    {
        using var escenario = new EscenarioDePrueba(CalidadConexion.Buena);
        var sesion = escenario.Servicio.Iniciar(escenario.UsuarioPremium, "C1")!;

        escenario.Servicio.Detener(sesion);
        escenario.Monitor.Medir(CalidadConexion.Mala); // ya no debería afectarla

        Assert.Equal(CalidadReproduccion.CuatroK, sesion.CalidadActual);
    }
}
