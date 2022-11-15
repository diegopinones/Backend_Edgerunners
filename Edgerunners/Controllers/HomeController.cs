using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Edgerunners.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization;

namespace Edgerunners.Controllers
{
    public class HomeController : Controller
    {
        private IMongoCollection<Usuario> collectionUsuario;

        public HomeController(IMongoClient client)
        {
            var database = client.GetDatabase("app_edgerunners");
            collectionUsuario= database.GetCollection<Usuario>("Usuario");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Usuario usuario)
        {
            var obj = collectionUsuario.Find(s => s.Username == usuario.Username && s.Password == usuario.Password && s.Admin == true);
            if (obj != null)
            {
                HttpContext.Session.SetString("username", usuario.Username);
                return RedirectToAction("Home");
            }
            else
            {
                TempData["Error"] = "Usuario o contraseña equivocados";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Home()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[HttpGet]
        //public IEnumerable<Usuario> Get()
        //{
        //    //var client = new MongoClient("mongodb+srv://diegopinones:edgerunners_db@clusteredgerunners.zogbhm3.mongodb.net/?retryWrites=true&w=majority");

        //    return collectionUsuario.Find(s => s.Username == "admin").ToList();
        //}
    }
}
