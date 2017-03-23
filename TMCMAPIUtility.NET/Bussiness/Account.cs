using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using log4net;
using TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Shared;
using TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Data;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Bussiness
{
    public class Account
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(CommandTracking));
        private string m_DbConnectionString = string.Empty;

        public Account(string dbConnectionString)
        {
            m_DbConnectionString = dbConnectionString;
        }

        public LocalAccountEntity GetLocalSystemAccount()
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            LocalAccountEntity systemAccount = new LocalAccountEntity();
            try
            {
                using (AccountDatabaseUtility db = new AccountDatabaseUtility(m_DbConnectionString))
                {
                    systemAccount = db.QueryLocalSystemAccount();
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
            return systemAccount;
        }
    }
}
