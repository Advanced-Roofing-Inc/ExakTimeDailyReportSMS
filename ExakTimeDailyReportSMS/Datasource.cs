using System;
using System.Data.SqlClient;

namespace ExakTimeSMSDailyJobReport
{
   class Datasource
   {
      private String connectionString { get; set; }

      public Datasource(String connectionString)
      {
         this.connectionString = connectionString;
      }

      public SqlConnection CreateConnection()
      {
         SqlConnection connection = new SqlConnection(this.connectionString);

         try
         {
            connection.Open();
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }

         return connection;
      }
   }
}
