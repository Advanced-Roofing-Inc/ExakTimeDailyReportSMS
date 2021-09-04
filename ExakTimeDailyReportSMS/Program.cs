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
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

         Console.WriteLine("Running ExakTimeDailyReportSMS");
         var dataSource = new Datasource(ConfigurationManager.ConnectionStrings["ExakTime"].ConnectionString);

         // Send a text message to employees that have a cell number
         var employeesWithCellNumber = Employee.WithCellNumber(dataSource);

         Console.WriteLine("Found {0} employees with a cell number set.", employeesWithCellNumber.Count);

         foreach (var employee in employeesWithCellNumber)
         {
            Console.WriteLine(employee.weeklyHours);
            foreach (var project in employee.projects)
            {
               var message = String.Empty;
               message += String.Format("{0}: {1:M/d/yy}\n", employee.name, project.date);
               message += String.Format("{0}\n{1} hours\n\n", project.description.Trim(), project.hours);
               message += String.Format("{0} hrs today, {1} hours this week", employee.dailyHours, employee.weeklyHours);

               var twilioMessage = new TwilioMessage(employee.phone, message);
               twilioMessage.Send();
               //Console.Write(message);
            }
         }

         // Send an email to employees that do not have a cell number
         var employeesWithoutCellWithEmail = Employee.WithoutCellWithEmail(dataSource);

         Console.WriteLine("Found {0} employees without a cell number, but with an email address.", employeesWithoutCellWithEmail.Count);

         foreach (var employee in employeesWithoutCellWithEmail)
         {
            var message = String.Empty;

            foreach (var project in employee.projects)
            {
               message = String.Format("{0}: {1:M/d/yy}\n", employee.name, project.date);
               message += String.Format("{0}\n{1} hours\n", project.description.Trim(), project.hours);
               message += String.Format("{0} hrs today, {1} hours this week", employee.dailyHours, employee.weeklyHours);
            }

            var emailMessage = new EmailMessage
            {
               To = employee.email,
               Subject = "Daily Project Hours Report",
               Body = message
            };

            emailMessage.Send();
            //Console.Write(message);
         }

         //Console.ReadKey();
      }
   }
}
