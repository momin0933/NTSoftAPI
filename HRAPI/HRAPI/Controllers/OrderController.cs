using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRAPI.BusinessLayer.Interface;
using HRAPI.Models;

namespace HRAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderManager orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all active orders
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAllOrders(int pageNumber, int pageSize)
        {
            try
            {
                var orders = _orderService.GetAllOrders(pageNumber, pageSize);
                return Ok(new { success = true, data = orders, message = "Orders retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);
                if (order == null)
                    return NotFound(new { success = false, message = "Order not found" });

                return Ok(new { success = true, data = order, message = "Order retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with id: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get orders by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public IActionResult GetOrdersByCustomer(int customerId)
        {
            try
            {
                var orders = _orderService.GetOrdersByCustomer(customerId);
                return Ok(new { success = true, data = orders, message = "Orders retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for customer: {customerId}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

       

        /// <summary>
        /// Get orders by buyer ID
        /// </summary>
        [HttpGet("buyer/{buyerId}")]
        public IActionResult GetOrdersByBuyer(int buyerId)
        {
            try
            {
                var orders = _orderService.GetOrdersByBuyer(buyerId);
                return Ok(new { success = true, data = orders, message = "Orders retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for buyer: {buyerId}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get orders by date range
        /// </summary>
        [HttpGet("date-range")]
        public IActionResult GetOrdersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate >= endDate)
                    return BadRequest(new { success = false, message = "Start date must be before end date" });

                var orders = _orderService.GetOrdersByDateRange(startDate, endDate);
                return Ok(new { success = true, data = orders, message = "Orders retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for date range: {startDate} - {endDate}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Add new order
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddOrder([FromBody] Order order)
        {
            try
            {
                if (order == null)
                    return BadRequest(new { success = false, message = "Order data is required" });

                var result = await _orderService.AddOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = result }, new { success = true, data = result, message = "Order added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

       

        /// <summary>
        /// Update order
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder([FromBody] Order order)
        {
            try
            {
                if (order == null || order.Id <= 0)
                    return BadRequest(new { success = false, message = "Valid order data is required" });

                var result = await _orderService.UpdateOrderAsync(order);
                if (!result)
                    return NotFound(new { success = false, message = "Order not found" });

                return Ok(new { success = true, data = result, message = "Order updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete order (soft delete)
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Valid order ID is required" });

                var result = await _orderService.DeleteOrderAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Order not found" });

                return Ok(new { success = true, data = result, message = "Order deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order: {id}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}