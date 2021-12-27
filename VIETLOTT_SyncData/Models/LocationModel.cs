using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIETLOTT_SyncData.Models
{
    public class LocationModel
    {
        public List<LocationModelData> Data { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class LocationModelData
    {
        public string Id { get; set; }
        public string LocationType { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string locationDigitCode { get; set; }
        public string ParentCode { get; set; }
    }
}
