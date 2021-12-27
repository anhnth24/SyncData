using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIETLOTT_SyncData.Models
{
    public class BranchModel
    {
        public List<BranchModelData> Data { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class BranchModelData
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}