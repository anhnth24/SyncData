using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIETLOTT_SyncData.Constants
{
    public class Url
    {
        //shttps://vietlott-prod.xplat.fpt.com.vn/api/getagency
        public const string Uri = "https://vietlott-prod.xplat.fpt.com.vn/api/";

        //public const string UrlAgency = "https://vietlott-prod.xplat.fpt.com.vn/api/getagency";
        //public const string UrlPos = "https://vietlott-prod.xplat.fpt.com.vn/api/getpos?AgencyCode=";


        //public const string UrlAgency = "https://10.14.185.6:44340/api/getagency?UpdatedDate=";
        public const string UrlAgency = "https://erpapi.vietlott.vn/api/getagency?AccessToken=087e0ea4c6db4320820492141f94f29f";
        public const string UrlAgencyDaily = "https://erpapi.vietlott.vn/api/getagency?AccessToken=087e0ea4c6db4320820492141f94f29f&UpdatedDate=";
        //public const string UrlPos = "https://10.14.185.6:44340/api/getpos?AgencyCode=";
        public const string UrlPos = "https://erpapi.vietlott.vn/api/getpos?AccessToken=087e0ea4c6db4320820492141f94f29f&AgencyCode=";
        //url get pos by date
        public const string UrlPosByDate = "https://erpapi.vietlott.vn/api/getpos?AccessToken=087e0ea4c6db4320820492141f94f29f&UpdatedDate=";
        public const string UrlPosAll = "https://erpapi.vietlott.vn/api/getpos?AccessToken=087e0ea4c6db4320820492141f94f29f";
        public const string UrlLocation = "https://vietlott-prod.xplat.fpt.com.vn/api/getlocation";

        public const string UrlBranch = "https://vietlott-prod.xplat.fpt.com.vn/api/getbranch";


        public const string getP = "https://erpapi.vietlott.vn/api/getpos?AccessToken=087e0ea4c6db4320820492141f94f29f&AgencyCode=10100006";
    }
}
