using Orchard.Exceptions;
using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Orchard.Logging;
using Orchard.Domain.Uow;
using System.Data.Entity;
namespace Orchard.Data.EntityFramework.Uow
{
    /// <summary>
    /// This handle is used for innet unit of work scopes.
    /// A inner unit of work scope actually uses outer unit of work scope
    /// and has no effect on <see cref="IUnitOfWorkCompleteHandle.Complete"/> call.
    /// But if it's not called, an exception is thrown at end of the UOW to rollback the UOW.
    /// </summary>
    public class EfTransactionCompleteHandle : ITransactionCompleteHandle
    {
        public Database _database;
        private DbContextTransaction _transaction;
        public ILogger Logger { get; set; }
        public EfTransactionCompleteHandle(Database database, IsolationLevel isolationLevel)
        {
            _database = database;
            Logger = NullLogger.Instance;
            _transaction = _database.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Rollback()
        {
            if (_database != null)
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
            if (_database != null)
            {
                if (_transaction != null && CheckConnState())
                {
                    try
                    {
                        // IsActive is true if the transaction hasn't been committed or rolled back

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
            return _database.Connection.State != ConnectionState.Broken && _database.Connection.State != ConnectionState.Closed;
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
            if (_database != null)
            {
                _database.Connection.Close();
                _database.Connection.Dispose();
                _database = null;
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