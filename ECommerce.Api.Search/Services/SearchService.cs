using ECommerce.Api.Search.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService orderService;
        private readonly IProductsService productsService;
        private readonly ICustomerService customerService;

        public SearchService(IOrdersService orderService, IProductsService productsService, ICustomerService customerService)
        {
            this.orderService = orderService;
            this.productsService = productsService;
            this.customerService = customerService;
        }
        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var customerResult = await customerService.GetCustomerAsync(customerId);
            var ordersResult = await orderService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();

            if (ordersResult.IsSuccess)
            {
                foreach (var order in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ? 
                            productsResult.products.FirstOrDefault(p => p.Id == item.ProductId)?.Name :
                            "Product information is not available";
                    }
                }
                var result = new
                {
                    Customer = customerResult.IsSuccess ?
                                customerResult.Customer :
                                new { Name = "Customer information is not available"},
                    Orders = ordersResult.Orders,
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
