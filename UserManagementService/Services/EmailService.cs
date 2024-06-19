using MimeKit;
using MailKit.Net.Smtp;
using UserManagementService.Models;

namespace UserManagementService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfigration _emailConfig;

        public EmailService(EmailConfigration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Nitish Kumar", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };
            return emailMessage;
        }
        //private MimeMessage CreateEmailMessage(Message message)
        //{
        //    if (message == null)
        //        throw new ArgumentNullException(nameof(message), "Message cannot be null.");

        //    if (_emailConfig.From == null)
        //        throw new ArgumentNullException(nameof(_emailConfig.From), "Sender email address cannot be null.");

        //    var emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress("Nitish Kumar", _emailConfig.From));

        //    if (message.To == null || !message.To.Any())
        //        throw new ArgumentNullException(nameof(message.To), "Recipient email addresses cannot be null or empty.");

        //    emailMessage.To.AddRange(message.To);
        //    emailMessage.Subject = message.Subject;
        //    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
        //    {
        //        Text = message.Content
        //    };
        //    return emailMessage;
        //}

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
