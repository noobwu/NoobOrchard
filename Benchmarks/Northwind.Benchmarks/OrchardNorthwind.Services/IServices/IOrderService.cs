using OrchardNorthwind.Common.Entities;
using Orchard.IServices;
namespace OrchardNorthwind.IServices
{
    /// <summary>
    /// Orders服务相关接口
    /// </summary>
    public partial interface IOrderService:IServiceBase<Order,int>
    {
      
    }
}
