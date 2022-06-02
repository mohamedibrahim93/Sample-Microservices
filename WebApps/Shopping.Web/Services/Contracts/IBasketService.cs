using Shopping.Web.Models;

namespace Shopping.Web.Services.Contracts
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);

        Task<BasketModel> UpdateBasket(BasketModel model);

        Task CheckoutBasket(BasketCheckoutModel model);
    }
}