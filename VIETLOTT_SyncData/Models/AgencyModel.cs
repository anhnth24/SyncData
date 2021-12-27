using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIETLOTT_SyncData.Models
{
    public class AgencyModel
    {
        public List<AgencyModelData> Data { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        
    }

    public class AgencyModelData
    {
        public  string id { get; set; }
        public  string code { get; set; }
        public  string name { get; set; }
        public  string areacode { get; set; }
        public  string provincecode { get; set; }
        public  string districtcode { get; set; }
        public  string wardcode { get; set; }
        public  string wardid { get; set; }
        public  string address { get; set; }
        public  string taxCode { get; set; }
        public  string agencyCode { get; set; }
        public  string companyName { get; set; }
        public  string companyType { get; set; }
        public  string companyRepresent { get; set; }
        public  string companyRegulation { get; set; }
        public  string companyShareholder { get; set; }
        public  string companyDocument { get; set; }
        public  string agencyContract { get; set; }
        public  string represent { get; set; }
        public  string accountant { get; set; }

    }
}
