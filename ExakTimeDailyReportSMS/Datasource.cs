using System;
using System.Data.SqlClient;

namespace ExakTimeSMSDailyJobReport
{
   class Datasource
   {
      private String _connectionString { get; set; }

      public Datasource(String connectionString)
      {
         this._connectionString = connectionString;
      }

      public SqlConnection CreateConnection()
      {
         var connection = new SqlConnection(this._connectionString);

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
