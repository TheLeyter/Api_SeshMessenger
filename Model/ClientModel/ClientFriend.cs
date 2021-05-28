namespace AuthApiSesh.ClientModel
{
    public class ClientFriends
    {

        public int User1 { get; set; }
        public int User2 { get; set; }

        public ClientFriends() { }
        public ClientFriends(int User1, int User2)
        {
            this.User1 = User1;
            this.User2 = User2;
        }
    }
}