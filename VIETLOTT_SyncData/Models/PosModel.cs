using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIETLOTT_SyncData.Models
{
    public class PosModel
    {
        public List<PosModelData> Data { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

    }

    public class PosModelData
    {
        public string id { get; set; }
        public string posCode { get; set; }
        public string posName { get; set; }
        public string posType { get; set; }
        public string agencyCode { get; set; }
        public string subAgencyCode { get; set; }
        public string areaCode { get; set; }
        public string provinceCode { get; set; }
        public string districtCode { get; set; }
        public string wardCode { get; set; }
        public string address { get; set; }
        public string posNumber { get; set; }
        public string certificate { get; set; }
        public string contractNumber { get; set; }
        public string updateContractNumber { get; set; }
        public string updateContractDate { get; set; }
        public string releaseContractNumber { get; set; }
        public string releaseContractDate { get; set; }
        public string terminalCount { get; set; }
        public string branchcode { get; set; }

    }
}
