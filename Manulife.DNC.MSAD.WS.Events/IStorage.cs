using System;

namespace Manulife.DNC.MSAD.WS.Events
{
    public interface IStorage
    {
        string ID { get; set; }
        int StorageNumber { get; set; }
        DateTime CreatedTime { get; set; }
        DateTime UpdatedTime { get; set; }
    }
}
