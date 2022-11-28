using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;
using Microsoft.Extensions.Logging;
using Edgerunners.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Edgerunners.Controllers
{
    [Route("api/app")]
    public class AppApiController : Controller
    {

        private IMongoCollection<Usuario> collectionUsuario;
        private IMongoCollection<LeaderBoard> collectionLeaderBoard;

        public static int EMPTY_NAME = 100;
        public static int EMPTY_PASSWORD = 101;
        public static int EMPTY_MAIL = 102;
        public static int EMPTY_USERNAME = 103;
        public static int EMPTY_GENDER = 104;
        public static int WRONG_USER = 105;

        public AppApiController(IMongoClient client)
        {
            var database = client.GetDatabase("app_edgerunners");
            collectionUsuario = database.GetCollection<Usuario>("Usuario");
            collectionLeaderBoard = database.GetCollection<LeaderBoard>("LeaderBoard");
        }

        [HttpGet]
        public IEnumerable<Usuario> Get()
        {
            var lista = collectionUsuario.Find(new BsonDocument()).ToList();

            return lista;
        }

        [HttpPost("usuario")]
        public IActionResult Register([FromBody] Usuario usuario)
        {

            Usuario document = new Usuario();

            if (usuario.Name == null)
            {
                return BadRequest(new
                {
                    error = "Empty name",
                    code = EMPTY_NAME
                });
            }
            else if (usuario.Username == null)
            {
                return BadRequest(new
                {
                    error = "Empty username",
                    code = EMPTY_USERNAME
                });
            }
            else if (usuario.Email == null)
            {
                return BadRequest(new
                {
                    error = "Empty email",
                    code = EMPTY_MAIL
                });
            }
            else if (usuario.Password == null)
            {
                return BadRequest(new
                {
                    error = "Empty password",
                    code = EMPTY_PASSWORD
                });
            }
            else if (usuario.Gender == null)
            {
                return BadRequest(new
                {
                    error = "Empty gender",
                    code = EMPTY_GENDER
                });
            }
            else
            {
                var hash = GetHashedText(usuario.Password);
                document.Password = hash;
            }

            document.Name = usuario.Name;
            document.Email = usuario.Email;
            document.Gender = usuario.Gender;
            document.Username = usuario.Username;

            collectionUsuario.InsertOne(document);

            return Ok();
        }

        //Login
        //Recibe mail, contraseña
        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario usuario)
        {
            var obj = collectionUsuario.Find(s => s.Email == usuario.Email && s.Password == GetHashedText(usuario.Password)).ToList();
            try
            {
                var userlog = obj[0];
                return Ok(usuario);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    error = "Wrong username or password",
                    code = WRONG_USER
                });
            }
        }


        //Crear calificacion
        //Recibe mail, puntuacion
        [HttpPost("score")]
        public IActionResult Score([FromBody] LeaderBoard actividad)
        {
            LeaderBoard document = new LeaderBoard();

            document.Email = actividad.Email;
            document.Score = actividad.Score;
            document.Date = DateTime.Now;
            collectionLeaderBoard.InsertOne(document);
            return Ok();
        }

        private string GetHashedText(string Text)
        {
            var encrypted = Encoding.UTF8.GetBytes(Text);
            Sha256Digest hash = new Sha256Digest();
            hash.BlockUpdate(encrypted, 0, encrypted.Length);
            byte[] compArr = new byte[hash.GetDigestSize()];
            hash.DoFinal(compArr, 0);

            return BitConverter.ToString(compArr).Replace("-", "").ToLower();
        }
    }
}

