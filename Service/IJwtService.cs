using System.Threading.Tasks;
using AuthApiSesh.Model;
namespace AuthApiSesh.Service
{
    public interface IJwtService
    {
        string GetAccessToken(User User);//
        Task<string> GetAccessTokenAsync(User User);
        string GetAccessToken(long id, string username);//
        Task<string> GetAccessTokenAsync(long id, string username);
        string GetConfirmToken(long id);
        string GetConfirmToken(User User);
        Task<string> GetConfirmTokenAsync(long id);
        bool validateToken(string token);
        Task<string> GetRefreshTokenAsync(User user);
        string GetRefreshToken(User user);
        Task<string> GetRefreshTokenAsync(long userId, string username);
        string GetRefreshToken(long userId, string username);

    }
}