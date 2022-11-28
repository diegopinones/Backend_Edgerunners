using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Edgerunners.Models
{
	public class LeaderBoard
	{
        public ObjectId Id { get; set; }

        [BsonElement("email")]
		public string Email { get; set; }

        [BsonElement("score")]
        public int Score { get; set; }

        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
    }
}

