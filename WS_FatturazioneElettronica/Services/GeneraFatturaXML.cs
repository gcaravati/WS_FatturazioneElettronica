using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using WS_FatturazioneElettronica.Models;

namespace WS_FatturazioneElettronica.Services
{
    public partial class GeneraFatturaXML : ServiceBase
    {
        private Timer _timer = new Timer();
        private double _Interval = 30000;
        private string _Queue;
        private string _Sended;
        public GeneraFatturaXML()
        {
            InitializeComponent();
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
            Program.WriteLog(this.ServiceName, " OnProcess:" + System.DateTime.Now.ToString());
            List<Fatture> invoices = Fatture.ReadAll();

           




            XmlTextWriter writer;
            XmlDocument xmlQueue;
            XmlDocument xmlSended;

            xmlSended = new XmlDocument();
            xmlSended.Load(_Sended);

            using (FileStream fileQueue = new FileStream(_Queue, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                xmlQueue = new XmlDocument();
                xmlQueue.Load(fileQueue);

                string fragmentTemplate = "<item><email>{0}</email><dataInvio>{1} {2}</dataInvio></item>";

                foreach (XmlNode node in xmlQueue.SelectNodes("/emails/email"))
                {
                    //Create a document fragment.
                    XmlDocumentFragment docFrag = xmlSended.CreateDocumentFragment();

                    //Set the contents of the document fragment.
                    docFrag.InnerXml = String.Format(fragmentTemplate, node.InnerText, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());

                    //Add the children of the document fragment to the
                    //original document.
                    xmlSended.DocumentElement.AppendChild(docFrag);
                }

                xmlQueue.DocumentElement.RemoveAll();

            }

            //fileToSend.Seek(0, SeekOrigin.End);

            writer = new XmlTextWriter(_Queue, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            xmlQueue.WriteTo(writer);
            writer.Flush();
            writer.Close();

            writer = new XmlTextWriter(_Sended, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            xmlSended.WriteTo(writer);
            writer.Flush();
            writer.Close();

        }//OnProcess
    }
}
