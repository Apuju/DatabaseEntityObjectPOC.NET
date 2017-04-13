using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using log4net;
using APIUtility.NET.Shared;

namespace APIUtility.NET.Data
{
    public class ServerDatabaseUtility: DatabaseUtility
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(ServerDatabaseUtility));

        public ServerDatabaseUtility(string dbConnectionString)
            :base(dbConnectionString)
        {
            try
            {
                OpenConnection();
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Create DatabaseUtility failed = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
        }

        public string[] QueryServerGuidListByProductType(string strLogonUserGuid, string strProductID, string strPluginID)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            m_Logger.DebugFormat("__{0}__: {1}: strLogonUserGuid={2}, strProductID={3}, strPluginID={4}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, strLogonUserGuid, strProductID, strPluginID);

            List<string> ServerGuidList = new List<string>();
            string cmdText = "dbo.sp_SDK_QueryServerGuidListByProductType";
            try
            {
                AddSqlParameter("LogonUserGuid", SqlDbType.Char, string.IsNullOrEmpty(strLogonUserGuid) ? DBNull.Value : (Object)strLogonUserGuid);
                AddSqlParameter("ProductID", SqlDbType.Char, string.IsNullOrEmpty(strProductID) ? DBNull.Value : (Object)strProductID);
                AddSqlParameter("PluginID", SqlDbType.Char, string.IsNullOrEmpty(strPluginID) ? DBNull.Value : (Object)strPluginID);
                DataTable dtResult = new DataTable();
                ExecuteSqlReaderWithTable(cmdText, CommandType.StoredProcedure, ref dtResult);
                foreach (DataRow dr in dtResult.Rows)
                {
                    if (dr["ServerID"] != null && !string.IsNullOrEmpty(dr["ServerID"].ToString()))
                        ServerGuidList.Add(dr["ServerID"].ToString());
                }
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }

            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return ServerGuidList.ToArray<string>();
        }
    }
}
