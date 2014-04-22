using System;
using System.Configuration;
using Twilio;

namespace ExakTimeSMSDailyJobReport
{
   class TwilioMessage
   {
      public String phoneNumber { get; set; }
      public String message { get; set; }
      private String _accountSid { get; set; }
      private String _authToken { get; set; }
      private String _fromPhoneNumber { get; set; }

      public TwilioMessage(String phoneNumber, String message)
      {
         this._accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
         this._authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
         this._fromPhoneNumber = ConfigurationManager.AppSettings["TwilioFromPhoneNumber"];

         // Twilio requires the country code
         this.phoneNumber = "+1" + phoneNumber;
         this.message = message;
      }

      public void Send()
      {
         TwilioRestClient twilio = new TwilioRestClient(this._accountSid, this._authToken);

         Console.WriteLine("Sending message to {0}", this.phoneNumber);
         var message = twilio.SendSmsMessage(this._fromPhoneNumber, this.phoneNumber, this.message, "");
         Console.WriteLine("{0} {1}", message.Sid, message.Status);
      }
   }
}
