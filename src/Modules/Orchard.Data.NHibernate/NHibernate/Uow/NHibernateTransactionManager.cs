using NHibernate;
using Orchard.Domain.Uow;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Orchard.Data.NHibernate.Uow
{
    public class NHibernateTransactionManager : ITransactionManager, IDisposable
    {
        private ISession _session;
        private ITransaction _transaction;
        public ILogger Logger { get; set; }
        public System.Data.IsolationLevel IsolationLevel { get; set; }
        public NHibernateTransactionManager(ISession session)
        {
            _session = session;
            Logger = NullLogger.Instance;
            IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Begin()
        {
            Begin(IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public void Begin(IsolationLevel level)
        {
            EnsureSession();
            if (_transaction != null)
            {
                _transaction = _session.BeginTransaction(level);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Rollback()
        {
            if (_session != null)
            {
                // IsActive is true if the transaction hasn't been committed or rolled back
                if (_transaction != null && _transaction.IsActive)
                {
                    Logger.Debug("Rolling back transaction");
                    _transaction.Rollback();
                }
            }
            DisposeSession();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            if (_session != null)
            {
                // IsActive is true if the transaction hasn't been committed or rolled back
                if (_transaction != null && _transaction.IsActive)
                {
                    Logger.Debug("Committing transaction");
                    try
                    {
                        _transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        _transaction.Rollback();
                        DisposeSession();
                        throw ex;
                    }
                }
            }
            DisposeSession();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            DisposeSession();
        }

        private void EnsureSession()
        {
            if (_session == null)
            {
                throw new ArgumentNullException("Session can't be null, ever");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DisposeSession()
        {
            Logger.Debug("Disposing session");
            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
