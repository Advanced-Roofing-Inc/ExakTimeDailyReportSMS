using System;

namespace ExakTimeSMSDailyJobReport
{
   class TwilioMessage
   {
      public String phoneNumber { get; set; }
      public String message { get; set; }

      public TwilioMessage(String phoneNumber, String message)
      {
         // Twilio requires the country code
         this.phoneNumber = "001" + phoneNumber;

         this.message = message;
      }
   }
}
