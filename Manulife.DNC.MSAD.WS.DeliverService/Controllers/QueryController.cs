using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Exceptionless.Json;
using Manulife.DNC.MSAD.WS.DeliveryService.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Manulife.DNC.MSAD.WS.DeliverService.Controllers
{
    public class QueryController : Controller
    {
        public string ConnStr { get; } // For Dapper use

        public QueryController(string ConnStr)
        {
            this.ConnStr = ConnStr;
        }

        [Route("~/api/queryall")]
        [HttpGet]
        public string Get()
        {
            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();

                // business code here
                string sqlCommand = @"select * from deliveries t order by t.CreatedTime desc LIMIT 1";

                var query = conn.Query<Delivery>(sqlCommand);

                conn.Close();

                return JsonConvert.SerializeObject(query);

            }
        }
    }
}