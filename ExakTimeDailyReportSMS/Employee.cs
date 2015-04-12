using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ExakTimeSMSDailyJobReport
{
   class Employee
   {
      public String id { get; set; }
      public String name { get; set; }
      public String phone { get; set; }
      public String email { get; set; }
      public List<Project> projects { get; set; }
      public Double dailyHours { get; set; }
      public Double weeklyHours { get; set; }

      public static List<Employee> WithCellNumber(Datasource dataSource)
      {
         var employees = new List<Employee>();

         using (var connection = dataSource.CreateConnection())
         {
            try
            {
                var sqlQueryText = "SELECT EmployeeId, Name, Fax, eMailAddress FROM IMPORT_solomon_xhr_employee WHERE User6 in (0,'') and fax <> '' and Status = 'e'";

               SqlDataReader sqlReader = null;
               SqlCommand employeesSelect = new SqlCommand(sqlQueryText, connection);

               sqlReader = employeesSelect.ExecuteReader();

               while (sqlReader.Read())
               {
                  var reportDate = DateTime.Now.AddDays(-1);
                  var employee = new Employee();

                  // Employee ID
                  employee.id = sqlReader["EmployeeId"].ToString().Trim();
                  
                  // Employee's first and last name
                  employee.name = sqlReader["Name"].ToString().Trim();

                  // Cell phone number is in the fax column. Formatting is retarded, only grab 10 chars
                  employee.phone = sqlReader["Fax"].ToString().Substring(0, 10);

                  // Employee's email address - will fall back to this if no cell number
                  employee.email = sqlReader["eMailAddress"].ToString();

                  // Query for a list of projects
                  employee.projects = Project.GetProjectsForEmployeeForDate(dataSource, employee.id, reportDate);

                  employee.calculateWeeklyHours(dataSource);
                  employee.calculateDailyHours();
                  employees.Add(employee);
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

         return employees;
      }

      public static List<Employee> WithoutCellWithEmail(Datasource dataSource)
      {
         var employees = new List<Employee>();

         using (var connection = dataSource.CreateConnection())
         {
            try
            {
                var sqlQueryText = "SELECT EmployeeId, Name, Fax, eMailAddress FROM IMPORT_solomon_xhr_employee WHERE User6 = 1 AND eMailAddress <> '' and Status = 'e'";

               SqlDataReader sqlReader = null;
               SqlCommand employeesSelect = new SqlCommand(sqlQueryText, connection);

               sqlReader = employeesSelect.ExecuteReader();

               while (sqlReader.Read())
               {
                  // Date to fetch projects on
                  var reportDate = DateTime.Now.AddDays(-1);

                  var employee = new Employee();

                  // Employee ID
                  employee.id = sqlReader["EmployeeId"].ToString().Trim();

                  // Employee's first and last name
                  employee.name = sqlReader["Name"].ToString().Trim();

                  // Employee's email address - will fall back to this if no cell number
                  employee.email = sqlReader["eMailAddress"].ToString();

                  // Query for a list of projects
                  employee.projects = Project.GetProjectsForEmployeeForDate(dataSource, employee.id, reportDate);

                  if (employee.projects.Count > 0)
                  {
                     employee.calculateWeeklyHours(dataSource);
                     employee.calculateDailyHours();
                     employees.Add(employee);
                  }                 
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
         return employees;
      }

      public void calculateDailyHours()
      {
         Double hours = 0;

         for (int i = 0; i < this.projects.Count; i++)
         {
            hours += this.projects[i].hours;
         }

         this.dailyHours = hours;
      }

      public void calculateWeeklyHours(Datasource datasource)
      {
         Double hours = 0;
         DateTime today = DateTime.Today.AddDays(-1);
         DateTime startDate = new DateTime(today.Year, today.Month, today.Day);

         while (startDate.DayOfWeek != DayOfWeek.Monday)
         {
            startDate = startDate.AddDays(-1);
         }

         using (var connection = datasource.CreateConnection())
         {
            try
            {
               var commandText = "SELECT Hours FROM vw_timesummary WHERE EmployeeNumber = @employeeId AND WorkDate >= @startDate AND WorkDate <= @endDate";

               var hoursSelect = new SqlCommand(commandText, connection);
               hoursSelect.Parameters.AddWithValue("@employeeId", this.id);
               hoursSelect.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd 00:00:00"));
               hoursSelect.Parameters.AddWithValue("@endDate", today.ToString("yyyy-MM-dd 23:59:59"));

               SqlDataReader reader = hoursSelect.ExecuteReader();

               while (reader.Read())
               {
                  hours += Convert.ToDouble(reader["Hours"].ToString());
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

         this.weeklyHours = hours;
      }
   }
}
