using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using log4net;
using TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Data;
using TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Shared;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Bussiness
{
    public class Server
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(Server));
        private string m_DbConnectionString = string.Empty;

        public Server(string dbConnectionString)
        {
            m_DbConnectionString = dbConnectionString;
        }

        //public List<ServerEntity> GetServerInfoByGuid(string[] GuidList, bool isWithPassword=false, bool bNeedSSO=false)
        //{
        //    m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
        //    DataTable dtResult = new DataTable();
        //    List<ServerEntity> ServerInfos = new List<ServerEntity>();
        //    string strGuidList = String.Join(",", GuidList);
        //    m_Logger.DebugFormat("__{0}__: {1}: strGuidList={2}, isWithPassword={3}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, strGuidList, isWithPassword.ToString());
        //    dtResult = ServerList.GetServer(m_DbConnectionString, strGuidList, isWithPassword);

        //    if (dtResult.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dtResult.Rows)
        //        {
        //            ServerEntity se = new ServerEntity();

        //            se.ServerID = (dr["ServerID"] == null) ? null : dr["ServerID"].ToString();
        //            se.ServerName = (dr["ServerName"] == null) ? null : dr["ServerName"].ToString();
        //            se.DisplayName = (dr["DisplayName"] == null) ? null : dr["DisplayName"].ToString();
        //            se.ProductID = (dr["ProductID"] == null) ? null : dr["ProductID"].ToString();
        //            se.ProductVersion = (dr["ProductVersion"] == null) ? null : dr["ProductVersion"].ToString();
        //            se.ServerType = (dr["ServerType"] == null) ? -1 : Convert.ToInt32(dr["ServerType"].ToString());
        //            se.Protocol = (dr["Protocol"] == null) ? null : dr["Protocol"].ToString();
        //            se.Host = (dr["Host"] == null) ? null : dr["Host"].ToString();
        //            se.Port = (dr["Port"] == null) ? null : dr["Port"].ToString();
        //            se.UserID = (dr["UserID"] == null) ? null : dr["UserID"].ToString();
        //            se.Password = (dr["Password"] == null) ? null : dr["Password"].ToString();
        //            se.ProxyEnable = (dr["ProxyEnable"] == null) ? 0 : Convert.ToInt32(dr["ProxyEnable"].ToString());
        //            se.CreatedUserID = (dr["CreatedUserID"] == null) ? null : dr["CreatedUserID"].ToString();
        //            se.Others = (dr["Others"] == null) ? null : dr["Others"].ToString();
        //            se.ProxyID = (dr["ProxyID"] == null) ? null : dr["ProxyID"].ToString();
        //            se.ProxyDisplayName = (dr["ProxyDisplayName"] == null) ? null : dr["ProxyDisplayName"].ToString();
        //            se.ProxyProtocol = (dr["ProxyProtocol"] == null) ? null : dr["ProxyProtocol"].ToString();
        //            se.ProxyServerName = (dr["ProxyServerName"] == null) ? null : dr["ProxyServerName"].ToString();
        //            se.ProxyPort = (dr["ProxyPort"] == null) ? null : dr["ProxyPort"].ToString();
        //            se.ProxyUserID = (dr["ProxyUserID"] == null) ? null : dr["ProxyUserID"].ToString();
        //            se.ProxyPassword = (dr["ProxyPassword"] == null) ? null : dr["ProxyPassword"].ToString();

        //            SingleSignOnInfo ssoInfo = new SingleSignOnInfo();
        //            if (string.IsNullOrEmpty(dr["UserID"].ToString()) && bNeedSSO)
        //            {
        //                ssoInfo.SSOInfo = 1;
        //                string installPath = string.Empty;
        //                SystemConfigurationXmlWrapper sysCfg = new SystemConfigurationXmlWrapper(true);
        //                installPath = sysCfg.GetValueByParameterName("m_strTMS_InstallPath");
        //                Dictionary<string, string> dictSSOParameters = ServerList.GetSSOParameters(m_DbConnectionString, se.ServerID, installPath);
        //                // remove the SSO parameter used by policy deployment
        //                string host = string.Empty;
        //                if (dictSSOParameters.ContainsKey("TargetServerIP"))
        //                {
        //                    host = dictSSOParameters["TargetServerIP"]; // got from Uri.Host which is the hostname without port number
        //                    dictSSOParameters.Remove("TargetServerIP");
        //                }
        //                if (host != string.Empty)
        //                    se.Host = host;
        //                ssoInfo.SSOCookie = dictSSOParameters;
        //            }
        //            else
        //            {
        //                ssoInfo.SSOInfo = 0;
        //                ssoInfo.SSOCookie = null;
        //            }
        //            se.SSO = ssoInfo;

        //            ServerInfos.Add(se);
        //        }
        //    }
        //    m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
        //    return ServerInfos;
        //}

        public string[] GetServerGuidListByProductType(string strLogonUserGuid, string strProductID, string strPluginID)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            m_Logger.DebugFormat("__{0}__: {1}: strLogonUserGuid={2}, strProductID={3}, strPluginID={4}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, strLogonUserGuid, strProductID, strPluginID);
            string[] ServerGuidList = { };
            try
            {
                using (ServerDatabaseUtility serverDB = new ServerDatabaseUtility(m_DbConnectionString))
                {
                    ServerGuidList = serverDB.QueryServerGuidListByProductType(strLogonUserGuid, strProductID, strPluginID);
                }
            }
            catch (SqlException ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: SQL Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }

            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return ServerGuidList;
        }
    }
}
