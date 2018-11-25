using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_FatturazioneElettronica.Models
{
    public class Fatture
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int Anno           { get; set; }
        public int Numero         { get; set; }
        public int BuonoSpesa     { get; set; }
        public decimal TotaleProdotti { get; set; }
        public decimal ImportoFattura { get; set; }
        public decimal ImportoBuono   { get; set; }
        public decimal Iva            { get; set; }
        public decimal Imponibile     { get; set; }
        public int FatturaPA { get; set; }

        public static List<Fatture> ReadAll()
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                conn.Open();
                var result = conn.Query<Fatture>("Select * From Fatture").ToList();
                conn.Close();
                return result;
            }
        }

        public static Fatture Find(int id)
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);

                conn.Open();
                var result = conn.Query<Fatture>("Select * From Fatture WHERE ID = @Id", parameters).SingleOrDefault();
                conn.Close();
                return result;
            }
        }

        public static int Update(int Id)
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                try
                {
                    conn.Open();
                    string sqlQuery = "UPDATE Fatture SET FatturaPA = 1 WHERE ID=1;";
                    int rowsAffected = conn.Execute(sqlQuery);
                    return rowsAffected;
                }
                finally 
                {
                    conn.Close();
                }
            }
        }

    }
}
