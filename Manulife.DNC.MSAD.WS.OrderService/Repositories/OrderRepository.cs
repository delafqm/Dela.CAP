using Dapper;
using DotNetCore.CAP;
using Manulife.DNC.MSAD.WS.Events;
using Manulife.DNC.MSAD.WS.OrderService.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public OrderDbContext DbContext { get; }
        public ICapPublisher CapPublisher { get; }
        public string ConnStr { get; } // For Dapper use

        public OrderRepository(OrderDbContext DbContext, ICapPublisher CapPublisher, string ConnStr)
        {
            this.DbContext = DbContext;
            this.CapPublisher = CapPublisher;
            this.ConnStr = ConnStr;
        }


        public async Task<bool> CreateOrderByEF(IOrder order)
        {
            using (var trans = DbContext.Database.BeginTransaction())
            {
                var orderEntity = new Order()
                {
                    ID = GenerateOrderID(),
                    OrderUserID = order.OrderUserID,
                    OrderTime = order.OrderTime,
                    OrderItems = null,
                    ProductID = order.ProductID // For demo use
                };

                DbContext.Orders.Add(orderEntity);
                await DbContext.SaveChangesAsync();

                // When using EF, no need to pass transaction
                var orderMessage = new OrderMessage()
                {
                    ID = orderEntity.ID,
                    OrderUserID = orderEntity.OrderUserID,
                    OrderTime = orderEntity.OrderTime,
                    OrderItems = null,
                    ProductID = orderEntity.ProductID // For demo use
                };
                
                //CapPublisher.Publish(EventConstants.EVENT_NAME_CREATE_ORDER, orderMessage);
                await CapPublisher.PublishAsync(EventConstants.EVENT_NAME_CREATE_ORDER, orderMessage);

                trans.Commit();
            }

            return true;
        }

        public async Task<bool> CreateOrderByDapper(IOrder order)
        {
            using (var conn = new MySqlConnection(ConnStr))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    // business code here
                    string sqlCommand = @"INSERT INTO orders(OrderID, OrderTime, OrderUserID, ProductID)
                                                                VALUES(@OrderID, @OrderTime, @OrderUserID, @ProductID)";
                    //IDataReader abc = await conn.ExecuteReaderAsync(sqlCommand);
                    //string ab;
                    //while (abc.Read())
                    //{
                    //    ab = abc["a1"].ToString();
                    //}
                    var query = conn.QueryAsync<Order>(sqlCommand);

                    order.ID = GenerateOrderID();
                    await conn.ExecuteAsync(sqlCommand, param: new
                    {
                        OrderID = order.ID,
                        OrderTime = DateTime.Now,
                        OrderUserID = order.OrderUserID,
                        ProductID = order.ProductID
                    }, transaction: trans);

                    // For Dapper/ADO.NET, need to pass transaction
                    var orderMessage = new OrderMessage()
                    {
                        ID = order.ID,
                        OrderUserID = order.OrderUserID,
                        OrderTime = order.OrderTime,
                        OrderItems = null,
                        MessageTime = DateTime.Now,
                        ProductID = order.ProductID // For demo use
                    };

                    await CapPublisher.PublishAsync(EventConstants.EVENT_NAME_CREATE_ORDER, orderMessage, trans);

                    trans.Commit();
                }
            }

            return true;
        }

        private string GenerateOrderID()
        {
            // TODO: Some business logic to generate Order ID
            return Guid.NewGuid().ToString();
        }

        private string GenerateEventID()
        {
            // TODO: Some business logic to generate Order ID
            return Guid.NewGuid().ToString();
        }
    }
}
