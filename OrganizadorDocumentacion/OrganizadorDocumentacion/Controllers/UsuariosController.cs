using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using OrganizadorDocumentacion.Models;

namespace OrganizadorDocumentacion.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DocnizerContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public UsuariosController(ILogger<HomeController> logger, DocnizerContext context, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
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

            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
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

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Clave,TipoUsuario")] Usuario usuario)
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

            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nombre,Clave,TipoUsuario")] Usuario usuario)
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

            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Eliminar iniciativas y archivos asociados al usuario
                var homeController = new HomeController(_logger,_context,_configuration); // Pasar el logger y configuration al HomeController
                var resultadoEliminarIniciativas = await homeController.EliminarIniciativasPorUsuario(id);

                    // Si la eliminación de iniciativas fue exitosa, eliminar el usuario
                    _context.Usuarios.Remove(usuario);
                    await _context.SaveChangesAsync();
  
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
