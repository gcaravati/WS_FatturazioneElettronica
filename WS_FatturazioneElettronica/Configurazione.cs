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
        private string _IdPaese;
        private string _IdCodice;
        private string _Email;
        private string _Telefono;
        private string _PartitaIva;
        private string _Denominazione;
        private decimal _AliquotaIVA;

        private Sede _Sede;
        private IscrizioneREA _IscrizioneREA;


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
        public string IdPaese
        {
            get { return _IdPaese; }
        }
        public string IdCodice
        {
            get { return _IdCodice; }
        }
        public string Email
        {
            get { return _Email; }
        }
        public string Telefono
        {
            get { return _Telefono; }
        }
        public string PartitaIva
        {
            get { return _PartitaIva; }
        }
        public string Denominazione
        {
            get { return _Denominazione; }
        }
        public decimal AliquotaIVA
        {
            get { return _AliquotaIVA; }
        }

        public Sede Sede
        {
            get { return _Sede; }
        }
        public IscrizioneREA IscrizioneREA
        {
            get { return _IscrizioneREA; }
        }


        public Configurazione()
        {
            var boolintSpediz = double.TryParse(ConfigurationManager.AppSettings["INTERVALLOATTIVITA"].ToString(), out var intSpediz);

            _IntervalSpedizione = boolintSpediz ? intSpediz : 3000;
            _QueueFile = ConfigurationManager.AppSettings["queueFile"] ?? string.Empty;
            _SendedFile = ConfigurationManager.AppSettings["sendedFile"] ?? string.Empty;
            _IdPaese = ConfigurationManager.AppSettings["IdPaese"] ?? string.Empty;
            _IdCodice = ConfigurationManager.AppSettings["IdCodice"] ?? string.Empty;
            _Email = ConfigurationManager.AppSettings["Email"] ?? string.Empty;
            _Telefono = ConfigurationManager.AppSettings["Telefono"] ?? string.Empty;
            _PartitaIva = ConfigurationManager.AppSettings["PartitaIva"] ?? string.Empty;
            _Denominazione = ConfigurationManager.AppSettings["Denominazione"] ?? string.Empty;

            var boo_AliquotaIVA = decimal.TryParse(ConfigurationManager.AppSettings["AliquotaIVA"].ToString(), out var iva);
            _AliquotaIVA = boo_AliquotaIVA ? iva : 22.00m;

            _Sede = new Sede();
            _IscrizioneREA = new IscrizioneREA();
        }
    }
    public class Sede
    {
        private string _Indirizzo;
        private string _CAP;
        private string _Comune;
        private string _Provincia;
        private string _Nazione;

        public string Indirizzo
        {
            get { return _Indirizzo; }
        }
        public string CAP
        {
            get { return _CAP; }
        }
        public string Comune
        {
            get { return _Comune; }
        }
        public string Provincia
        {
            get { return _Provincia; }
        }
        public string Nazione
        {
            get { return _Nazione; }
        }

        public Sede()
        {
            _Indirizzo = ConfigurationManager.AppSettings["Indirizzo"] ?? string.Empty;
            _CAP = ConfigurationManager.AppSettings["CAP"] ?? string.Empty;
            _Comune = ConfigurationManager.AppSettings["Comune"] ?? string.Empty;
            _Provincia = ConfigurationManager.AppSettings["Provincia"] ?? string.Empty;
            _Nazione = ConfigurationManager.AppSettings["Nazione"] ?? string.Empty;
        }
    }

    public class IscrizioneREA
    {
        private string _Ufficio;
        private string _NumeroREA;
        private string _SocioUnico;
        private string _StatoLiquidazione;

        public string Ufficio
        {
            get { return _Ufficio; }
        }
        public string NumeroREA
        {
            get { return _NumeroREA; }
        }
        public string SocioUnico
        {
            get { return _SocioUnico; }
        }
        public string StatoLiquidazione
        {
            get { return _StatoLiquidazione; }
        }
        public IscrizioneREA()
        {
            _Ufficio = ConfigurationManager.AppSettings["Ufficio"] ?? string.Empty;
            _NumeroREA = ConfigurationManager.AppSettings["NumeroREA"] ?? string.Empty;
            _SocioUnico = ConfigurationManager.AppSettings["SocioUnico"] ?? string.Empty;
            _StatoLiquidazione = ConfigurationManager.AppSettings["StatoLiquidazione"] ?? string.Empty;
        }
    }
}
