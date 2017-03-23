using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using log4net;
using TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Shared;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Data
{
    public class EndpointDatabaseUtility: DatabaseUtility
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(EndpointDatabaseUtility));

        public EndpointDatabaseUtility(string dbConnectionString)
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

        public List<EndpointEntity> QueryEndpointsByGuid(string userGuid, string parentGuid)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            List<EndpointEntity> endpoints = new List<EndpointEntity>();
            string cmdText = "SELECT ChildGuid AS Guid FROM dbo.fn_TMCMSDK_Inventory_QueryEndpointsByGuid(@UserGuid, @ParentGuid) WHERE ChildType = 4";
            try
            {
                AddSqlParameter("UserGuid", SqlDbType.Char, userGuid);
                AddSqlParameter("ParentGuid", SqlDbType.Char, parentGuid);
                endpoints = ExecuteSqlReaderWithObject<EndpointEntity>(cmdText, CommandType.Text);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            if (endpoints.Count > 0)
            {
                m_Logger.DebugFormat("__{0}__: {1}: Query successfully", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            else
            {
                m_Logger.DebugFormat("__{0}__: {1}: No Query result ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return endpoints;
        }
        /*
        public string GetEnpointsByGuid(string guid)
        {
            m_logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            string result = string.Empty;
            string cmdText = "SELECT GUID FROM dbo.fn_AD_GetOUParentListStringByOUGuidListString(@OUGuidList)";
            try
            {
                AddSqlParameter("OUGuidList", SqlDbType.NVarChar, guidList);
                List<fn_AD_GetOUParentListStringByOUGuidListString> parentOUGuidList = ExecuteSqlReaderWithObject<fn_AD_GetOUParentListStringByOUGuidListString>(cmdText, CommandType.Text);
                m_logger.DebugFormat("__{0}__: {1}: Query fn_AD_GetOUParentListStringByOUGuidListString's result = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, parentOUGuidList.FirstOrDefault<fn_AD_GetOUParentListStringByOUGuidListString>().GUID);
                result = parentOUGuidList.FirstOrDefault<fn_AD_GetOUParentListStringByOUGuidListString>().GUID;
            }
            catch (Exception ex)
            {
                m_logger.ErrorFormat("__{0}__: {1}: Exception = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            if (!string.IsNullOrEmpty(result))
            {
                m_logger.DebugFormat("__{0}__: {1}: AD Query result = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, result);
            }
            else
            {
                m_logger.DebugFormat("__{0}__: {1}: No Query result ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
                NullAdResourceMisc nullAdResourceMisc = new NullAdResourceMisc();
                result = nullAdResourceMisc.Parents;
            }
            m_logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return result;
        }
        */

        #region Discrad the object
        ~EndpointDatabaseUtility()
        {
            Dispose(false);
        }
        #endregion
    }
}
