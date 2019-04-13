using Manulife.DNC.MSAD.WS.Events;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manulife.DNC.MSAD.WS.DeliveryService.Models
{
    [Table("Deliveries")]
    public class Delivery : IDelivery
    {
        [Column("DeliveryID")]
        public string ID { get; set; }

        [Column("OrderID")]
        public string OrderID { get; set; }

        [Column("OrderUserID")]
        public string OrderUserID { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        [Column("UpdatedTime")]
        public DateTime UpdatedTime { get; set; }
    }
}
