using DogNet.Repositories;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingdongCall.Models
{
    [TableName("DingDongCall_Log")]
    [PrimaryKey("Id")]
    public class LogEntity : Repository<LogEntity>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Operation { get; set; }
        public DateTime InputTime { get; set; }
    }
}