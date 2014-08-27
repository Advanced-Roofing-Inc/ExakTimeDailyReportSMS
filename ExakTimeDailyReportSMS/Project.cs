using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ExakTimeSMSDailyJobReport
{
   class Project
   {
      public DateTime date { get; set; }
      public String number { get; set; }
      public String description { get; set; }
      public Double hours { get; set; }

      public static List<Project> GetProjectsForEmployeeForDate(Datasource dataSource, String employeeId, DateTime date)
      {
         var projects = new List<Project>();

         using (SqlConnection connection = dataSource.CreateConnection())
         {
            try
            {
               var commandText = "SELECT WorkDate, JobsiteNumber, JobsiteName, Hours FROM vw_timesummary WHERE EmployeeNumber = @employeeId AND WorkDate = @date";
               
               var projectsSelect = new SqlCommand(commandText, connection);
               projectsSelect.Parameters.AddWithValue("@employeeId", employeeId);
               projectsSelect.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd 00:00:00"));

               SqlDataReader reader = null;
               reader = projectsSelect.ExecuteReader();

               while (reader.Read())
               {
                  var project = new Project();
                  project.date = Convert.ToDateTime(reader["WorkDate"].ToString());
                  project.number = reader["JobsiteNumber"].ToString();
                  project.description = reader["JobsiteName"].ToString();
                  project.hours = Convert.ToDouble(reader["Hours"].ToString());

                  projects.Add(project);
               }
            }
            catch (Exception e)
            {
               Console.WriteLine(e.ToString());
            }
            finally
            {
               connection.Close();
            }
         }         

         return projects;
      }
   }
}
