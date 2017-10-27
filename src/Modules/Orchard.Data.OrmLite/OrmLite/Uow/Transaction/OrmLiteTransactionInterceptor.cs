using Castle.DynamicProxy;
using Orchard.Domain.Uow;
using Orchard.Domain.Uow.Transaction;
using ServiceStack.OrmLite;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Orchard.Data.OrmLite.Uow.Transaction
{
    /// <summary>
    /// OrmLite事务拦截器
    /// </summary>
    public class OrmLiteTransactionInterceptor : IInterceptor
    {
        private ITransactionUnitOfWork<IDbConnection> transUnitOfWork;
        private OrmLiteConnectionFactory dbFactory;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transUnitOfWork"></param>
        /// <param name="dbFactory"></param>
        public OrmLiteTransactionInterceptor(ITransactionUnitOfWork<IDbConnection> transUnitOfWork, OrmLiteConnectionFactory dbFactory)
        {
            this.transUnitOfWork = transUnitOfWork;
            this.dbFactory = dbFactory;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            //Console.WriteLine("Intercept,Time:{0},GetType():{1},Method:{2} Starting.", DateTime.Now.ToString("O"), invocation.InvocationTarget.GetType(), invocation.Method.Name);
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }
            UowTransactionAttribute transaction =methodInfo.GetCustomAttributes<UowTransactionAttribute>(true).FirstOrDefault();
            if (transaction != null)
            {
                var connection = dbFactory.OpenDbConnection();
                ITransactionCompleteHandle transHandle;
                if (transaction.IsolationLevel.HasValue)
                {
                    transHandle = transUnitOfWork.Begin(connection, transaction.IsolationLevel.Value);
                }
                else
                {
                    transHandle = transUnitOfWork.Begin(connection);
                }
                using (transHandle)
                {
                    invocation.Proceed();
                    transHandle.Commit();
                }
            }
            else
            {
                // 没有事务时直接执行方法
                invocation.Proceed();
            }
            //Console.WriteLine("Intercept,Time:{0},GetType():{1},Method:{2} Ending.", DateTime.Now.ToString("O"), invocation.InvocationTarget.GetType(), invocation.Method.Name);
        }
    }
}
