using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS_FatturazioneElettronica.Models;

namespace WS_FatturazioneElettronica
{
    public class ConnectionHelper
    {
        public static SqlConnection GetSqlConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["bubusetteteaccdb"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static OleDbConnection GetOleDbConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["bubusetteteaccdb"].ConnectionString;
            var connection = new OleDbConnection(connectionString);
            connection.Open();
            return connection;
        }

        
    }
}
