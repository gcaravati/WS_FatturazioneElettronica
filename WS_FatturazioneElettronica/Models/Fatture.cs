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

        public static List<Fatture> ReadAll()
        {
            using (OleDbConnection db = ConnectionHelper.GetOleDbConnection())
            {
                return db.Query<Fatture>("Select * From Fatture").ToList();
            }
        }

        public static Fatture Find(int id)
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);

                conn.Open();
                var result = conn.Query<Fatture>("Select * From Fatture WHERE Id = @Id", parameters).SingleOrDefault();
                conn.Close();
                return result;
            }
        }

    }
}
