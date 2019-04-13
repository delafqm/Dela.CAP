using Manulife.DNC.MSAD.WS.Events;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manulife.DNC.MSAD.WS.StorageService.Models
{
    [Table("Storages")]
    public class Storage : IStorage
    {
        [Column("StorageID")]
        public string ID { get; set; }

        [Column("StorageNumber")]
        public int StorageNumber { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        [Column("UpdatedTime")]
        public DateTime UpdatedTime { get; set; }
    }
}
