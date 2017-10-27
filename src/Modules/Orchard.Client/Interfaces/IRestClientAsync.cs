using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Client
{
    public interface IRestClientAsync : IDisposable
    {
        void SetCredentials(string userName, string password);

        void CancelAsync();
    }

}