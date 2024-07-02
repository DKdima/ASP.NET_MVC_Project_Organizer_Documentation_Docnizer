using Microsoft.AspNetCore.Mvc;
using OrganizadorDocumentacion.Models;
using System.Diagnostics;
using OrganizadorDocumentacion.DTOs;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Microsoft.CodeAnalysis.Host;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor;
using Spire.Pdf;
using Spire.Doc;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
namespace OrganizadorDocumentacion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DocnizerContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, DocnizerContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }
        public IActionResult ConexionAPI()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // Eliminar cualquier información de sesión existente
            HttpContext.Session.Clear();
            
            // Redirigir a la página de inicio
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ActualizarCantidadArchivos(int? campoId, int? subcampoId, int nuevaCantidad)
        {
            if (subcampoId.HasValue)
            {
                var subcampo = _context.Subcampos.FirstOrDefault(s => s.SubcampoId == subcampoId);
                if (subcampo != null)
                {
                    subcampo.CantidadArchivos = nuevaCantidad;
                    _context.SaveChanges();
                }
            }
            else  if (campoId.HasValue)
            {
                var campo = _context.Campos.FirstOrDefault(c => c.CampoId == campoId);
                if (campo != null)
                {
                    campo.CantidadArchivos = nuevaCantidad;
                    _context.SaveChanges();
                }
            }

            return Json(new { success = true });
        }


        // Método Index
        [HttpGet]
        public IActionResult Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            var nombre = HttpContext.Session.GetString("Nombre");
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            

            if (usuarioId.HasValue && !string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(tipoUsuario))
            {
                // Utilizar los datos del usuario según sea necesario
                ViewBag.UsuarioId = usuarioId.Value;
                ViewBag.Nombre = nombre;
                ViewBag.TipoUsuario = tipoUsuario;
            }

            // Pasar el nombre del usuario a la vista
            ViewData["UserName"] = ViewBag.Nombre;
            return View();
        }



        public class MetadatosArchivo
        {
            public string Autor { get; set; }
        }

        public MetadatosArchivo ObtenerMetadatosArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return new MetadatosArchivo { Autor = "Desconocido" };
            }

            string autor = "Desconocido";
            string extension = Path.GetExtension(archivo.FileName).ToLower();

            using (var stream = archivo.OpenReadStream())
            {
                if (extension == ".doc" || extension == ".docx")
                {
                    Document document = new Document(stream);
                    autor = document.BuiltinDocumentProperties.Author;
                }
                else if (extension == ".pdf")
                {
                    PdfDocument pdfDocument = new PdfDocument(stream);
                    autor = pdfDocument.DocumentInformation.Author;
                }
                else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    var directories = ImageMetadataReader.ReadMetadata(stream);
                    var exifSubIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                    if (exifSubIfdDirectory != null)
                    {
                        autor = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagArtist) ?? "Desconocido";
                    }
                }
            }

            return new MetadatosArchivo
            {
                Autor = string.IsNullOrEmpty(autor) ? "Desconocido" : autor
            };
        }

        [HttpPost]
        [Route("SubirArchivo")]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo, int? campoId, int? subcampoId)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest("No se ha seleccionado ningún archivo.");
            }

            var fileName = $"{Guid.NewGuid().ToString()}_{archivo.FileName}"; // Añade un GUID al nombre del archivo
            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Archivos", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var metadatos = ObtenerMetadatosArchivo(archivo);

            var nuevoArchivo = new Archivo
            {
                Nombre = fileName,
                CampoId = campoId,
                SubcampoId = subcampoId,
                FechaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Usar fecha actual como fecha de modificación
                Autor = metadatos.Autor,
                Tamano = archivo.Length
            };

            _context.Archivos.Add(nuevoArchivo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Archivo subido correctamente." });
        }

        [HttpPost]
        [Route("SubirArchivoIndex")]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo, string Ref)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest("No se ha seleccionado ningún archivo.");
            }

            var campo = _context.Campos.FirstOrDefault(c => c.Ref == Ref);
            var subcampo = _context.Subcampos.FirstOrDefault(sc => sc.Ref == Ref);

            if (campo == null && subcampo == null)
            {
                return BadRequest("La referencia no existe.");
            }

            int? campoId = campo?.CampoId;
            int? subcampoId = subcampo?.SubcampoId ?? null;
            int? cantidadArchivosMaxima = null;
            int archivosExistentes = 0;

            if (subcampo != null)
            {
                campoId = subcampo.ParentCampoId;
                cantidadArchivosMaxima = subcampo.CantidadArchivos;
                archivosExistentes = _context.Archivos.Count(a => a.SubcampoId == subcampoId);
            }
            else if (campo != null)
            {
                cantidadArchivosMaxima = campo.CantidadArchivos;
                archivosExistentes = _context.Archivos.Count(a => a.CampoId == campoId && a.SubcampoId == null);
            }

            if (cantidadArchivosMaxima.HasValue && archivosExistentes >= cantidadArchivosMaxima)
            {
                return BadRequest("Se ha alcanzado la cantidad máxima de archivos permitidos.");
            }

            var fileName = $"{Guid.NewGuid().ToString()}_{archivo.FileName}"; // Añade un GUID al nombre del archivo
            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Archivos", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var metadatos = ObtenerMetadatosArchivo(archivo);

            var nuevoArchivo = new Archivo
            {
                Nombre = fileName,
                CampoId = campoId,
                SubcampoId = subcampoId,
                FechaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Autor = metadatos.Autor,
                Tamano = archivo.Length
            };

            _context.Archivos.Add(nuevoArchivo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Archivo subido correctamente." });
        }



        [HttpPost]
        [Route("EliminarArchivo")]
        public IActionResult EliminarArchivo(string nombre)
        {
            var archivo = _context.Archivos.FirstOrDefault(a => a.Nombre == nombre);
            if (archivo == null)
            {
                return Json(new { success = false, message = "El archivo no existe." });
            }

            try
            {
                // Eliminar el archivo de la base de datos
                _context.Archivos.Remove(archivo);
                _context.SaveChanges();

                // Eliminar el archivo físico
                var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Archivos", nombre);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar el archivo: " + ex.Message });
            }
        }


        //Método Login

        [HttpPost]
        public IActionResult Login(string Nombre, string Clave)
        {
            // Buscar el usuario en la base de datos por su nombre
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == Nombre);

            if (usuario != null && usuario.Clave == Clave)
            {
                // Almacenar datos del usuario en la sesión
                HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioId);
                HttpContext.Session.SetString("Nombre", usuario.Nombre);
                HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario.ToString());

                // Credenciales válidas, redirigir a la página MisIniciativas
                return RedirectToAction("MisIniciativas", "Home");
            }
            else
            {
                // Credenciales inválidas, mostrar un mensaje de error
                ViewBag.ErrorMessage = "Nombre de usuario o contraseña incorrectos";

                return View("Index");
            }
        }


        //Método Login acaba aquí

        public IActionResult Privacy()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            var nombre = HttpContext.Session.GetString("Nombre");
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (usuarioId.HasValue && !string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(tipoUsuario))
            {
                // Utilizar los datos del usuario según sea necesario
                ViewBag.UsuarioId = usuarioId.Value;
                ViewBag.Nombre = nombre;
                ViewBag.TipoUsuario = tipoUsuario;
            }

            // Pasar el nombre del usuario a la vista
            ViewData["UserName"] = ViewBag.Nombre;


            return View();
        }

        public IActionResult MisIniciativas()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            var nombre = HttpContext.Session.GetString("Nombre");
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (usuarioId.HasValue && !string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(tipoUsuario))
            {
                // Utilizar los datos del usuario según sea necesario
                ViewBag.UsuarioId = usuarioId.Value;
                ViewBag.Nombre = nombre;
                ViewBag.TipoUsuario = tipoUsuario;
            }
            else
            {
                // Redirigir al login si no hay datos de sesión
                return RedirectToAction("Index", "Home");
            }
            // Consultar las iniciativas del usuario utilizando su ID
            var iniciativas = _context.UsuarioIniciativa
                .Where(ui => ui.UsuarioId == usuarioId)
                .Select(ui => ui.Iniciativa)
                .ToList();

            // Obtener el nombre del usuario autenticado
            string userName = ViewBag.Nombre;
            // Pasar el nombre del usuario a la vista
            ViewData["UserName"] = userName;

            // Llamar al procedimiento almacenado para eliminar las iniciativas sin vinculación
            _context.Database.ExecuteSqlRaw("EXEC EliminarIniciativasSinVinculoUsuario");
            return View(iniciativas);
        }

        [HttpPost]
        public IActionResult EditarIniciativaNombre(int iniciativaId, string nuevoNombre)
        {
            // Encuentra la iniciativa por ID y actualiza el nombre
            var iniciativa = _context.Iniciativas.FirstOrDefault(i => i.IniciativaId == iniciativaId);
            if (iniciativa != null)
            {
                iniciativa.Nombre = nuevoNombre;
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }


        public IActionResult EliminarIniciativa(int iniciativaId)
        {
            try
            {
                // Obtener los nombres de los archivos asociados a la iniciativa
                var archivos = new List<string>();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_ObtenerNombresArchivosPorIniciativa";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IniciativaId", iniciativaId));

                    _context.Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            archivos.Add(reader.GetString(0));
                        }
                    }
                }

                // Eliminar físicamente los archivos
                foreach (var nombreArchivo in archivos)
                {
                    var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Archivos", nombreArchivo);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        _logger.LogInformation("Archivo eliminado: {0}", path);
                    }
                    else
                    {
                        _logger.LogWarning("No se encontró el archivo para eliminar: {0}", path);
                    }
                }

                // Llamar al procedimiento almacenado para eliminar la iniciativa
                _context.Database.ExecuteSqlRaw("EXEC sp_EliminarIniciativa @IniciativaId", new SqlParameter("@IniciativaId", iniciativaId));
                
                // Redirigir a la vista MisIniciativas después de la eliminación
                return RedirectToAction("MisIniciativas");
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                _logger.LogError(ex, "Error al eliminar la iniciativa");
                return RedirectToAction("MisIniciativas");
            }
        }

        public async Task<IActionResult> EliminarIniciativasPorUsuario(int usuarioId)
        {
            try
            {
                // Obtener las iniciativas asociadas al usuario a través de la tabla intermedia
                var iniciativas = await _context.UsuarioIniciativa
                                                 .Where(ui => ui.UsuarioId == usuarioId)
                                                 .Select(ui => ui.Iniciativa)
                                                 .ToListAsync();

                foreach (var iniciativa in iniciativas)
                {
                    // Obtener los nombres de los archivos asociados a la iniciativa
                    var archivos = new List<string>();
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "sp_ObtenerNombresArchivosPorIniciativa";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@IniciativaId", iniciativa.IniciativaId));

                        _context.Database.OpenConnection();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                archivos.Add(reader.GetString(0));
                            }
                        }
                    }

                    // Eliminar físicamente los archivos
                    foreach (var nombreArchivo in archivos)
                    {
                        var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Archivos", nombreArchivo);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                            _logger.LogInformation("Archivo eliminado: {0}", path);
                        }
                        else
                        {
                            _logger.LogWarning("No se encontró el archivo para eliminar: {0}", path);
                        }
                    }

                    // Llamar al procedimiento almacenado para eliminar la iniciativa
                    _context.Database.ExecuteSqlRaw("EXEC sp_EliminarIniciativa @IniciativaId", new SqlParameter("@IniciativaId", iniciativa.IniciativaId));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                _logger.LogError(ex, "Error al eliminar las iniciativas del usuario");
                return StatusCode(500, "Error interno del servidor");
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    _context.Database.CloseConnection();
                }
            }
        }

        public IActionResult CalcularCantidadArchivos(int iniciativaId)
        {
            // Cadena de conexión a la base de datos
            var connectionString = _configuration.GetConnectionString("conexion");

            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new { IniciativaId = iniciativaId };
                var result = connection.QuerySingleOrDefault<dynamic>("sp_CalcularCantidadArchivos", parameters, commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    var response = new
                    {
                        SumaCantidadArchivos = result.SumaCantidadArchivos,
                        CantidadActualArchivos = result.CantidadActualArchivos
                    };

                    return Json(response);
                }
                else
                {
                    return Json(new { SumaCantidadArchivos = 0, CantidadActualArchivos = 0 });
                }
            }
        }



        public IActionResult CrearIniciativa()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            var nombre = HttpContext.Session.GetString("Nombre");
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (usuarioId.HasValue && !string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(tipoUsuario))
            {
                // Utilizar los datos del usuario según sea necesario
                ViewBag.UsuarioId = usuarioId.Value;
                ViewBag.Nombre = nombre;
                ViewBag.TipoUsuario = tipoUsuario;
            }

            // Consultar la iniciativa por UsuarioID
            

            // Pasar el nombre del usuario a la vista
            ViewData["UserName"] = ViewBag.Nombre;
            return View();
        }

        public IActionResult LaIniciativa(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            var nombre = HttpContext.Session.GetString("Nombre");
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (usuarioId == null || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(tipoUsuario))
            {
                // Si faltan los datos de sesión, redirige al usuario al login
                return RedirectToAction("Index", "Home");
            }

            // Consultar la iniciativa asociada al usuario y al id proporcionado
            var usuarioIniciativa = _context.UsuarioIniciativa
                .Include(ui => ui.Iniciativa)
                .ThenInclude(i => i.Campos)
                .ThenInclude(c => c.Subcampos)
                .FirstOrDefault(ui => ui.UsuarioId == usuarioId && ui.IniciativaId == id);

            if (usuarioIniciativa == null)
            {
                // Si no se encuentra la iniciativa asociada al usuario, devolver un error
                return NotFound();
            }

            var iniciativa = usuarioIniciativa.Iniciativa;

            // Pasar la iniciativa y los detalles del usuario a la vista
            ViewBag.UsuarioId = usuarioId.Value;
            ViewBag.Nombre = nombre;
            ViewBag.TipoUsuario = tipoUsuario;
            ViewBag.UserName = nombre;


            // Pasar el nombre del usuario a la vista
            ViewData["UserName"] = ViewBag.Nombre;
            return View(iniciativa);
        }

        [HttpGet]
        public IActionResult GetSubcampos(int campoId)
        {
            var subcampos = _context.Subcampos
                .Where(s => s.ParentCampoId == campoId)
                .Select(s => new { s.SubcampoId, s.Nombre })
                .ToList();

            return Json(subcampos);
        }

        [HttpPost]
        public IActionResult ObtenerRefCampo(int? campoId, int? subcampoId)
        {
            string refSeleccionado = "";

            if (campoId.HasValue)
            {
                // Consultar la referencia del campo
                var campo = _context.Campos.FirstOrDefault(c => c.CampoId == campoId);
                if (campo != null)
                {
                    refSeleccionado = campo.Ref;
                }
            }
            else if (subcampoId.HasValue)
            {
                // Consultar la referencia del subcampo
                var subcampo = _context.Subcampos.FirstOrDefault(s => s.SubcampoId == subcampoId);
                if (subcampo != null)
                {
                    refSeleccionado = subcampo.Ref;
                }
            }

            // Devolver la referencia como resultado de la llamada AJAX
            return Json(refSeleccionado);
        }

        [HttpPost]
        public IActionResult ObtenerCantidadArchivos(int? campoId, int? subcampoId)
        {
            // Inicializar la variable para la cantidad de archivos permitidos
            int cantidadArchivos = 0;

            if (campoId.HasValue)
            {
                // Consultar la cantidad de archivos permitidos asociados al campo
                var campo = _context.Campos.FirstOrDefault(c => c.CampoId == campoId);
                if (campo != null && campo.CantidadArchivos.HasValue)
                {
                    cantidadArchivos = campo.CantidadArchivos.Value;
                }
            }
            else if (subcampoId.HasValue)
            {
                // Consultar la cantidad de archivos permitidos asociados al subcampo
                var subcampo = _context.Subcampos.FirstOrDefault(s => s.SubcampoId == subcampoId);
                if (subcampo != null && subcampo.CantidadArchivos.HasValue)
                {
                    cantidadArchivos = subcampo.CantidadArchivos.Value;
                }
            }

            // Devolver la cantidad de archivos permitidos como resultado de la llamada AJAX
            return Json(cantidadArchivos);
        }




        [HttpPost]
        public IActionResult ObtenerArchivos(int? campoId, int? subcampoId)
        {
            List<Archivo> archivos = new List<Archivo>();

            if (campoId.HasValue)
            {
                // Consultar archivos asociados al campo
                archivos = _context.Archivos
                    .Where(a => a.CampoId == campoId && a.SubcampoId == subcampoId)
                    .Select(a => new Archivo
                    {
                        Nombre = a.Nombre ?? "Nombre Desconocido",
                       FechaModificacion = a.FechaModificacion ?? "Fecha Desconocida",
                       Autor = a.Autor ?? "Autor Desconocido",
                       Tamano = a.Tamano
                    })
                    .ToList();
            }
            else if (subcampoId.HasValue)
            {
                // Consultar archivos asociados al subcampo
                archivos = _context.Archivos
                    .Where(a => a.SubcampoId == subcampoId)
                    .Select(a => new Archivo
                    {
                        Nombre = a.Nombre ?? "Nombre Desconocido",
                       FechaModificacion = a.FechaModificacion ?? "Fecha Desconocida",
                        Autor = a.Autor ?? "Autor Desconocido",
                        Tamano = a.Tamano
                    })
                    .ToList();
            }

            // Devolver la lista de archivos como resultado de la llamada AJAX
            return Json(archivos);
        }

        [HttpGet]
        [Route("DescargarArchivo")]
        public IActionResult DescargarArchivo(string nombreArchivo)
        {
            try
            {
                var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Archivos", nombreArchivo);
                if (System.IO.File.Exists(path))
                {
                    // Devolver el archivo como un FileResult
                    byte[] archivoBytes = System.IO.File.ReadAllBytes(path);
                    string tipoMIME = "application/octet-stream";

                    // Extraer el nombre del archivo después del primer guion bajo
                    var partesNombre = nombreArchivo.Split('_', 2);
                    string nuevoNombreArchivo = partesNombre.Length > 1 ? partesNombre[1] : nombreArchivo;

                    return File(archivoBytes, tipoMIME, nuevoNombreArchivo);
                }
                else
                {
                    _logger.LogWarning("No se encontró el archivo para descargar: {0}", path);
                    return RedirectToAction("ArchivoNoEncontrado");
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "FileNotFoundException al intentar descargar el archivo");
                return RedirectToAction("ArchivoNoEncontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción general al intentar descargar el archivo");
                return RedirectToAction("ArchivoNoEncontrado");
            }
        }




        [HttpPost]
        [Route("GuardarIniciativa")]
        public IActionResult GuardarIniciativa(string iniciativaJson)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
                var nombre = HttpContext.Session.GetString("Nombre");
                var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

                if (usuarioId.HasValue && !string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(tipoUsuario))
                {
                    // Utilizar los datos del usuario según sea necesario
                    ViewBag.UsuarioId = usuarioId.Value;
                    ViewBag.Nombre = nombre;
                    ViewBag.TipoUsuario = tipoUsuario;
                }

                // Deserializar el JSON en un objeto IniciativaDTO
                var iniciativa = JsonConvert.DeserializeObject<IniciativaDTO>(iniciativaJson);

                // Insertar la iniciativa en la base de datos
                var nuevaIniciativa = new Iniciativa
                {
                    Nombre = iniciativa.NombreIniciativa,
                };
                _context.Iniciativas.Add(nuevaIniciativa);
                _context.SaveChanges();

                // Insertar la relación entre el usuario y la iniciativa en la base de datos
                var usuarioIniciativa = new UsuarioIniciativa
                {
                    UsuarioId = usuarioId.Value,
                    IniciativaId = nuevaIniciativa.IniciativaId
                };
                _context.UsuarioIniciativa.Add(usuarioIniciativa);
                _context.SaveChanges();


                // Insertar los campos de la iniciativa en la base de datos
                GuardarCampos(iniciativa.Campos, nuevaIniciativa.IniciativaId);

                return RedirectToAction("MisIniciativas");
            }
            catch (Exception ex)
            {
                // Manejar errores
                Response.StatusCode = 500;
                return Content("Error al guardar la iniciativa en la base de datos: " + ex.Message);
            }
        }

        private string GenerarReferenciaUnica(int longitud = 8)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rand = new Random();
            string referencia;
            bool referenciaUnica;

            do
            {
                // Generar referencia aleatoria personalizada
                referencia = new string(Enumerable.Repeat(caracteres, longitud)
                    .Select(s => s[rand.Next(s.Length)]).ToArray());

                // Comprobar si la referencia ya existe en las tablas Campo y Subcampo
                referenciaUnica = !_context.Campos.Any(c => c.Ref == referencia) &&
                                  !_context.Subcampos.Any(sc => sc.Ref == referencia);
            }
            while (!referenciaUnica);

            return referencia;
        }

        private void GuardarCampos(List<CampoDTO> camposDto, int iniciativaId)
        {
            foreach (var campoDto in camposDto)
            {
                var nuevoCampo = new Campo
                {
                    Nombre = campoDto.NombreCampo,
                    CantidadArchivos = campoDto.CantidadArchivos,
                    IniciativaId = iniciativaId,
                    Ref = GenerarReferenciaUnica() // Asignar referencia única
                };
                _context.Campos.Add(nuevoCampo);
                _context.SaveChanges();

                GuardarSubcampos(campoDto.Subcampos, nuevoCampo.CampoId);
            }
        }

        private void GuardarSubcampos(List<SubcampoDTO> subcamposDto, int parentCampoId, int? parentSubcampoId = null)
        {
            foreach (var subcampoDto in subcamposDto)
            {
                var nuevoSubcampo = new Subcampo
                {
                    Nombre = subcampoDto.NombreSubcampo,
                    CantidadArchivos = subcampoDto.CantidadArchivos,
                    ParentCampoId = parentCampoId,
                    ParentSubcampoId = parentSubcampoId,
                    Ref = GenerarReferenciaUnica() // Asignar referencia única
                };
                _context.Subcampos.Add(nuevoSubcampo);
                _context.SaveChanges();

                if (subcampoDto.Subsubcampos != null && subcampoDto.Subsubcampos.Any())
                {
                    GuardarSubcampos(subcampoDto.Subsubcampos, parentCampoId, nuevoSubcampo.SubcampoId);
                }
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
