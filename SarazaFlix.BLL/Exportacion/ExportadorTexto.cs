namespace SarazaFlix.BLL.Exportacion;

/// <summary>
/// Exportador de listados a texto plano separado por '|'. Una sola instancia sirve para
/// contenidos, usuarios, reproducciones o cualquier clase que se agregue después.
/// </summary>
public class ExportadorTexto<T> : ExportadorBase<T>
{
    protected override string Separador => "|";
}
