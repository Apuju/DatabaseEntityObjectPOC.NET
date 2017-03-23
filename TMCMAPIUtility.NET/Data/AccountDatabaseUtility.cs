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
    public class AccountDatabaseUtility : DatabaseUtility
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(AccountDatabaseUtility));

        public AccountDatabaseUtility(string dbConnectionString)
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

        public LocalAccountEntity QueryLocalSystemAccount()
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            List<LocalAccountEntity> systemAccounts = new List<LocalAccountEntity>();
            string cmdText = "SELECT Guid, ID FROM tb_Account WHERE Type = 1 AND Category = 1";
            try
            {
                systemAccounts = ExecuteSqlReaderWithObject<LocalAccountEntity>(cmdText, CommandType.Text);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            if (systemAccounts.Count > 0)
            {
                m_Logger.DebugFormat("__{0}__: {1}: Query successfully", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            else
            {
                m_Logger.DebugFormat("__{0}__: {1}: No Query result ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return systemAccounts.FirstOrDefault<LocalAccountEntity>();
        }
    }
}
