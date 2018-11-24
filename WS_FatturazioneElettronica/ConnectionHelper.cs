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
        public static OleDbConnection GetOleDbConnectionJET()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["bubusetteteaccdbJET"].ConnectionString;
            var connection = new OleDbConnection(connectionString);
            return connection;
        }

        public static OleDbConnection GetOleDbConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["bubusetteteaccdb"].ConnectionString;
            var connection = new OleDbConnection(connectionString);
            return connection;
        }
    }
}
