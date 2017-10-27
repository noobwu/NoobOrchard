using OrchardNorthwind.Common.Entities;
using Orchard.IServices;
namespace OrchardNorthwind.IServices
{
    /// <summary>
    /// Employees服务相关接口
    /// </summary>
    public partial interface IEmployeeService:IServiceBase<Employee,int>
    {
      
    }
}
