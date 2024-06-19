using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementService.Models
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Message(IEnumerable <string> to, string subject,string content ) 
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;

            //if (to == null || !to.Any())
            //{
            //    throw new ArgumentNullException(nameof(to), "Recipient email addresses cannot be null or empty.");
            //}

            //To = to.Select(email => new MailboxAddress(email, email)).ToList();
            //Subject = subject ?? throw new ArgumentNullException(nameof(subject), "Subject cannot be null.");
            //Content = content ?? throw new ArgumentNullException(nameof(content), "Content cannot be null.");
        }
    }
}
