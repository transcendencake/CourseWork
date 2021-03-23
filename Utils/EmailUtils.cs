using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace CourseWork.Utils
{
    public class EmailService
    {
        private IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public async Task SendEmailsAsync(IEnumerable<string> emails, string message)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("Fun fiction", "pav.ziaz@yandex.ru"));
            var networkAdresses = emails.Select(e => new MailboxAddress("", e));
            mail.To.AddRange(networkAdresses);
            mail.Subject = "Book updates";
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = message };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465);
                await client.AuthenticateAsync(configuration["Mail:login"], configuration["Mail:password"]);
                await client.SendAsync(mail);
                await client.DisconnectAsync(true); 
            }
        }
    }
}
