using System;
using System.Collections.Generic;
using System.IO;

namespace Orchard.Client
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRestClient
    {
        void AddHeader(string name, string value);

        void ClearCookies();
        Dictionary<string, string> GetCookieValues();
        void SetCookie(string name, string value, TimeSpan? expiresIn = null);

    }
}