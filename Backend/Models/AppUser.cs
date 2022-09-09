using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Backend.Models
{
    public class AppUser
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("fullName")]
        public string FullName { get; set; }

        [BsonElement("accessToken")]
        public string AccessToken { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }


    }
}
