using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AuthApiSesh.Settings;
using AuthApiSesh.Service;
using AuthApiSesh.Model;
using System.Threading.Tasks;
using System;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;

namespace AuthApiSesh.Service
{
    public class EmailService : IEmailService{

        private MailSettings settings{get; }
        
        public EmailService(IOptions<MailSettings> mailsettings)
        {
            settings = mailsettings.Value;
        }

        public async Task SendVerificationAsync(int code, User user)
        {
            try
            {
                var Message = new MimeMessage();
                Message.From.Add(new MailboxAddress(settings.DisplayName,settings.Mail));
                Message.To.Add(new MailboxAddress(user.username,user.email));
                Message.Subject = "Email verification";
                Message.Body = new TextPart(TextFormat.Plain){ Text = code.ToString() };

                using(var client = new SmtpClient()){
                    // client.ServerCertificateValidationCallback = (s, c, h, e) =>true;
                    client.CheckCertificateRevocation = false;

                    await client.ConnectAsync(settings.Host,settings.Port,SecureSocketOptions.Auto);

                    await client.AuthenticateAsync(settings.Mail,settings.Password);
                    
                    await client.SendAsync(Message);

                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception e)
            {
                
                throw new InvalidOperationException(e.Message);
            }
        }


        public void SendVerification(int code, User user)
        {
            try
            {
                var Message = new MimeMessage();
                Message.From.Add(new MailboxAddress(settings.DisplayName,settings.Mail));
                Message.To.Add(new MailboxAddress(user.username,user.email));
                Message.Subject = "Email verification";
                Message.Body = new TextPart(TextFormat.Plain){ Text = code.ToString() };

                using(var client = new SmtpClient()){
                    client.ServerCertificateValidationCallback = (s, c, h, e) =>true;

                    client.Connect(settings.Host,settings.Port);

                    client.Authenticate(settings.Mail,settings.Password);
                    
                    client.Send(Message);

                    client.Disconnect(true);
                }

            }
            catch (Exception e)
            {
                
                throw new InvalidOperationException(e.Message);
            }
        }
    }

}