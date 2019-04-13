using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP;
using Exceptionless.Json;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.OrderService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manulife.DNC.MSAD.WS.OrderService.Controllers
{
    [Route("api/OrderSyn")]
    public class OrderSynController : Controller
    {
        public OrderDbContext DbContext { get; }
        public ICapPublisher CapPublisher { get; }
        public string ConnStr { get; } // For Dapper use

        public OrderSynController(OrderDbContext DbContext, ICapPublisher CapPublisher, string ConnStr)
        {
            this.DbContext = DbContext;
            this.CapPublisher = CapPublisher;
            this.ConnStr = ConnStr;
        }

        
        [HttpGet]
        public string Get()
        {
            //var query;
            //bool result = false;
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                
                    // business code here
                    string sqlCommand = @"select top 1 * from Orders t order by t.OrderTime desc";

                    var query = conn.Query<Order>(sqlCommand);

                conn.Close();

                    return JsonConvert.SerializeObject(query);
                
            }

            //return "";
        }
    }
}