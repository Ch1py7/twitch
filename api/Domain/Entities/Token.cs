﻿namespace api.Domain.Entities
{
    public class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public long ObtainedAt { get; set; }
    }
}
