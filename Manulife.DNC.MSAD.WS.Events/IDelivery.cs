using System;

namespace Manulife.DNC.MSAD.WS.Events
{
    public interface IDelivery
    {
        string ID { get; set; }
        string OrderID { get; set; }
        string OrderUserID { get; set; }
        DateTime CreatedTime { get; set; }
        DateTime UpdatedTime { get; set; }
    }
}
