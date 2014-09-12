using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using ExakTimeDailyReportSMS;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ExakTimeSMSDailyJobReport
{
   class Program
   {
      static void Main(string[] args)
      {
         var dataSource = new Datasource(ConfigurationManager.ConnectionStrings["ExakTime"].ConnectionString);

         // Send a text message to employees that have a cell number
         var employeesWithCellNumber = Employee.WithCellNumber(dataSource);

         foreach (var employee in employeesWithCellNumber)
         {
            foreach (var project in employee.projects)
            {
               var message = String.Empty;
               message += String.Format("{0}: {1:M/d/yy}\n", employee.name, project.date);
               message += String.Format("{0}\n{1} hours\n\n", project.description.Trim(), project.hours);

               var twilioMessage = new TwilioMessage(employee.phone, message);

               twilioMessage.Send();
            }
         }

         // Send an email to employees that do not have a cell number
         var employeesWithoutCellWithEmail = Employee.WithoutCellWithEmail(dataSource);

         foreach (var employee in employeesWithoutCellWithEmail)
         {
            var message = String.Empty;

            foreach (var project in employee.projects)
            {
               message = String.Format("{0}: {1:M/d/yy}\n", employee.name, project.date);
               message += String.Format("{0}\n{1} hours", project.description.Trim(), project.hours);
            }

            var emailMessage = new EmailMessage
            {
               To = employee.email,
               Subject = "Daily Project Hours Report",
               Body = message
            };

            emailMessage.Send();
         }
      }
   }
}
