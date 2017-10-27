using Orchard.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.Configuration
{
    /// <summary>
    /// 基本设置描述类, 加[Serializable]标记为可序列化
    /// </summary>
    [Serializable]
    public class WebConfig : NameValueBasedConfig
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebTitle = "互联网站内容管理系统";
        /// <summary>
        /// 网站url地址
        /// </summary>
        public string WebUrl = ""; //网站url地址
        /// <summary>
        /// 网站备案信息
        /// </summary>
        public string ICP = "";
        /// <summary>
        /// 统计代码
        /// </summary>
        public string StatCode = "";

        /// <summary>
        /// 当前站点web园进程数      
        /// </summary>
        public int WebGarden = 1;

        /// <summary>
        /// 网站版权文字
        /// </summary>
        public string Copyright = "&copy; 2013-" + DateTime.Now.Year.ToString();

        /// <summary>
        /// 网址根目录地址
        /// </summary>
        public string RootUrl = "/";

        /// <summary>
        /// 上传地址
        /// </summary>
        public string UploadUrl = "/Uploads/";
        /// <summary>
        /// 上传目录
        /// </summary>
        public string UploadDir = "Uploads";

        /// <summary>
        /// Cookie域名
        /// </summary>
        public string CookieDomain = "/";

        /// <summary>
        /// Property indicating if performance is enabled for the app
        /// </summary>
        /// <remarks>
        /// Performance is considered enabled if there is a Performance Counter Category name
        /// in the app config and that category exists on the machine
        /// </remarks>
        public bool PerformanceEnabled = false;


        /// <summary>
        /// Gets the process id of the ASP.NET worker process the application is running inside
        /// </summary>
        public int ProcessId = 0;

        /// <summary>
        /// Gets a String of the name of the Performance Counter Category to use
        /// </summary>
        /// <remarks>
        /// This performance counter category needs to exist on the machine for performance to
        /// be tracked.  Nominally, it is a good idea to make this name the same as the application
        /// name so it is easy to tell the performance for one app versus another
        /// </remarks>
        public String PerformanceCategoryName = "Performance.EnablePerformanceMonitoring";
        /// <summary>
        /// Gets a String of the name of the Performance Counter Category to use
        /// </summary>
        /// <remarks>
        /// Metric HttpEndpoint
        /// </remarks>
        public String MetricUrl = "";
        public WebConfig(string configPath, string sectionName) : base(configPath, sectionName)
        {
            Init();
        }
        private void Init()
        {
            ICP = Get("ICP");
            WebTitle = Get("WebTitle");
            WebUrl = Get("WebUrl");
            RootUrl = Get("RootUrl");
            StatCode = Get("StatCode");
            WebGarden = Get("WebGarden", 1);
            UploadUrl = Get("UploadUrl");
            UploadDir = Get("UploadDir");
            CookieDomain = Get("CookieDomain");
            PerformanceEnabled = Get("PerformanceEnabled", false);
            PerformanceCategoryName = Get("PerformanceCategoryName");
            if (PerformanceEnabled)
            {
                ProcessId = Process.GetCurrentProcess().Id;
            }
            MetricUrl = Get("MetricUrl");
        }

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static WebConfig GetInstance(string configPath, string sectionName)
        {

            if (Singleton<WebConfig>.Instance == null)
            {
                Initialize(configPath,sectionName);
            }
            return Singleton<WebConfig>.Instance;
        }

        /// <summary>
        /// Initializes a static instance of the Nop factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        /// <param name="dependencyRegistrar">IDependencyRegistrar</param>
        /// <param name="httpConfig">HttpConfiguration</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static WebConfig Initialize(string configPath, string sectionName)
        {
            if (Singleton<WebConfig>.Instance == null)
            {
                Singleton<WebConfig>.Instance = new WebConfig(configPath,sectionName);
            }
            return Singleton<WebConfig>.Instance;
        }
    }
}
