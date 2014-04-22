using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace ExakTimeSMSDailyJobReport
{
   class Program
   {
      static void Main(string[] args)
      {
         Datasource dataSource = new Datasource(ConfigurationManager.ConnectionStrings["ExakTime"].ConnectionString);

         // Send a text message to employees that have a cell number
         List<Employee> employeesWithCellNumber = Employee.WithCellNumber(dataSource);

         foreach (var employee in employeesWithCellNumber)
         {
            foreach (var project in employee.projects)
            {
               String message = String.Empty;
               message = String.Format("{0}: {1:M/d/yy}\n", employee.name, project.date);
               message += String.Format("{0}\n{1} hours", project.description.Trim(), project.hours);

               TwilioMessage twilioMessage = new TwilioMessage(employee.phone, message);

               twilioMessage.Send();
            }
         }

         // TODO: Send an email to employees with no phone number set
         // ...
      }
   }
}