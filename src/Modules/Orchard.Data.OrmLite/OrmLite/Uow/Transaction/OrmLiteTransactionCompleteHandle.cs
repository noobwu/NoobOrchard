using Orchard.Exceptions;
using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using Orchard.Logging;
using Orchard.Domain.Uow;

namespace Orchard.Data.OrmLite.Uow
{
    /// <summary>
    /// This handle is used for innet unit of work scopes.
    /// A inner unit of work scope actually uses outer unit of work scope
    /// and has no effect on <see cref="IUnitOfWorkCompleteHandle.Complete"/> call.
    /// But if it's not called, an exception is thrown at end of the UOW to rollback the UOW.
    /// </summary>
    public class OrmLiteTransactionCompleteHandle : ITransactionCompleteHandle
    {
        public IDbConnection _connection;
        private IDbTransaction _transaction;
        public ILogger Logger { get; set; }
        public OrmLiteTransactionCompleteHandle(IDbConnection connection, IsolationLevel isolationLevel)
        {
            _connection = connection;
            Logger = NullLogger.Instance;
            _transaction = _connection.OpenTransaction(isolationLevel);
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
            {
                // IsActive is true if the transaction hasn't been committed or rolled back
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

        private static bool HasException()
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}