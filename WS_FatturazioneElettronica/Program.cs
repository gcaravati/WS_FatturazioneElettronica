using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WS_FatturazioneElettronica.Services;

namespace WS_FatturazioneElettronica
{
    static class Program
    {
        public static Configurazione Config;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            
            Config = new Configurazione();

            ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            
            
            if (Environment.UserInteractive)
            {
                GeneraFatturaXML generaFatturaXML = new GeneraFatturaXML();
                string[] args = null;
                generaFatturaXML.TestStartupAndStop(args);
            }
            else
            {
                ServicesToRun = new ServiceBase[] { new GeneraFatturaXML() };
                ServiceBase.Run(ServicesToRun);
            }
        }


        public static void WriteLog(string ServiceName, string Messaggio)
        {
            //Esiste un file di log per ogni servizio
            // + un file di log per l'accesso ai dati
            // i file di log sono posizionati in una sottocartella
            // rispetto alla posizione del servizio

            string sFileName = ServiceName + ".log";
            System.Diagnostics.Debug.WriteLine(Messaggio);

            try
            {
                FileStream fs = new FileStream(@"D:\temp\" + sFileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(ServiceName + " " + Messaggio);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
