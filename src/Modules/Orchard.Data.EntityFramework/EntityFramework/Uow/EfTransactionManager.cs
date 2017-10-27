using Orchard.Domain.Uow;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

namespace Orchard.Data.EntityFramework.Uow
{
    public class EfTransactionManager : ITransactionManager, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public ILogger Logger { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
        public EfTransactionManager(IDbConnection connection)
        {
            _connection = connection;
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
            EnsureDatabase();
            if (_transaction != null)
            {
                _transaction = _connection.BeginTransaction(level);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Rollback()
        {
            if (_connection != null)
            {
                // IsActive is true if the transaction hasn't been committed or rolled back
                if (_transaction != null)
                {
                    Logger.Debug("Rolling back transaction");
                    _transaction.Rollback();
                }
            }
            DisposeConnection();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            if (_connection != null)
            {                    // IsActive is true if the transaction hasn't been committed or rolled back
                if (_transaction != null && CheckConnState())
                {
                    try
                    {
                        Logger.Debug("Committing transaction");
                        _transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        DisposeConnection();
                        throw ex;
                    }
                }
            }
            DisposeConnection();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckConnState()
        {
            return _connection.State != ConnectionState.Broken && _connection.State != ConnectionState.Closed;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            DisposeConnection();
        }
        /// <summary>
        /// 
        /// </summary>
        private void EnsureDatabase()
        {
            if (_connection == null)
            {
                throw new ArgumentNullException("database can't be null, ever");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DisposeConnection()
        {
            Logger.Debug("Disposing session");
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
