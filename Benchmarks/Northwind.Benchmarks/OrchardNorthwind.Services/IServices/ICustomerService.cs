using OrchardNorthwind.Common.Entities;
using Orchard.IServices;
namespace OrchardNorthwind.IServices
{
    /// <summary>
    /// Customers服务相关接口
    /// </summary>
    public partial interface ICustomerService:IServiceBase<Customer,string>
    {
      
    }
}
