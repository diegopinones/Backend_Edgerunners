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

namespace Edgerunners.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private IMongoCollection<Usuario> collectionUsuario;

        public UserController(IMongoClient client)
        {
            var database = client.GetDatabase("app_edgerunners");
            collectionUsuario = database.GetCollection<Usuario>("Usuario");
        }

        [HttpGet]
        public IEnumerable<Usuario> Get()
        {
            //var client = new MongoClient("mongodb+srv://diegopinones:edgerunners_db@clusteredgerunners.zogbhm3.mongodb.net/?retryWrites=true&w=majority");

            return collectionUsuario.Find(s => s.Username == "admin").ToList();
        }

        public IActionResult Home
    }
}