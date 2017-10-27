using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Orchard.Caching.Memcached.Services
{
    public class MemcachedClientHolder : IDisposable {
        private MemcachedClient _client;
        private HashAlgorithm _hasher;
        private readonly object _synLock = new object();
        public HashAlgorithm Hasher {
            get {
                if (_hasher == null) {
                    _hasher = MD5.Create();
                }

                return _hasher;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public MemcachedClient GetClient(string servers) {
            if (_client == null  && !String.IsNullOrEmpty(servers)) {
                var configuration = new MemcachedClientConfiguration();
                using (var urlReader = new StringReader(servers)) {
                    string server;
                    // ignore empty lines and comments (#)
                    while (null != (server = urlReader.ReadLine())) {
                        if (String.IsNullOrWhiteSpace(server) || server.Trim().StartsWith("#")) {
                            continue;
                        }

                        var values = server.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int port = 11211;

                        if (values.Length == 2) {
                            Int32.TryParse(values[1], out port);
                        }

                        if (values.Length > 0) {
                            configuration.AddServer(values[0], port);
                        }
                    }

                    lock (_synLock) {
                        _client = new MemcachedClient(configuration);
                    }
                }
            }
            
            return _client;
        }

        /// <summary>
        /// Initializes the Cache using the default configuration section (enyim/memcached) to configure the memcached client
        /// </summary>
        /// <see cref="Enyim.Caching.Configuration.MemcachedClientSection"/>
        public MemcachedClientHolder()
        {
            _client = new MemcachedClient();
        }

        /// <summary>
        /// Initializes the Cache using the provided hosts to configure the memcached client
        /// </summary>
        /// <param name="hosts"></param>
        public MemcachedClientHolder(IEnumerable<string> hosts)
        {
            const int defaultPort = 11211;
            const int ipAddressIndex = 0;
            const int portIndex = 1;

            var ipEndpoints = new List<IPEndPoint>();
            foreach (var host in hosts)
            {
                var hostParts = host.Split(':');
                if (hostParts.Length == 0)
                    throw new ArgumentException("'{0}' is not a valid host IP Address: e.g. '127.0.0.0[:11211]'");

                var port = (hostParts.Length == 1) ? defaultPort : int.Parse(hostParts[portIndex]);

                var hostAddresses = Dns.GetHostAddresses(hostParts[ipAddressIndex]);
                foreach (var ipAddress in hostAddresses)
                {
                    var endpoint = new IPEndPoint(ipAddress, port);
                    ipEndpoints.Add(endpoint);
                }
            }

            LoadClient(PrepareMemcachedClientConfiguration(ipEndpoints));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoints"></param>
        public MemcachedClientHolder(IEnumerable<IPEndPoint> ipEndpoints)
        {
            LoadClient(PrepareMemcachedClientConfiguration(ipEndpoints));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedClientCache"/> class based on an existing <see cref="IMemcachedClientConfiguration"/>.
        /// </summary>
        /// <param name="memcachedClientConfiguration">The <see cref="IMemcachedClientConfiguration"/>.</param>
        public MemcachedClientHolder(IMemcachedClientConfiguration memcachedClientConfiguration)
        {
            LoadClient(memcachedClientConfiguration);
        }

        /// <summary>
        /// Prepares a MemcachedClientConfiguration based on the provided ipEndpoints.
        /// </summary>
        /// <param name="ipEndpoints">The ip endpoints.</param>
        /// <returns></returns>
        private IMemcachedClientConfiguration PrepareMemcachedClientConfiguration(IEnumerable<IPEndPoint> ipEndpoints)
        {
            var config = new MemcachedClientConfiguration();
            foreach (var ipEndpoint in ipEndpoints)
            {
                config.Servers.Add(ipEndpoint);
            }

            config.SocketPool.MinPoolSize = 10;
            config.SocketPool.MaxPoolSize = 100;
            config.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 10);
            config.SocketPool.DeadTimeout = new TimeSpan(0, 2, 0);

            return config;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        private void LoadClient(IMemcachedClientConfiguration config)
        {
            _client = new MemcachedClient(config);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key) {
            return _client.Get(FormatKey(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(string key, object value) {
            _client.Store(StoreMode.Set, FormatKey(key), value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        public void Put(string key, object value, TimeSpan validFor) {
            _client.Store(StoreMode.Set, FormatKey(key), value, validFor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) {
            _client.Remove(FormatKey(key));
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            _client.FlushAll();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose() {
            if (_client != null) {
                _client.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string FormatKey(string key) {
            if (String.IsNullOrEmpty(key)) {
                throw new ArgumentException("key");
            }

            // memcached keys can't be longer than 250 chars
            if (key.Length >= 250) {
                return ComputeHash(key, Hasher);
            }
            
            // memcache keys can't have spaces or system chars
            var chars = key.ToCharArray();
            var altered = false;
            for (int i = 0; i < chars.Length; i++) {
                if (chars[i] < 33) {
                    chars[i] = '-';
                    altered = true;
                }
            }

            if (altered) {
                return new string(chars);
            }

            return key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullKeyString"></param>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        private  string ComputeHash(string fullKeyString, HashAlgorithm hashAlgorithm) {
            byte[] bytes = Encoding.ASCII.GetBytes(fullKeyString);
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
        }
    }
}