using SarazaFlix.DomainModel;

namespace SarazaFlix.BLL.Calidades;

/// <summary>
/// PATRÓN DECORATOR: agrega el modo ahorro de datos por encima de la estrategia que correspondía
/// por conexión, sin modificar ni las estrategias ni el reproductor.
/// Con el modo activo se reproduce siempre en baja definición, sin importar la conexión.
/// </summary>
public class AhorroDatosDecorator : ICalidadReproduccion
{
    private readonly ICalidadReproduccion _calidadPorConexion;
    private readonly CalidadBaja _baja = new();

    public AhorroDatosDecorator(ICalidadReproduccion calidadPorConexion)
        => _calidadPorConexion = calidadPorConexion;

    /// <summary>El modo ahorro siempre termina en baja definición.</summary>
    public CalidadReproduccion Calidad => CalidadReproduccion.Baja;

    /// <summary>Calidad que se usaría si el usuario apagara el modo ahorro.</summary>
    public CalidadReproduccion CalidadQueCorresponderiaPorConexion => _calidadPorConexion.Calidad;

    public string Reproducir(Contenido contenido)
        => $"{_baja.Reproducir(contenido)} (modo ahorro de datos activo)";
}
