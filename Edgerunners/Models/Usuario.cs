using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable disable

namespace Edgerunners.Models
{
    public class Usuario
    {
        public ObjectId Id { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("administrator")]
        public bool Admin { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }
    }
}