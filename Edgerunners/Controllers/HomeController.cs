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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Collections;

namespace Edgerunners.Controllers
{
    public class HomeController : Controller
    {
        private IMongoCollection<Usuario> collectionUsuario;
        private IMongoCollection<LeaderBoard> collectionLeaderBoard;

        public HomeController(IMongoClient client)
        {
            var database = client.GetDatabase("app_edgerunners");
            collectionUsuario= database.GetCollection<Usuario>("Usuario");
            collectionLeaderBoard = database.GetCollection<LeaderBoard>("LeaderBoard");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Usuario usuario, string user)
        {
            if (user.Contains('@')){
                usuario.Email = user;
                var obj = collectionUsuario.Find(s => s.Email == usuario.Email && s.Password == GetHashedText(usuario.Password)).ToList();
                try
                {
                    var userlog = obj[0];
                    if (userlog.Admin == true)
                    {
                        HttpContext.Session.SetString("sesion", userlog.Email);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        TempData["Error"] = "Usuario o contraseña equivocados";
                        return View("Index");
                    }
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Usuario o contraseña equivocados";
                    return View("Index");
                }
            }
            else
            {
                usuario.Username = user;
                var obj = collectionUsuario.Find(s => s.Username == usuario.Username && s.Password == GetHashedText(usuario.Password)).ToList();
                try
                {
                    var userlog = obj[0];
                    if (userlog.Admin == true)
                    {
                        HttpContext.Session.SetString("sesion", userlog.Email);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        TempData["Error"] = "Usuario o contraseña equivocados";
                        return View("Index");
                    }
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Usuario o contraseña equivocados";
                    return View("Index");
                }
            }
        }

        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetString("sesion") != null)
            {
                ViewData["Sesion"] = HttpContext.Session.GetString("sesion");
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        public IActionResult Home()
        {
            if(HttpContext.Session.GetString("sesion") != null)
            {
                ViewData["Sesion"] = HttpContext.Session.GetString("sesion");
                var lista = collectionUsuario.Find(new BsonDocument()).ToList();
                return View(lista);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Home(string typeSearch, string searchString)
        {
            if(typeSearch == "user")
            {
                var obj = collectionUsuario.Find(s => s.Username == searchString).ToList();
                try
                {
                    var user = obj[0];
                    return View(obj);
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Home");
                }
            }
            else if(typeSearch == "name")
            {
                var obj = collectionUsuario.Find(s => s.Name == searchString).ToList();
                try
                {
                    var user = obj[0];
                    return View(obj);
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Home");
                }
            }
            else if (typeSearch == "email")
            {
                var obj = collectionUsuario.Find(s => s.Email == searchString).ToList();
                try
                {
                    var user = obj[0];
                    return View(obj);
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Home");
                }
            }
            else
            {
                return RedirectToAction("Home");
            }
        }

        public IActionResult Actividades()
        {
            if (HttpContext.Session.GetString("sesion") != null)
            {
                ViewData["Sesion"] = HttpContext.Session.GetString("sesion");
                var lista = collectionLeaderBoard.Find(new BsonDocument()).ToList().OrderByDescending(s=> s.Date);
                return View(lista);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Actividades(string typeSearch, string searchString, string sort)
        {
            if (typeSearch == "score")
            {
                try
                {
                    var obj = collectionLeaderBoard.Find(s => s.Score == int.Parse(searchString)).ToList();
                    var test = obj[0];
                    return View(obj);
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Puntaje no registrado";
                    return RedirectToAction("Actividades");
                }
            }
            else if (typeSearch == "email")
            {
                var obj = collectionLeaderBoard.Find(s => s.Email == searchString).ToList();
                try
                {
                    var test = obj[0];
                    return View(obj);
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Actividades");
                }
            }
            else
            {
                return RedirectToAction("Actividades");
            }
        }

        [HttpGet]
        public IActionResult Usuario(string email)
        {
            if (HttpContext.Session.GetString("sesion") != null)
            {
                ViewData["Sesion"] = HttpContext.Session.GetString("sesion");
                var obj = collectionLeaderBoard.Find(s => s.Email == email).ToList();
                try
                {
                    var user = obj[0];
                    ViewData["usuario"] = user.Email;
                    return View(obj);
                }
                catch(Exception e)
                {
                    TempData["error"] = "Usuario sin actividad.";
                    return RedirectToAction("Home");
                }
                
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("sesion") != null)
            {
                ViewData["Sesion"] = HttpContext.Session.GetString("sesion");
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Register(Usuario usuario, string confPassword)
        {

            Usuario document = new Usuario();

            if(usuario.Name == null)
            {
                TempData["name"] = "Campo requerido";
                return View();

            }else if (usuario.Username == null)
            {
                TempData["user"] = "Campo requerido";
                return View();
            }
            else if (usuario.Email == null)
            {
                TempData["email"] = "Campo requerido";
                return View();
            }
            else if(usuario.Password == null)
            {
                TempData["password"] = "Campo requerido";
                return View();
            }

            if(usuario.Password == confPassword)
            {
                var hash = GetHashedText(usuario.Password);
                document.Password = hash;
            }
            else
            {
                TempData["password"] = "Contraseña equivocada";
                return View();
            }

            document.Name = usuario.Name;
            document.Email = usuario.Email;
            document.Gender = usuario.Gender;
            document.Username = usuario.Username;
            document.Admin = usuario.Admin;

            collectionUsuario.InsertOne(document);
            TempData["Register"] = "Usuario Registrado Correctamente.";

            return View();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var _id = new ObjectId(id);
            if (HttpContext.Session.GetString("sesion") != null)
            {
                ViewData["Sesion"] = HttpContext.Session.GetString("sesion");
                var obj = collectionUsuario.Find(s => s.Id == _id).ToList();
                var user = obj[0];
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(Usuario newUserData, string confPassword)
        {
            var filter = Builders<Usuario>.Filter.Eq("email", newUserData.Email);
            var obj = collectionUsuario.Find(s => s.Email == newUserData.Email).ToList();
            var user = obj[0];

            if ((newUserData.Username == null) && (newUserData.Password == null))
            {
                TempData["edit"] = "Usuario actualizado.";
                return RedirectToAction("Home");
            }else if((newUserData.Username != null) && (newUserData.Password == null))
            {
                var updateUser = Builders<Usuario>.Update.Set("username", newUserData.Username);
                collectionUsuario.UpdateOne(filter, updateUser);
                TempData["edit"] = "Usuario actualizado.";
                return RedirectToAction("Home");
            }
            else if((newUserData.Username == null) && (newUserData.Password != null))
            {
                if (newUserData.Password == confPassword)
                {
                    var hash = GetHashedText(newUserData.Password);
                    var updatePassword = Builders<Usuario>.Update.Set("password", hash);
                    collectionUsuario.UpdateOne(filter, updatePassword);
                    TempData["edit"] = "Usuario actualizado.";
                    return RedirectToAction("Home");
                }
                else
                {
                    TempData["password"] = "Contraseña equivocada";
                    return View(user);
                }

            }
            else
            {
                var updateUser = Builders<Usuario>.Update.Set("username", newUserData.Username);
                collectionUsuario.UpdateOne(filter, updateUser);
                if (newUserData.Password == confPassword)
                {
                    var hash = GetHashedText(newUserData.Password);
                    var updatePassword = Builders<Usuario>.Update.Set("password", hash);
                    collectionUsuario.UpdateOne(filter, updatePassword);
                    return RedirectToAction("Home");
                }
                else
                {
                    TempData["password"] = "Contraseña equivocada";
                    return View(user);
                }
            }
        }

        public IActionResult Delete(string id)
        {
            var _id = new ObjectId(id);
            var filter = Builders<Usuario>.Filter.Eq("_id", _id);
            collectionUsuario.DeleteOne(filter);
            TempData["delete"] = "Usuario eliminado.";
            return RedirectToAction("Home");
        }

        public IActionResult Salir()
        {
            HttpContext.Session.Remove("sesion");
            return RedirectToAction("Index");
        }

        private string GetHashedText(string Text)
        {
            var encrypted = Encoding.UTF8.GetBytes(Text);
            Sha256Digest hash = new Sha256Digest();
            hash.BlockUpdate(encrypted, 0, encrypted.Length);
            byte[] compArr = new byte[hash.GetDigestSize()];
            hash.DoFinal(compArr, 0);
            
            return BitConverter.ToString(compArr).Replace("-","").ToLower();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
