# SarazaFlix S.A. — Núcleo de la plataforma de video

Resolución del ejercicio práctico de la **Evaluación Parcial Nº 1** (Trabajo de diploma).
Arquitectura en capas con patrones de diseño en C# / .NET 9.

## Estructura de la solución

| Proyecto | Capa | Contenido |
|---|---|---|
| `SarazaFlix.DomainModel` | Dominio | `Usuario`, `Contenido`, `Reproduccion` y los enums `Plan`, `CalidadConexion`, `CalidadReproduccion` |
| `SarazaFlix.DAL` | Acceso a datos | `IFuenteContenido` / `ServidorRemotoContenido` (contenidos en el servidor remoto) e `IRepositorioReproduccion` / `RepositorioReproduccionArchivo` (persistencia) |
| `SarazaFlix.BLL` | Negocio | Estrategias de calidad, fábrica, decorador, monitor de conexión, proxies de acceso a contenidos, exportador genérico, `Reproductor` y `ServicioReproduccion` |
| `SarazaFlix.Consola` | Presentación | Recorre los escenarios del enunciado e imprime el resultado |
| `SarazaFlix.Tests` | Pruebas | Un caso de prueba por cada escenario planteado (xUnit) |

Dependencias: `DAL → DomainModel`, `BLL → DomainModel + DAL`, `Consola y Tests → BLL + DAL + DomainModel`.

## Patrones utilizados y por qué

| Requisito del enunciado | Patrón | Dónde |
|---|---|---|
| La calidad depende de la conexión y debe poder cambiar **en plena reproducción** | **Strategy** | `ICalidadReproduccion` + `Calidad4K`, `CalidadHD`, `CalidadBaja`; contexto: `Reproductor` |
| El sistema **mide la conexión cada cierto tiempo** y hay que reaccionar sin acoplarse | **Observer** | Subject: `MonitorConexion`; observador: `Reproductor` (`IObservadorConexion`) |
| Tabla conexión → calidad en un solo lugar, sin conocer las clases concretas | **Factory Method** | `FabricaCalidad.Crear(conexion, modoAhorro)` |
| Modo ahorro de datos: reproduce **siempre en baja** sin importar la conexión, y se prende desde un botón | **Decorator** | `AhorroDatosDecorator` envuelve la estrategia que correspondía por conexión |
| Impedir contenido fuera del plan y registrar el intento rechazado | **Proxy** (control de acceso) | `ProxyControlPlan` |
| Mantener en el dispositivo los **últimos 5 vistos** y no volver a descargarlos | **Proxy** (caché) | `ProxyCacheLocal` |
| La pantalla no distingue si el contenido viene del servidor o del dispositivo | **Proxy** (misma interfaz) | Los tres implementan `IFuenteContenido` |
| Exportar **cualquier listado** con encabezados mediante una única rutina, incluso clases futuras | **Template Method + Reflexión** | `ExportadorBase<T>` / `ExportadorTexto<T>` |
| Persistir cada reproducción | **Repository** | `IRepositorioReproduccion` → `RepositorioReproduccionArchivo` |

Cadena de acceso al contenido: `ServicioReproduccion` → `ProxyControlPlan` → `ProxyCacheLocal` → `ServidorRemotoContenido`.

> Nota sobre el patrón *State*: no se usó porque la variación de comportamiento ("con qué calidad reproduzco")
> la resuelve Strategy y el cambio lo dispara un agente externo (el monitor o el botón), que es la diferencia
> de intención entre ambos patrones. Además el modo ahorro es ortogonal a la conexión, por eso se modeló
> como Decorator sobre la estrategia y no como un estado más.

## Persistencia

`RepositorioReproduccionArchivo` guarda cada reproducción en un archivo de texto con formato personalizado
(campos separados por `;`, listas por `,`), con un encabezado que describe las columnas:

```
# usuarioId;usuarioNombre;planUsuario;contenidoId;contenidoTitulo;planMinimo;calidades;origen;rechazos
U1;Ana;Basico;C1;Tutorial de C# desde cero;Basico;CuatroK;SERVIDOR;Final de la Champions
```

Cada línea registra usuario, contenido, las calidades usadas durante la reproducción (secuencia de cambios),
si se sirvió desde el servidor o del dispositivo y los intentos rechazados por plan.

## Cómo ejecutarlo

```bash
dotnet build                  # compila la solución
dotnet test                   # ejecuta los casos de prueba (23)
dotnet run --project SarazaFlix.Consola
```

La consola recorre los escenarios en orden (calidad por conexión, cambio en plena reproducción, modo ahorro,
rechazo por plan, caché del dispositivo, persistencia y exportación) y genera los archivos en
`SarazaFlix.Consola/bin/Debug/net9.0/datos/`.

## Documentación

- `docs/DiagramaDeClases.pdf` — diagrama de clases (A3 apaisado).
- `docs/diagrama-de-clases.png` — el mismo diagrama en imagen.
- `docs/diagrama-de-clases.html` — fuente del diagrama (Mermaid).
