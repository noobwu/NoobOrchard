using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using System;
using System.Threading;
using Topshelf;

namespace OrchardNorthwind.Services.OrleansClient
{
    /// <summary>
    /// 
    /// </summary>
    public class SiloClientService : ServiceControl
    {
        /// <summary>
        /// 
        /// </summary>
        private Action runAction;
        /// <summary>
        /// 
        /// </summary>
        public SiloClientService(Action runAction)
        {
            this.runAction = runAction;
        }
        /// <summary>
        /// 
        /// </summary>
        private IClusterClient client = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            StartClient();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            client?.Close();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void StartClient()
        {
            ClientConfiguration clientConfig=null;
            //bool startOk = false;
            try
            {
                clientConfig = ClientConfiguration.LoadFromFile("ClientConfiguration.xml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
                return;
            }
           
            try
            {
                //clientConfig.SerializationProviders.Add(typeof(LinqSerializer).GetTypeInfo());
                InitializeWithRetries(clientConfig, initializeAttemptsBeforeFailing: 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");
                Console.ReadKey();
                return;
            }
            try
            {
                // Then configure and connect a client.

                //client = new ClientBuilder().UseConfiguration(clientConfig).Build();
                //client.Connect().Wait();

                Console.WriteLine("Client connected.");
                //startOk = true;
                //
                // This is the place for your test code.
                //
                runAction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
            //return startOk;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="initializeAttemptsBeforeFailing"></param>
        private void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.ConfigureLoggingDelegate = logBuilder => {
                        logBuilder.AddConsole();
                        logBuilder.AddNLog();
                        logBuilder.ConfigureNLog("App_Data/Configs/NLog.config");
                    };
                    GrainClient.Initialize(config);
                    client = GrainClient.Instance;
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException ex)
                {
                    Console.WriteLine(ex);
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }

            }
        }
       
    }
}
