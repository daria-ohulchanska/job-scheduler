using JobScheduler.Core.Enums;
using JobScheduler.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("serve")]
        public async Task<IActionResult> Serve(string userId, Dish dish)
        {
            await _orderService.ServeAsync(userId, dish);
            return Ok();
        }
    }
}
