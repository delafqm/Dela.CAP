using Dapper;
using Exceptionless.Json;
using Manulife.DNC.MSAD.WS.OrderService.Models;
using Manulife.DNC.MSAD.WS.OrderService.Repositories;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Manulife.DNC.MSAD.WS.OrderService.Controllers
{
    
    public class OrderController : Controller
    {
        public IOrderRepository OrderRepository { get; }

        public string ConnStr { get; } // For Dapper use

        public OrderController(IOrderRepository OrderRepository, string Connstr)
        {
            this.OrderRepository = OrderRepository;
            this.ConnStr = Connstr;
        }

        [Route("~/api/Order")]
        [HttpPost]
        public string Post([FromBody]OrderDTO orderDTO)
        {
            var result = OrderRepository.CreateOrderByDapper(orderDTO).GetAwaiter().GetResult();

            //var result = OrderRepository.CreateOrderByEF(orderDTO).GetAwaiter().GetResult();

            return result ? "提交订单成功" : "提交订单失败";
        }

        [Route("~/api/OrderOne")]
        [HttpGet]
        public string Get1()
        {
            OrderDTO orderDTO = new OrderDTO();
            orderDTO.ProductID = "PD10001";
            orderDTO.OrderUserID = "WS10101";

            var result = OrderRepository.CreateOrderByDapper(orderDTO).GetAwaiter().GetResult();

            //var result = OrderRepository.CreateOrderByEF(orderDTO).GetAwaiter().GetResult();

            return result ? "提交订单成功" : "提交订单失败";
        }

        [Route("~/api/queryall")]
        [HttpGet]
        public string Get()
        {
            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();

                // business code here
                string sqlCommand = @"select * from Orders t order by t.OrderTime desc LIMIT 1";

                var query = conn.Query<Order>(sqlCommand);

                conn.Close();

                return JsonConvert.SerializeObject(query);

            }
        }
    }
}