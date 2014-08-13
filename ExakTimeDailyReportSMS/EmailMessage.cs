using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ExakTimeDailyReportSMS
{
   class EmailMessage
   {
      public String To { get; set; }
      public String Subject { get; set; }
      public String Body { get; set; }

      public void Send()
      {
         var smtp = new SmtpClient
         {
            Host = ConfigurationManager.AppSettings["SmtpServer"],
            Port = 587,
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 20000
         };

         using (var message = new MailMessage(ConfigurationManager.AppSettings["SmtpFromAddress"], this.To)
         {
            Subject = this.Subject,
            Body = this.Body
         })
         {
            try
            {
               smtp.Send(message);
            }
            catch (Exception e)
            {
               Console.WriteLine(e.ToString());
            }
         }

         Console.WriteLine("sending email...");
      }
   }
}
