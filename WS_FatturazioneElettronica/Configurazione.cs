using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_FatturazioneElettronica
{
    public class Configurazione
    {
        private double _IntervalSpedizione;
        private string _QueueFile;
        private string _SendedFile;

        public double IntervalSpedizione
        {
            get { return _IntervalSpedizione; }
        }

        public string QueueFile
        {
            get { return _QueueFile; }
        }

        public string SendedFile
        {
            get { return _SendedFile; }
        }
        
        public Configurazione()
        {
            var boolintSpediz = double.TryParse(ConfigurationManager.AppSettings["INTERVALLOATTIVITA"].ToString(),out var intSpediz);

            _IntervalSpedizione = boolintSpediz ? intSpediz : 3000;
            _QueueFile = ConfigurationManager.AppSettings["queueFile"] ?? string.Empty; 
            _SendedFile = ConfigurationManager.AppSettings["sendedFile"].ToString() ?? string.Empty;
        }

        
    }
}
