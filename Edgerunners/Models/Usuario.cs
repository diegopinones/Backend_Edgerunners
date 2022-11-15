using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable disable

namespace Edgerunners.Models
{
    public class Usuario
    {
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("E-mail")]
        public string Email { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("phoneNum")]
        public string Phone { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("administrator")]
        public bool Admin { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }
    }
}