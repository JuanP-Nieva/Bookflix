// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Bookflix.Models.Mails
{
    public class EmailSender
    {        
        public async Task Execute(string From, string FromEmail, string To, string ToEmail, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient("");
            var from = new EmailAddress(FromEmail, From);
            var to = new EmailAddress(ToEmail, To);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
       }
    }
}