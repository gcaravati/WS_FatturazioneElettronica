using Dapper;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_FatturazioneElettronica.Models
{
    public class Prodotti
    {
        public int ID { get; set; }
        public int CodCliente { get; set; }
        public int CodProdotto { get; set; }
        public string TipoProdotto { get; set; }
        public DateTime DataArrivo { get; set; }
        public char Sesso { get; set; }
        public string Articolo { get; set; }
        public string Descrizione { get; set; }
        public string Taglia { get; set; }
        public decimal Prezzo { get; set; }
        public decimal PrezzoVenduto { get; set; }
        public DateTime DataVendita { get; set; }
        public int Stato { get; set; }
        public int Pagato { get; set; }
        public DateTime DataInserimento { get; set; }
        public int Stampato { get; set; }
        public DateTime DataComunicazioneReso { get; set; }
        public DateTime DataReso { get; set; }
        public string Note { get; set; }
        public int Numero { get; set; }
        public int NumeroVenduti { get; set; }
        public DateTime DataBeneficenza { get; set; }

        public static List<Prodotti> ReadAll()
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                conn.Open();
                var result = conn.Query<Prodotti>("Select * From Prodotti").ToList();
                conn.Close();
                return result;
            }
        }

        public static Prodotti Find(int id)
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);

                conn.Open();
                var result = conn.Query<Prodotti>("Select * From Prodotti WHERE Id = @Id", parameters).SingleOrDefault();
                conn.Close();
                return result;
            }
        }

        public static List<Prodotti> GetProdottiPerFattura(int idFattura)
        {
            using (OleDbConnection conn = ConnectionHelper.GetOleDbConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Pagato", idFattura);

                conn.Open();
                var result = conn.Query<Prodotti>("Select * From Prodotti WHERE Pagato = @Id", parameters).ToList();
                conn.Close();
                return result;
            }
        }
    }
}
