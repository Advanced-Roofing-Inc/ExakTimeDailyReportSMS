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

      public static List<Employee> WithCellNumber(Datasource dataSource)
      {
         List<Employee> employees = new List<Employee>();

         using (SqlConnection connection = dataSource.CreateConnection())
         {
            try
            {
               String sqlQueryText = "SELECT EmployeeId, Name, Fax, eMailAddress FROM IMPORT_solomon_xhr_employee WHERE User6 = '' AND fax <> ''";

               SqlDataReader sqlReader = null;
               SqlCommand employeesSelect = new SqlCommand(sqlQueryText, connection);

               sqlReader = employeesSelect.ExecuteReader();

               while (sqlReader.Read())
               {
                  Employee employee = new Employee();

                  // Employee ID
                  employee.id = sqlReader["EmployeeId"].ToString().Trim();
                  
                  // Employee's first and last name
                  employee.name = sqlReader["Name"].ToString().Trim();

                  // Cell phone number is in the fax column. Formatting is retarded, only grab 10 chars
                  employee.phone = sqlReader["Fax"].ToString().Substring(0, 10);

                  // Employee's email address - will fall back to this if no cell number
                  employee.email = sqlReader["eMailAddress"].ToString();

                  // Query for a list of projects
                  employee.projects = Project.GetProjectsForEmployeeForDate(dataSource, employee.id, new DateTime(2014, 4, 18));

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
   }
}
