using System.Reflection;
using System.Text;

namespace SarazaFlix.BLL.Exportacion;

/// <summary>
/// PATRÓN TEMPLATE METHOD + REFLEXIÓN.
/// Template Method: acá está la única rutina de exportación (encabezado, filas, formato); las
/// subclases sólo definen el formato concreto.
/// Reflexión: los encabezados y los valores se obtienen de las propiedades del tipo en tiempo de
/// ejecución, por eso funciona con cualquier clase, incluso con las que se agreguen en el futuro.
/// </summary>
public abstract class ExportadorBase<T>
{
    /// <summary>Devuelve el listado exportado como texto.</summary>
    public string Exportar(IEnumerable<T> elementos)
    {
        var texto = new StringBuilder();
        texto.AppendLine(Encabezado());

        foreach (var elemento in elementos)
            texto.AppendLine(Fila(elemento));

        return texto.ToString();
    }

    /// <summary>Exporta el listado a un archivo de texto.</summary>
    public void Exportar(string ruta, IEnumerable<T> elementos)
        => File.WriteAllText(ruta, Exportar(elementos));

    /// <summary>Separador entre columnas del formato.</summary>
    protected abstract string Separador { get; }

    protected virtual string Encabezado()
        => string.Join(Separador, Propiedades().Select(p => p.Name));

    protected virtual string Fila(T elemento)
        => string.Join(Separador, Propiedades().Select(p => Valor(p, elemento)));

    private static IEnumerable<PropertyInfo> Propiedades()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    private static string Valor(PropertyInfo propiedad, T elemento)
    {
        var valor = propiedad.GetValue(elemento);

        return valor switch
        {
            null => string.Empty,
            string texto => texto,
            // las colecciones (por ejemplo las calidades usadas) salen separadas por coma
            System.Collections.IEnumerable coleccion => string.Join(",",
                coleccion.Cast<object>().Select(item => item.ToString())),
            _ => valor.ToString() ?? string.Empty
        };
    }
}
