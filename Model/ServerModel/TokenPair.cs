namespace AuthApiSesh.Model{

    public class TokenPair
    {
        public string accessToken { get; set; }

        public string refreshToken { get; set; }

        public TokenPair(){}

        public TokenPair(string accessToken, string refreshToken){

            this.accessToken = accessToken;

            this.refreshToken = refreshToken;
        }
    }
}