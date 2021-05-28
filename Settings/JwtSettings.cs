namespace AuthApiSesh.Settings{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int AccessTokenLifeTime { get; set; }
        public int ConfirmTokenLifeTime { get; set; }
        public int RefreshTokenLifeTime { get; set; }
    }
}