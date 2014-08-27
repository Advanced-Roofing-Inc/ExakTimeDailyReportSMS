using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ExakTimeDailyReportSMS
{
    class EmailMessage
    {
        public String To { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }

        public void Send()
        {
            SmtpClient smtpClient = new SmtpClient();
            
            MailMessage message = new MailMessage();
            message.Subject = Subject;
            message.Body = Body;
            message.To.Add(new MailAddress(To));

            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("sending email to {0}...", To);
        }
    }
}