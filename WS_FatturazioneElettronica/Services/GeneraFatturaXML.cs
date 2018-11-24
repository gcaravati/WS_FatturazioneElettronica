using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using WS_FatturazioneElettronica.Models;

namespace WS_FatturazioneElettronica.Services
{
    public partial class GeneraFatturaXML : ServiceBase
    {
        private Timer _timer = new Timer();
        private double _Interval = 30000;
        private string _Queue;
        private string _Sended;
        public bool look;

        public GeneraFatturaXML()
        {
            InitializeComponent();
            this.look = false;
            this._Interval = Program.Config.IntervalSpedizione;
            this._Queue = Program.Config.QueueFile;
            this._Sended = Program.Config.SendedFile;
            this._timer.Elapsed += new ElapsedEventHandler(OnProcess);
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            Program.WriteLog(this.ServiceName, " OnStart:" + System.DateTime.Now.ToString());
            this._timer.Interval = _Interval;
            this._timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Program.WriteLog(this.ServiceName, " OnStop:" + System.DateTime.Now.ToString());
            this._timer.Enabled = false;
        }

        private void OnProcess(object sender, ElapsedEventArgs e)
        {
            
            if (!look)
            {
                look = true;
                try
                {
                    Program.WriteLog(this.ServiceName, " OnProcess:" + System.DateTime.Now.ToString());
                    List<Fatture> invoices = Fatture.ReadAll();

                    foreach (var invoice in invoices)
                    {
                        List<Prodotti> products = Prodotti.GetProdottiPerFattura(invoice.Id);
                        if (products.Count == 0)
                        {
                            Program.WriteLog(this.ServiceName, $"OnProcess: la fattura {invoice.Numero} non contiene elementi.");
                            continue;
                        }

                        AnagrafeClienti customer = AnagrafeClienti.Find(products.First().CodCliente);
                        if (customer == null)
                        {
                            Program.WriteLog(this.ServiceName, $"OnProcess: la fattura {invoice.Numero} non è associata ad un cliente.");
                            continue;
                        }

                        FatturaElettronicaType fatturaPA = new FatturaElettronicaType();

                        #region Header
                        fatturaPA.FatturaElettronicaHeader = new FatturaElettronicaHeaderType();
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione = new DatiTrasmissioneType();
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.IdTrasmittente = new IdFiscaleType();
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.IdTrasmittente.IdPaese = Program.Config.IdPaese;
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.IdTrasmittente.IdCodice = Program.Config.IdCodice;

                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.ProgressivoInvio = $"{invoice.Anno}-{invoice.Numero}";
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione = FormatoTrasmissioneType.FPR12;
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario = "0000000";
                        //fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.PECDestinatario = "";

                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.ContattiTrasmittente = new ContattiTrasmittenteType();
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.ContattiTrasmittente.Telefono = Program.Config.Telefono;
                        fatturaPA.FatturaElettronicaHeader.DatiTrasmissione.ContattiTrasmittente.Email = Program.Config.Email;

                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore = new CedentePrestatoreType();
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici = new DatiAnagraficiCedenteType();
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA = new IdFiscaleType();
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA.IdPaese = Program.Config.IdPaese;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.IdFiscaleIVA.IdCodice = Program.Config.PartitaIva;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica = new AnagraficaType();
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Denominazione };
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.Anagrafica.Items = new string[] { Program.Config.Denominazione };
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.DatiAnagrafici.RegimeFiscale = RegimeFiscaleType.RF14;

                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.Sede = new IndirizzoType();
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.Sede.Indirizzo = Program.Config.Sede.Indirizzo;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.Sede.CAP = Program.Config.Sede.CAP;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.Sede.Comune = Program.Config.Sede.Comune;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.Sede.Provincia = Program.Config.Sede.Provincia;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.Sede.Nazione = Program.Config.Sede.Nazione;

                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA = new IscrizioneREAType();
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.Ufficio = Program.Config.IscrizioneREA.Ufficio;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.NumeroREA = Program.Config.IscrizioneREA.NumeroREA;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.SocioUnico = SocioUnicoType.SM;
                        fatturaPA.FatturaElettronicaHeader.CedentePrestatore.IscrizioneREA.StatoLiquidazione = StatoLiquidazioneType.LN;

                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente = new CessionarioCommittenteType();
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici = new DatiAnagraficiCessionarioType();
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.CodiceFiscale = customer.CodFisc;
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica = new AnagraficaType();
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.Nome, ItemsChoiceType.Cognome };
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.DatiAnagrafici.Anagrafica.Items = new string[] { customer.Nome, customer.Cognome };

                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.Sede = new IndirizzoType();
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.Sede.Indirizzo = customer.Indirizzo;
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.Sede.CAP = customer.Cap;
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.Sede.Comune = customer.Citta;
                        //fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.Sede.Provincia = customer.
                        fatturaPA.FatturaElettronicaHeader.CessionarioCommittente.Sede.Nazione = Program.Config.Sede.Nazione;
                        #endregion

                        #region Body
                        FatturaElettronicaBodyType fatturaElettronicaBodyType = new FatturaElettronicaBodyType();
                        fatturaPA.FatturaElettronicaBody = new FatturaElettronicaBodyType[] { fatturaElettronicaBodyType };
                        fatturaElettronicaBodyType.DatiGenerali = new DatiGeneraliType();
                        fatturaElettronicaBodyType.DatiGenerali.DatiGeneraliDocumento = new DatiGeneraliDocumentoType();
                        fatturaElettronicaBodyType.DatiGenerali.DatiGeneraliDocumento.TipoDocumento = TipoDocumentoType.TD01;
                        fatturaElettronicaBodyType.DatiGenerali.DatiGeneraliDocumento.Divisa = "EUR";
                        fatturaElettronicaBodyType.DatiGenerali.DatiGeneraliDocumento.Data = DateTime.ParseExact(invoice.Data.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        fatturaElettronicaBodyType.DatiGenerali.DatiGeneraliDocumento.Numero = $"{invoice.Anno}-{invoice.Numero}";

                        List<DettaglioLineeType> listDettaglioLineeType = new List<DettaglioLineeType>();
                        var cont = 0;
                        foreach (var product in products)
                        {
                            cont++;
                            DettaglioLineeType dettaglioLineeType = new DettaglioLineeType();
                            dettaglioLineeType.NumeroLinea = cont.ToString();
                            dettaglioLineeType.Descrizione = $"{product.TipoProdotto} - {product.Articolo} {product.Descrizione} {product.Taglia}";
                            dettaglioLineeType.PrezzoUnitario = Math.Round(product.PrezzoVenduto, 2); // TODO il separatore decimali deve essere il punto.
                            dettaglioLineeType.PrezzoTotale = Math.Round(product.PrezzoVenduto, 2);
                            dettaglioLineeType.AliquotaIVA = Program.Config.AliquotaIVA;

                            listDettaglioLineeType.Add(dettaglioLineeType);
                        }
                        fatturaElettronicaBodyType.DatiBeniServizi = new DatiBeniServiziType();
                        fatturaElettronicaBodyType.DatiBeniServizi.DettaglioLinee = listDettaglioLineeType.ToArray();

                        DatiRiepilogoType datiRiepilogoType = new DatiRiepilogoType();
                        datiRiepilogoType.AliquotaIVA = Program.Config.AliquotaIVA;
                        datiRiepilogoType.ImponibileImporto = Math.Round(invoice.Imponibile, 2);
                        datiRiepilogoType.Imposta = Math.Round(invoice.Iva, 2);
                        datiRiepilogoType.EsigibilitaIVA = EsigibilitaIVAType.I;

                        fatturaElettronicaBodyType.DatiBeniServizi.DatiRiepilogo = new DatiRiepilogoType[] { datiRiepilogoType };

                        #endregion

                        SerializeToXmlDocument(fatturaPA);

                        //TODO segno la fattura come esportata
                        Fatture.Update(invoice.Id);
                    }
                }
                catch (Exception ex)
                {
                    Program.WriteLog(this.ServiceName, " OnProcess: ERROR" + ex.Message);
                }
                finally
                {
                    look = false;
                }
            }
        }//OnProcess

        public XmlDocument SerializeToXmlDocument(FatturaElettronicaType input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType(), input.SchemaLocation);

            XmlDocument xd = null;
            string filePath = $"D:\\Fatture\\{DateTime.Now.Date.ToString("dd-MM-yyyy")}\\IT{input.FatturaElettronicaHeader.DatiTrasmissione.ProgressivoInvio}_FPA.xml";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (FileStream memStm = new FileStream(@filePath, FileMode.Create))
            {
                ser.Serialize(memStm, input);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }

            return xd;
        }
    }
}
