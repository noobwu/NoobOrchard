using Castle.DynamicProxy;

namespace Orchard.Domain.Uow.Transaction
{
    /// <summary>
    /// 事务拦截器
    /// </summary>
    public interface ITransactionInterceptor: IInterceptor
    {
    }
}
