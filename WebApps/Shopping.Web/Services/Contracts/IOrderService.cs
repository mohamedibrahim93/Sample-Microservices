using Shopping.Web.Models;

namespace Shopping.Web.Services.Contracts
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
    }
}