using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Orchard.Domain.Uow;
using Orchard.Domain.Uow.Transaction;
using System;
using System.Linq;
using System.Reflection;

namespace Orchard.Data.EntityFrameworkCore.Uow.Transaction
{
    /// <summary>
    /// EntityFramework事务拦截器
    /// </summary>
    public class EfCoreTransactionInterceptor : ITransactionInterceptor
    {
        private ITransactionUnitOfWork<DatabaseFacade> transUnitOfWork;
        protected EfCoreDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transUnitOfWork"></param>
        /// <param name="dbContext"></param>
        public EfCoreTransactionInterceptor(ITransactionUnitOfWork<DatabaseFacade> transUnitOfWork, EfCoreDbContext dbContext)
        {
            this.transUnitOfWork = transUnitOfWork;
            this.dbContext = dbContext;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("Intercept,Time:{0},GetType():{1},Method:{2} Starting.", DateTime.Now.ToString("O"), invocation.InvocationTarget.GetType(), invocation.Method.Name);
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }
            ITransactionCompleteHandle transHandle;
            UowTransactionAttribute transaction = methodInfo.GetCustomAttributes<UowTransactionAttribute>(true).FirstOrDefault();
            if (transaction != null)
            {
                if (transaction.IsolationLevel.HasValue)
                {
                    transHandle = transUnitOfWork.Begin(dbContext.Database, transaction.IsolationLevel.Value);
                }
                else
                {
                    transHandle= transUnitOfWork.Begin(dbContext.Database);
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
            Console.WriteLine("Intercept,Time:{0},GetType():{1},Method:{2} Ending.", DateTime.Now.ToString("O"), invocation.InvocationTarget.GetType(), invocation.Method.Name);
        }
    }
}
