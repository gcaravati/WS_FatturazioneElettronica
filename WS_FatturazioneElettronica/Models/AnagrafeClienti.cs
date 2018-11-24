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
    public class AnagrafeClienti
    {
        public int Id { get; set; }
        public string CodFisc { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        public string Cap { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Cellulare { get; set; }
        public string CartaIdentita { get; set; }
        public DateTime DataScadenza { get; set; }
        public string CIComune { get; set; }
        public DateTime DataNascita { get; set; }
        public string ComuneNascita { get; set; }
        public int Beneficenza { get; set; }
        public int BitCancellato { get; set; }

        public static List<AnagrafeClienti> ReadAll()
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                conn.Open();
                var result = conn.Query<AnagrafeClienti>("Select * From AnagrafeClienti").ToList();
                conn.Close();
                return result;
            }
        }

        public static AnagrafeClienti Find(int id)
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);

                conn.Open();
                var result = conn.Query<AnagrafeClienti>("Select * From AnagrafeClienti WHERE Id = @Id", parameters).SingleOrDefault();
                conn.Close();
                return result;
            }
        }



    }
}
