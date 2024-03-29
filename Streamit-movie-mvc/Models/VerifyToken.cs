﻿using MongoDB.Bson;

namespace Movies.Business.users
{
    public class VerifyToken
    {
        public ObjectId Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Token { get; set; }
        public int? Code { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}
