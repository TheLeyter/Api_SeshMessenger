using System.Threading.Tasks;
using AuthApiSesh.Model;

namespace AuthApiSesh.Service
{
    public interface IEmailService
    {
        Task SendVerificationAsync(int code, User user);
        void SendVerification(int code, User user);
    }
}