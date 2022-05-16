using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using VIETLOTT_SyncData.Models;
using System.Net.Security;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.Win32;
using System.Data.Linq;

namespace VIETLOTT_SyncData
{
    public class Globals
    {
        public static SAPbobsCOM.Company CompanyAPI;
        public static AgencyModel zAgency;
        public static PosModel zPos;
        public static LocationModel zLocation;
        public static BranchModel zBranch;
        public static string path = "C:\\VietlottInterface\\LogInterface\\BP.xml";

        //private static string sCon;
        //private static string h;
        //private static string port;
        //private static string u;
        //private static string pass;
        private static string db;
        private static string SapServer;
        private static string LicenseServer;
        private static string SLDServer;
        private static string SapUser;
        private static string SapPass;

        //anhnth PC
        public static bool ConnectSapB1()
        {
            if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node").GetSubKeyNames().Contains("Vietlott"))
            {
                RegistryKey keyVietlott = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Vietlott");
                if (keyVietlott.GetValueNames().Contains("HanaDB"))
                    db = keyVietlott.GetValue("HanaDB").ToString();
                if (keyVietlott.GetValueNames().Contains("SapServer"))
                    SapServer = keyVietlott.GetValue("SapServer").ToString();
                if (keyVietlott.GetValueNames().Contains("LicenseServer"))
                    LicenseServer = keyVietlott.GetValue("LicenseServer").ToString();
                if (keyVietlott.GetValueNames().Contains("SLDServer"))
                    SLDServer = keyVietlott.GetValue("SLDServer").ToString();
                if (keyVietlott.GetValueNames().Contains("SapUser"))
                    SapUser = keyVietlott.GetValue("SapUser").ToString();
                if (keyVietlott.GetValueNames().Contains("SapPass"))
                    SapPass = keyVietlott.GetValue("SapPass").ToString();

                try
                {
                    SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company
                    {
                        CompanyDB = db,
                        Server = SapServer,
                        LicenseServer = LicenseServer,
                        SLDServer = SLDServer,
                        UserName = SapUser,
                        Password = SapPass,
                        DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB,
                        UseTrusted = false,
                        XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode

                    };

                    if (oCompany.Connected == true)
                        oCompany.Disconnect();

                    if (oCompany.Connect() != 0)
                    {
                        WriteLog("Error : " + oCompany.GetLastErrorDescription());
                        return false;
                    }
                    CompanyAPI = oCompany;
                    return true;
                }
                catch (Exception ex)
                {
                    WriteLog(ex.ToString());
                    return false;
                }
            }
            else
            {
                WriteLog("Missing something in RegistryKey !");
                return false;
            }
        }


        public static void WriteLog(string msg)
        {
            File.AppendAllText(String.Format("{0}\\{1}.txt", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), "Sync logs - " + DateTime.Now.ToString("yyyy-MM-dd")), String.Format("{0}: {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));
        }

        public static DataTable QueryToDataTable(string query)
        {
            try
            {
                SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rcs.DoQuery(query);
                StringReader theReader = new StringReader(rcs.GetAsXML());
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);
                return theDataSet.Tables[3];
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                return null;
            }
        }

        public static bool CheckBusinessPartner(string BPcode)
        {
            DataTable BP = QueryToDataTable("Select \"CardCode\" from OCRD");
            bool contains = BP.AsEnumerable().Any(row => BPcode == row.Field<String>("CardCode"));
            return contains;
        }

        public static bool CheckProject(string ProjCode)
        {
            DataTable P = QueryToDataTable("Select \"PrjCode\" from OPRJ");
            bool contains = P.AsEnumerable().Any(row => ProjCode == row.Field<String>("PrjCode"));
            return contains;
        }

        public static bool CheckLocation(string IdCode)
        {
            DataTable P = QueryToDataTable("Select \"ID\" from ZLOCATION");
            bool contains = P.AsEnumerable().Any(row => IdCode == row.Field<String>("ID"));
            return contains;
        }

        public static bool CheckBranch(string BraCode)
        {
            DataTable P = QueryToDataTable("Select \"CODE\" from ZBRANCH");
            bool contains = P.AsEnumerable().Any(row => BraCode == row.Field<String>("CODE"));
            return contains;
        }

        public static void UpdateProject(string BPcode)
        {
            string query = "Update OPRJ set \"PrjName\" = " + BPcode + ", where \"PrjCode\" = " + BPcode + " ";
            SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            rcs.DoQuery(query);
        }

        public static AgencyModel GetAgency()
        {
            DateTime today = DateTime.Today;
            var date = today.ToString("yyyyMMdd");
            WebRequest request = WebRequest.Create(Constants.Url.UrlAgency + date);

            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zAgency = json.ToObject<AgencyModel>();

            return zAgency;
        }

        public static void GetLocation()
        {
            WebRequest request = WebRequest.Create(Constants.Url.UrlLocation);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zLocation = json.ToObject<LocationModel>();

            string query;
            try
            {
                foreach (LocationModelData p in zLocation.Data)
                {
                    if (CheckLocation(p.Id) == false)
                    {
                        SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        query = "INSERT INTO ZLOCATION" +
                            "(ID, LOCATIONTYPE, LOCATIONCODE, LOCATIONNAME, LOCATIONDIGITCODE, PARENTCODE)   VALUES" +
                            "('" + p.Id + "', '" + p.LocationType + "', '" + p.LocationCode + "','" + p.LocationName + "', '" + p.locationDigitCode + "', '" + p.ParentCode + "')";
                        rcs.DoQuery(query);
                    }
                    else
                    {
                        SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        query = "UPDATE ZLOCATION SET  LOCATIONTYPE='" + p.LocationType + "', " +
                                                      "LOCATIONCODE='" + p.LocationCode + "', " +
                                                      "PARENTCODE='" + p.ParentCode + "', " +
                                                      "LOCATIONDIGITCODE='" + p.locationDigitCode + "', " +
                                                      "LOCATIONNAME='" + p.LocationName + "' " +
                                                      "WHERE  ID='" + p.Id + "'";
                        rcs.DoQuery(query);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        public static void GetBranch()
        {
            WebRequest request = WebRequest.Create(Constants.Url.UrlBranch);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zBranch = json.ToObject<BranchModel>();

            string query;
            try
            {
                foreach (BranchModelData p in zBranch.Data)
                {
                    if (p.Status != "update" && CheckBranch(p.Code) == false)
                    {
                        SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        query = "INSERT INTO ZBRANCH(ID, CODE, NAME, STATUS)VALUES('" + p.Id + "', '" + p.Code + "', '" + p.Name + "', '" + p.Status + "')";
                        rcs.DoQuery(query);
                    }
                    else
                    {
                        SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        query = "UPDATE VL.ZBRANCH SET  CODE='" + p.Code + "', NAME='" + p.Name + "', STATUS='" + p.Status + "'WHERE ID= '" + p.Id + "'";
                        rcs.DoQuery(query);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        //get pos by agency
        public static void GetPos(string AgencyCode)
        {
            WebRequest request = WebRequest.Create(Constants.Url.UrlPos + AgencyCode);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zPos = json.ToObject<PosModel>();

            SAPbobsCOM.Company oCompany = CompanyAPI;
            SAPbobsCOM.CompanyService Cs;
            SAPbobsCOM.IProjectsService Ps;
            SAPbobsCOM.IProject Pr;

            Cs = oCompany.GetCompanyService();
            Ps = Cs.GetBusinessService(SAPbobsCOM.ServiceTypes.ProjectsService);

            Pr = Ps.GetDataInterface(SAPbobsCOM.ProjectsServiceDataInterfaces.psProject);

            try
            {

                DataTable smsprov = QueryToDataTable("Select distinct U_PROVCODE,U_SMSPROV FROM OPRJ WHERE U_SMSPROV IS NOT NULL");
                foreach (PosModelData p in zPos.Data)
                {
                    //SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //string query = "Update OPRJ set \"U_BRANCODE\" ='" + p.branchcode + "' where \"PrjCode\"= '" + p.posCode + "'";
                    //rcs.DoQuery(query);
                    if (CheckProject(p.posCode) == false)
                    {

                        //DataRow[] dr = smsprov.Select("U_PROVCODE = " + p.provinceCode);
                        //int smsprovince = int.Parse(dr[0]["U_SMSPROV"].ToString());
                        string smsprovince = smsprov.AsEnumerable()
                                               .Where((row) => row.Field<string>("U_PROVCODE").Equals(p.provinceCode))
                                               .Select((row) => row.Field<string>("U_SMSPROV"))
                                               .FirstOrDefault();

                        Pr.Code = p.posCode;
                        Pr.Name = p.address.Substring(0, Math.Min(100, p.address.Length));
                        Pr.UserFields.Item("U_AgenID").Value = "C" + AgencyCode;
                        Pr.UserFields.Item("U_PROVCODE").Value = p.provinceCode;
                        Pr.UserFields.Item("U_POSTYPE").Value = p.posType;
                        Pr.UserFields.Item("U_DISTCODE").Value = p.districtCode;
                        //Pr.UserFields.Item("U_PROVINCE").Value = p.provinceCode;
                        Pr.UserFields.Item("U_AREA").Value = p.areaCode;
                        Pr.UserFields.Item("U_BRANCODE").Value = p.branchcode;
                        Pr.UserFields.Item("U_SMSPROV").Value = smsprovince;
                        if (AgencyCode == "55555555" || AgencyCode == "66666666" || AgencyCode == "88888888")
                            Pr.UserFields.Item("U_KENHPP").Value = "SMS";
                        else
                            Pr.UserFields.Item("U_KENHPP").Value = "TER";

                        Ps.AddProject((SAPbobsCOM.Project)Pr);
                        WriteLog("Create Project : " + p.posCode + " Success !");
                    }
                    else
                    {
                        WriteLog("Project : " + p.posCode + " Exists !");
                    }
                }


            }
            catch (Exception ex)
            {
                WriteLog(Globals.CompanyAPI.GetLastErrorDescription() + ex);
            }
        }

        //get pos by update date
        public static void GetPosByDate()
        {
            DateTime today = DateTime.Today;
            var date = today.ToString("yyyyMMdd");
            WebRequest request = WebRequest.Create(Constants.Url.UrlPosByDate + date);
            //WebRequest request = WebRequest.Create(Constants.Url.UrlPosAll);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zPos = json.ToObject<PosModel>();

            SAPbobsCOM.Company oCompany = CompanyAPI;
            SAPbobsCOM.CompanyService Cs;
            SAPbobsCOM.IProjectsService Ps;
            SAPbobsCOM.IProject Pr;

            Cs = oCompany.GetCompanyService();
            Ps = Cs.GetBusinessService(SAPbobsCOM.ServiceTypes.ProjectsService);

            Pr = Ps.GetDataInterface(SAPbobsCOM.ProjectsServiceDataInterfaces.psProject);

            try
            {
                DataTable smsprov = QueryToDataTable("Select distinct U_PROVCODE,U_SMSPROV FROM OPRJ WHERE U_SMSPROV IS NOT NULL");
                WriteLog("Pos Update,Add "+ DateTime.Today.ToString("yyyy-MM-dd") + " : " + zPos.Data.Count());
                foreach (PosModelData p in zPos.Data)
                {
                    try
                    {
                        string smsprovince = null;
                        if (!string.IsNullOrEmpty(p.provinceCode))
                        {
                            smsprovince = smsprov.AsEnumerable()
                                                   .Where((row) => row.Field<string>("U_PROVCODE").Equals(p.provinceCode))
                                                   .Select((row) => row.Field<string>("U_SMSPROV"))
                                                   .FirstOrDefault();
                        }


                        string posCode = p.posCode;
                        string name = p.address;
                        string AgenID = "C" + p.agencyCode;
                        string ProvCode = p.provinceCode;
                        string PosType = p.posType;
                        string Dist = p.districtCode;
                        string Area = p.areaCode;
                        string smsProv = smsprovince;
                        string branch = p.branchcode;
                        string kenhPP;
                        if (p.agencyCode == "55555555" || p.agencyCode == "66666666" || p.agencyCode == "88888888")
                            kenhPP = "SMS";
                        else
                            kenhPP = "TER";
                        if (CheckProject(p.posCode) == false)
                        {

                            //DataRow[] dr = smsprov.Select("U_PROVCODE = " + p.provinceCode);
                            //int smsprovince = int.Parse(dr[0]["U_SMSPROV"].ToString());


                            Pr.Code = p.posCode;
                            Pr.Name = p.address.Substring(0, Math.Min(100, p.address.Length));
                            Pr.UserFields.Item("U_AgenID").Value = "C" + p.agencyCode;
                            Pr.UserFields.Item("U_PROVCODE").Value = p.provinceCode;
                            Pr.UserFields.Item("U_POSTYPE").Value = p.posType;
                            Pr.UserFields.Item("U_DISTCODE").Value = p.districtCode;
                            //Pr.UserFields.Item("U_PROVINCE").Value = p.provinceCode;
                            Pr.UserFields.Item("U_AREA").Value = p.areaCode;
                            Pr.UserFields.Item("U_BRANCODE").Value = p.branchcode;
                            Pr.UserFields.Item("U_SMSPROV").Value = smsprovince;
                            if (p.agencyCode == "55555555" || p.agencyCode == "66666666" || p.agencyCode == "88888888")
                                Pr.UserFields.Item("U_KENHPP").Value = "SMS";
                            else
                                Pr.UserFields.Item("U_KENHPP").Value = "TER";

                            Ps.AddProject((SAPbobsCOM.Project)Pr);
                            WriteLog("Create Project : " + p.posCode + " Success !");
                        }
                        else
                        {
                            string query = "UPDATE OPRJ SET \"U_PROVCODE\" = '" + ProvCode + "'" +
                                ",\"U_POSTYPE\" = '" + PosType + "'" +
                                ",\"U_DISTCODE\" = '" + Dist + "'" +
                                ",\"U_AREA\" = '" + Area + "'" +
                                ",\"U_SMSPROV\" = '" + smsProv + "'" +
                                ",\"U_BRANCODE\" = '" + branch + "'" +
                                ",\"U_KENHPP\" = '" + kenhPP + "' WHERE \"PrjCode\" = '" + posCode + "'";
                            SAPbobsCOM.Recordset rcs = CompanyAPI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            rcs.DoQuery(query);
                            WriteLog("Project : " + p.posCode + " Updated !");
                        }
                    }
                    catch (Exception e)
                    {
                        WriteLog(Globals.CompanyAPI.GetLastErrorDescription() + e.ToString() + p.posCode);
                    }
                }
                WriteLog("Done !");

            }
            catch (Exception ex)
            {
                WriteLog(Globals.CompanyAPI.GetLastErrorDescription() + ex);
            }
        }

        public static void GetAgencyByDate()
        {
            DateTime today = DateTime.Today;
            var date = today.ToString("yyyyMMdd");
            WebRequest request = WebRequest.Create(Constants.Url.UrlAgencyDaily+date);
            //WebRequest request = WebRequest.Create(Constants.Url.UrlAgencyDaily + "20220429");
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zAgency = json.ToObject<AgencyModel>();

           
            WriteLog("Agency Update,Add " + DateTime.Today.ToString("yyyy-MM-dd") + " : " + zAgency.Data.Count());
            foreach (AgencyModelData m in zAgency.Data)
            {
                try
                {
                    SAPbobsCOM.Company oCompany = CompanyAPI;
                    SAPbobsCOM.BusinessPartners BP;
                    BP = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                    long result;
                    if (Globals.CheckBusinessPartner("C" + m.agencyCode) == false)
                    {
                        BP.CardCode = "C" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                        if (m.address.Length > 100)
                            BP.FreeText = m.address;
                        else
                            BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        //BP.LinkedBusinessPartner = null;
                        result = BP.Add();
                        if (result == 0)
                        {
                            Globals.WriteLog("Add Customer : " + "C" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }
                    else
                    {
                        BP.GetByKey("C" + m.agencyCode);
                        //BP.CardCode = "C" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                        if (m.address.Length > 100)
                            BP.FreeText = m.address;
                        else
                            BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        result = BP.Update();
                        if (result == 0)
                        {
                            Globals.WriteLog("Update Customer : " + "C" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }
                    if (Globals.CheckBusinessPartner("S" + m.agencyCode) == false)
                    {

                        BP.CardCode = "S" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                        if (m.address.Length > 100)
                            BP.FreeText = m.address;
                        else
                            BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        BP.LinkedBusinessPartner = "C" + m.agencyCode;
                        result = BP.Add();
                        if (result == 0)
                        {
                            Globals.WriteLog("Add Vendor : " + "S" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }
                    else
                    {
                        BP.GetByKey("S" + m.agencyCode);
                        //BP.CardCode = "S" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                        if (m.address.Length > 100)
                            BP.FreeText = m.address;
                        else
                            BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        result = BP.Update();
                        if (result == 0)
                        {
                            Globals.WriteLog("Update Vendor : " + "S" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }


                }
                catch (Exception e)
                {
                    WriteLog(e.ToString());
                }
            }
            WriteLog("Done !");
        }

        public static void BusinessPartnerAndProject()
        {
            DateTime today = DateTime.Today;
            var date = today.ToString("yyyyMMdd");
            WebRequest request = WebRequest.Create(Constants.Url.UrlAgency);
            //WebRequest request = WebRequest.Create(Constants.Url.getP);
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;

            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            JObject json = JObject.Parse(responseFromServer);
            zAgency = json.ToObject<AgencyModel>();

            SAPbobsCOM.Company oCompany = CompanyAPI;
            SAPbobsCOM.BusinessPartners BP;
            BP = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            long result;

            foreach (AgencyModelData m in zAgency.Data)
            {
                try
                {
                    if (Globals.CheckBusinessPartner("C" + m.agencyCode) == false)
                    {
                        BP.CardCode = "C" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                        BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        result = BP.Add();
                        if (result == 0)
                        {
                            Globals.WriteLog("Add Customer : " + "C" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }
                    else
                    {
                        BP.GetByKey("C" + m.agencyCode);
                        //BP.CardCode = "C" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                        BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        result = BP.Update();
                        if (result == 0)
                        {
                            Globals.WriteLog("Update Customer : " + "C" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }
                    if (Globals.CheckBusinessPartner("S" + m.agencyCode) == false)
                    {

                        BP.CardCode = "S" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                        BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        BP.LinkedBusinessPartner = "C" + m.agencyCode;
                        result = BP.Add();
                        if (result == 0)
                        {
                            Globals.WriteLog("Add Vendor : " + "S" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }
                    else
                    {
                        BP.GetByKey("S" + m.agencyCode);
                        //BP.CardCode = "S" + m.agencyCode;
                        BP.CardName = m.companyName;
                        BP.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                        BP.Address = m.address;
                        BP.FederalTaxID = m.taxCode;
                        result = BP.Update();
                        if (result == 0)
                        {
                            Globals.WriteLog("Update Vendor : " + "S" + m.agencyCode + " Success!");
                        }
                        else
                        {
                            Globals.WriteLog(Globals.CompanyAPI.GetLastErrorDescription());
                        }
                    }


                }
                catch (Exception e)
                {
                    WriteLog(e.ToString());
                }
                GetPos(m.agencyCode);
            }

        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
