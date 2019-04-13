using Dapper;
using DotNetCore.CAP;
using Manulife.DNC.MSAD.Common;
using Manulife.DNC.MSAD.WS.Events;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.StorageService.Services
{
    public class OrderSubscriberService : IOrderSubscriberService, ICapSubscribe
    {
        private readonly string _connStr;
        
        public OrderSubscriberService(string connStr)
        {
            _connStr = connStr;
        }

        [CapSubscribe(EventConstants.EVENT_NAME_CREATE_ORDER)]
        public async Task ConsumeOrderMessage(OrderMessage message)
        {
            await Console.Out.WriteLineAsync($"[StorageService] Received message : {JsonHelper.SerializeObject(message)}");
            await UpdateStorageNumberAsync(message);
        }

        private async Task<bool> UpdateStorageNumberAsync(OrderMessage order)
        {
            //throw new Exception("test"); // just for demo use
            using (var conn = new MySqlConnection(_connStr))
            {
                string sqlCommand = @"UPDATE storages SET StorageNumber = StorageNumber - 1,
                                                                  UpdatedTime = @UpdateTime
                                                                WHERE StorageID = @ProductID";

                int count = await conn.ExecuteAsync(sqlCommand, param: new
                {
                    ProductID = order.ProductID,
                    UpdateTime= DateTime.Now
                });

                return count > 0;
            }
        }
    }
}
