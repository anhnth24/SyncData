using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using VIETLOTT_SyncData.Models;
using VIETLOTT_SyncData;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Data;

namespace VIETLOTT_SyncData
{
    public partial class CallApi
    {
        public static SAPbobsCOM.Company CompanyAPI;

        public void SyncData()
        {
            if (Globals.ConnectSapB1() == true)
            {
                Globals.GetAgency();
                Globals.BusinessPartnerAndProject();
            }
                
        }
        //public static void ConnectSSL()
        //{
        //    if (ConnectSapB1() == true)
        //    {
        //        //Globals.ExportJE("10100001");
        //        WebRequest request = WebRequest.Create(Constants.Url.UrlAgency);
        //        request.Proxy = null;
        //        request.Credentials = CredentialCache.DefaultCredentials;

        //        ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        Stream dataStream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(dataStream);
        //        var responseFromServer = reader.ReadToEnd();
        //        JObject json = JObject.Parse(responseFromServer);
        //        AgencyModel model = json.ToObject<AgencyModel>();

        //        SAPbobsCOM.Company oCompany = CompanyAPI;
        //        SAPbobsCOM.Documents OCRD = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
        //        long result;


        //        foreach (AgencyModelData m in model.Data)
        //        {
        //            if (Globals.CheckBusinessPartner("C"+m.code) == false)
        //            {
        //                OCRD.CardCode = "C" + m.code;
        //                result = OCRD.Add();
        //                if (result == 0)
        //                {
        //                    Globals.WriteLog("Add Business Partner : " + "C" + m.code + " Success!");
        //                }
        //                else
        //                {
        //                    Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
        //                }
        //            }
        //        }
        //    }
        //    //var list = JsonConvert.DeserializeObject<List<AgencyModel>>(responseFromServer);
        //}
        //public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    return true;
        //}

        //public static bool ConnectSapB1()
        //{
        //    try
        //    {
        //        SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company
        //        {
        //            CompanyDB = Constants.SapConnectionInfo.CompanyDB,
        //            Server = Constants.SapConnectionInfo.Server,
        //            LicenseServer = Constants.SapConnectionInfo.LicenseServer,
        //            SLDServer = Constants.SapConnectionInfo.SLDServer,
        //            UserName = Constants.SapConnectionInfo.UserName,
        //            Password = Constants.SapConnectionInfo.Password,
        //            DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB,
        //            UseTrusted = false,
        //            XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode

        //        };

        //        if (oCompany.Connected == true)
        //            oCompany.Disconnect();

        //        if (oCompany.Connect() != 0)
        //        {
        //            Globals.WriteLog("Error : " + oCompany.GetLastErrorDescription());
        //            return false;
        //        }
        //        CompanyAPI = oCompany;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Globals.WriteLog(ex.ToString());
        //        return false;
        //    }
        //}
    }
}
//Constants.Url.UrlAgency