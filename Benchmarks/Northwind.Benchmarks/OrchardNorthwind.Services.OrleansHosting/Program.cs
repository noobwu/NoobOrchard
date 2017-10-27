using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Topshelf;

namespace OrchardNorthwind.Services.OrleansHosting
{
    /// <summary>
    /// Orleans Silo Host
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            //安装服务 OrchardNorthwind.Services.OrleansHosting.exe install
            //卸载服务 OrchardNorthwind.Services.OrleansHosting.exe uninstall
            //启动服务 OrchardNorthwind.Services.OrleansHosting.exe start
            //停止服务 OrchardNorthwind.Services.OrleansHosting.exe stop
            string postfix = string.Empty;
            int port = 28080;
            try
            {
                string logPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                HostFactory.Run(x =>
                {
                    //OrchardNorthwind.Services.OrleansHosting.exe -postfix:-Slave -port:38080
                    x.AddCommandLineDefinition("postfix", callback => { postfix = callback; });
                    x.AddCommandLineDefinition("port", callback => { port = To(callback,port); });
                    x.ApplyCommandLine();

                    x.RunAsLocalSystem();

                    x.SetServiceName("OrchardNorthwindServicesOrleansHosting" + postfix);
                    x.SetDisplayName("OrchardNorthwindServicesOrleansHosting" + postfix+ " Topshelf Server");
                    x.SetDescription("using topshelf to  orleans server,processing service logic etc.");
                    x.SetStartTimeout(TimeSpan.FromMinutes(5));
                    //https://github.com/Topshelf/Topshelf/issues/165
                    x.SetStopTimeout(TimeSpan.FromMinutes(35));

                    x.EnableServiceRecovery(r => { r.RestartService(1); });

                    x.Service(factory => {
                        return new SiloHostService(postfix,port);
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T To<T>(object value, T defaultValue)
        {
            try
            {
                return value == null ? defaultValue : (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }

        }
    }
}
