using Microsoft.Extensions.Configuration;
using ShippingApp.Models;
using System.Net.Mail;
using System.Net;

namespace ShippingApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public ResponseModel SendEmail(EmailModel email)
        {
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            // set the sender and recipient email addresses
            message.From = new MailAddress("mail@Shipmentapp.chicmic.co.in");
            message.To.Add(new MailAddress(email.email));

            // set the subject and body of the email
            message.Subject = email.Subject;
            message.Body = email.body;

            // create a new SmtpClient object
            SmtpClient client = new SmtpClient();

            string adminEmail = configuration.GetSection("EmailCred:Email").Value!;
            string adminPassword = configuration.GetSection("EmailCred:password").Value!;

            // set the SMTP server credentials and port
            client.Credentials = new NetworkCredential(adminEmail, adminPassword);
            client.Host = "mail.chicmic.co.in";
            client.Port = 587;
            client.EnableSsl = true;
            // send the email
            client.Send(message);

            return new ResponseModel("Verification Email Sent");
        }
    }
}
