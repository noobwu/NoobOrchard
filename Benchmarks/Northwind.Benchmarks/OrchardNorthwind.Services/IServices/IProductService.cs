using OrchardNorthwind.Common.Entities;
using Orchard.IServices;
namespace OrchardNorthwind.IServices
{
    /// <summary>
    /// Products服务相关接口
    /// </summary>
    public partial interface IProductService:IServiceBase<Product,int>
    {
      
    }
}
