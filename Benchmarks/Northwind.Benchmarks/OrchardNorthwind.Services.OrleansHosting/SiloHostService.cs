using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using OrleansDashboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace OrchardNorthwind.Services.OrleansHosting
{
    /// <summary>
    /// 
    /// </summary>
    public class SiloHostService : ServiceControl
    {
        private string configPath = "ServerConfiguration.xml";
        private string siloName = "OrchardNorthwindServicesSiloHost";
        private int port = 28080;
        public SiloHostService(string postfix = "",int port=28080)
        {
            if (!string.IsNullOrEmpty(postfix))
            {
                this.configPath = "ServerConfiguration"+postfix+".xml";
                siloName = siloName + postfix;
            }
            this.port = port;
        }
        private SiloHost siloHost;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            return StartHost();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            // Shut down
            if (siloHost != null)
            {
                siloHost.ShutdownOrleansSilo();
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        private bool StartHost()
        {
            try
            {
                TextReader input = File.OpenText(configPath);
                ClusterConfiguration siloConfig = new ClusterConfiguration();
                siloConfig.Load(input);
                //siloConfig.UseStartupType<Noob.Orleans.DependencyInjection.NoobBootstrap>();
                //siloConfig.Globals.RegisterDashboard(port: port);
                Console.WriteLine($"Dashboard will listen on http://localhost:{port}/");
                return StartHostByConfig(siloConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
                return false;
            }
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siloConfig"></param>
        private bool StartHostByConfig(ClusterConfiguration siloConfig)
        {
            bool startedOk = false;
            try
            {
                siloHost = new SiloHost(siloName, siloConfig);
                //siloHost.LoadOrleansConfig();
                siloHost.InitializeOrleansSilo();
                startedOk = siloHost.StartOrleansSilo(false);
                if (!startedOk)
                {
                    Console.WriteLine(String.Format("Failed to start Orleans silo '{0}' as a {1} node", siloHost.Name, siloHost.Type));
                }
                else
                {
                    Console.WriteLine("Silo started.");
                }
                //
                // This is the place for your test code.
                //
                //Console.WriteLine("\nPress Enter to terminate...");
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                if (siloHost != null)
                {
                    siloHost.ReportStartupError(ex);
                }
                throw ex;
            }
            return startedOk;
        }
    }
}
