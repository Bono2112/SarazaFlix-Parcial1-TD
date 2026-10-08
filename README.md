# SarazaFlix S.A. — Núcleo de la plataforma de video (variante con patrón State)

Resolución del ejercicio práctico de la **Evaluación Parcial Nº 1** (Trabajo de diploma).
Arquitectura en capas con patrones de diseño en C# / .NET 9.

> Esta rama (`patron-state`) es la **variante con State** para comparar con la versión de `main`
> (Strategy + Decorator). El comportamiento es el mismo: **los 23 casos de prueba pasan en las dos
> ramas sin modificar una sola línea de los tests**.

## Estructura de la solución

| Proyecto | Capa | Contenido |
|---|---|---|
| `SarazaFlix.DomainModel` | Dominio | `Usuario`, `Contenido`, `Reproduccion` y los enums `Plan`, `CalidadConexion`, `CalidadReproduccion` |
| `SarazaFlix.DAL` | Acceso a datos | `IFuenteContenido` / `ServidorRemotoContenido` e `IRepositorioReproduccion` / `RepositorioReproduccionArchivo` |
| `SarazaFlix.BLL` | Negocio | Estados de reproducción (`Estados/`), monitor de conexión, proxies de contenidos, exportador genérico, `Reproductor` y `ServicioReproduccion` |
| `SarazaFlix.Consola` | Presentación | Recorre los escenarios del enunciado e imprime el resultado |
| `SarazaFlix.Tests` | Pruebas | Un caso de prueba por cada escenario planteado (xUnit) |

Dependencias: `DAL → DomainModel`, `BLL → DomainModel + DAL`, `Consola y Tests → BLL + DAL + DomainModel`.

## Patrones utilizados y por qué

| Requisito del enunciado | Patrón | Dónde |
|---|---|---|
| La calidad depende de la conexión y debe poder cambiar **en plena reproducción** | **State** | `IEstadoReproduccion` + `ConexionBuena`, `ConexionMedia`, `ConexionMala`; contexto: `Reproductor` |
| El sistema **mide la conexión cada cierto tiempo** y hay que reaccionar sin acoplarse | **Observer** | Subject: `MonitorConexion`; observador: `Reproductor` (`IObservadorConexion`) |
| Tabla conexión → estado en un solo lugar, sin que los estados conozcan los concretos | **Factory Method** | `FabricaEstado.Crear(CalidadConexion)` |
| Modo ahorro de datos: reproduce **siempre en baja** sin importar la conexión, y se prende desde un botón | **State** (estado `AhorroDeDatos`) | `AhorroDeDatos` reproduce en baja y guarda adentro el estado de conexión |
| Impedir contenido fuera del plan y registrar el intento rechazado | **Proxy** (control de acceso) | `ProxyControlPlan` |
| Mantener en el dispositivo los **últimos 5 vistos** y no volver a descargarlos | **Proxy** (caché) | `ProxyCacheLocal` |
| La pantalla no distingue si el contenido viene del servidor o del dispositivo | **Proxy** (misma interfaz) | Los tres implementan `IFuenteContenido` |
| Exportar **cualquier listado** con encabezados mediante una única rutina, incluso clases futuras | **Template Method + Reflexión** | `ExportadorBase<T>` / `ExportadorTexto<T>` |
| Persistir cada reproducción | **Repository** | `IRepositorioReproduccion` → `RepositorioReproduccionArchivo` |

Cadena de acceso al contenido: `ServicioReproduccion` → `ProxyControlPlan` → `ProxyCacheLocal` → `ServidorRemotoContenido`.

### Cómo funciona el State acá

- Cada estado sabe dos cosas: **con qué calidad reproduce** (`Calidad`) y **hacia qué estado pasar**
  cuando llega una medición de conexión (`Siguiente(medicion)`). La transición la decide el estado, no el
  `Reproductor`; el contexto sólo pide el siguiente y registra la calidad resultante.
- `AhorroDeDatos` es un estado que **envuelve** al estado de conexión: reproduce siempre en baja y
  conserva el estado de abajo, actualizándolo con cada medición. Al apagar el modo ahorro, el
  `Reproductor` vuelve a ese estado de conexión, así retoma la calidad que corresponde en ese momento.
- El contexto no guarda un flag de "modo ahorro": `ModoAhorro` se deduce del estado actual
  (`_estado is AhorroDeDatos`). Es la diferencia de fondo con Strategy, donde el modo ahorro era un
  decorador sobre una estrategia elegida por el contexto.

### Comparación con la versión de `main`

| | `main` (Strategy) | `patron-state` |
|---|---|---|
| Variación de la calidad | `ICalidadReproduccion` + 3 estrategias | `IEstadoReproduccion` + 3 estados de conexión |
| Quién elige la calidad | El contexto, vía `FabricaCalidad` | El estado actual, vía `Siguiente(medicion)` |
| Modo ahorro | `AhorroDatosDecorator` sobre la estrategia | Estado `AhorroDeDatos` que envuelve al estado de conexión |
| Cantidad de clases | 6 en `Calidades/` | 6 en `Estados/` |
| Resto de las capas y patrones | idénticos | idénticos |
| Casos de prueba | 23/23 | 23/23 (los mismos, sin modificaciones) |

En los dos casos el disparador del cambio es externo (la medición del monitor o el botón), así que la
diferencia es de **intención**: en Strategy el contexto elige el algoritmo; en State el objeto delega su
comportamiento en un estado que además participa de la transición.

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
`SarazaFlix.Consola/bin/Debug/net9.0/datos/`. La salida es idéntica a la de `main`.

## Documentación

- `docs/DiagramaDeClases.pdf` — diagrama de clases de esta variante (A3 apaisado).
- `docs/diagrama-de-clases.png` — el mismo diagrama en imagen.
- `docs/diagrama-de-clases.html` — fuente del diagrama (Mermaid).
