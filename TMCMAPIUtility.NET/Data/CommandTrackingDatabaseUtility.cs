using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using log4net;
using TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Shared;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Data
{
    public class CommandTrackingDatabaseUtility: DatabaseUtility
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(CommandTrackingDatabaseUtility));

        public CommandTrackingDatabaseUtility(string dbConnectionString)
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

        public CommandTrackingEntity QueryCommandTrackingByCommandTrackingID(string comandTrackingID)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            List<CommandTrackingEntity> commandTracking = new List<CommandTrackingEntity>();
            string cmdText = "SELECT CT_ID, CT_ReceiverID, CT_UserAccessID, CT_MessageID, CT_ErrorDescription, CT_Parameters FROM tb_CommandTracking WHERE CT_ID = @ComandTrackingID";
            try
            {
                AddSqlParameter("ComandTrackingID", SqlDbType.Char, comandTrackingID);
                commandTracking = ExecuteSqlReaderWithObject<CommandTrackingEntity>(cmdText, CommandType.Text);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            if (commandTracking.Count > 0)
            {
                m_Logger.DebugFormat("__{0}__: {1}: Query successfully", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            else
            {
                m_Logger.DebugFormat("__{0}__: {1}: No Query result ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return commandTracking.FirstOrDefault<CommandTrackingEntity>();
        }

        public List<CommandItemTrackingEntity> QueryCommandItemTrackingByCommandTrackingID(string commandTrackingID)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            List<CommandItemTrackingEntity> commandItemTracking = new List<CommandItemTrackingEntity>();
            string cmdText = "SELECT CIT_ID, CIT_ReceiverID, CIT_MessageID, CIT_Status, CIT_ErrorDescription, CIT_Parameters FROM tb_CommandItemTracking WHERE CIT_CommandID = @ComandTrackingID";
            try
            {
                AddSqlParameter("ComandTrackingID", SqlDbType.Char, commandTrackingID);
                commandItemTracking = ExecuteSqlReaderWithObject<CommandItemTrackingEntity>(cmdText, CommandType.Text);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            if (commandItemTracking.Count > 0)
            {
                m_Logger.DebugFormat("__{0}__: {1}: Query successfully", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            else
            {
                m_Logger.DebugFormat("__{0}__: {1}: No Query result ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return commandItemTracking;
        }

        public CommandTrackingEntity AddCommandTracking(CommandTrackingEntity commandTracking)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            string cmdText = "sp_CreateCommandTracking";
            commandTracking.CommandTrackingID = Guid.NewGuid().ToString().ToUpper();
            m_Logger.DebugFormat("__{0}__: {1}: Add a Command Tracking. GUID = {1}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, commandTracking.CommandTrackingID);
            try
            {
                AddSqlParameter("cmdid", SqlDbType.Char, commandTracking.CommandTrackingID);
                //0: CTTG_UNKNOWN, 1: CTTG_ENTITY, 2: CTTG_SELF_TMS, 3: CTTG_CHILD_TMS
                AddSqlParameter("CT_TargetGroup", SqlDbType.Int, 1);
                AddSqlParameter("CT_ReceiverID", SqlDbType.Char, commandTracking.ReceiverID);
                AddSqlParameter("CT_UserAccessID", SqlDbType.NVarChar, commandTracking.UserAccessID);
                AddSqlParameter("CT_MessageID", SqlDbType.Int, commandTracking.MessageID);
                AddSqlParameter("CT_TotalCommandItems", SqlDbType.Int, commandTracking.TotalItems);
                AddSqlParameter("CT_TotalSuccess", SqlDbType.Int, commandTracking.TotalSuccess);
                AddSqlParameter("CT_ErrorDescription", SqlDbType.NVarChar, commandTracking.ErrorDescription);
                AddSqlParameter("CT_Location", SqlDbType.NVarChar, DBNull.Value);
                AddSqlParameter("CT_Parameters", SqlDbType.NVarChar, commandTracking.Parameters);
                AddSqlParameter("CT_CommandData", SqlDbType.Image, DBNull.Value);
                AddSqlParameter("CT_TotalFailure", SqlDbType.Int, commandTracking.TotalFailure);
                ExecuteSqlCommandNonQuery(cmdText, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return commandTracking;
        }

        public CommandTrackingEntity AddCommandItemTracking(CommandTrackingEntity commandTracking)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            string cmdText = "sp_CreateCommandItemTracking";
            try
            {
                foreach (CommandItemTrackingEntity item in commandTracking.Items)
                {
                    AddSqlParameter("cmdid", SqlDbType.Char, commandTracking.CommandTrackingID);
                    item.ItemID = Guid.NewGuid().ToString().ToUpper();
                    m_Logger.DebugFormat("__{0}__: {1}: Add a item. GUID = {1}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, item.ItemID);
                    AddSqlParameter("cmditemid", SqlDbType.Char, item.ItemID);
                    AddSqlParameter("CIT_ReceiverID", SqlDbType.Char, item.ReceiverID);
                    AddSqlParameter("CIT_MessageID", SqlDbType.Int, item.MessageID);
                    AddSqlParameter("CIT_Status", SqlDbType.Int, item.Status);
                    AddSqlParameter("CIT_ErrorDescription", SqlDbType.NVarChar, item.ErrorDescription);
                    AddSqlParameter("CIT_Location", SqlDbType.NVarChar, DBNull.Value);
                    AddSqlParameter("CIT_Parameters", SqlDbType.NVarChar, item.Parameters);
                    ExecuteSqlCommandNonQuery(cmdText, CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return commandTracking;
        }

        public CommandTrackingEntity UpdateCommandTracking(CommandTrackingEntity commandTracking)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            int affectedRows = 0;
            string cmdText = "UPDATE tb_CommandTracking SET CT_ReceiverID = @CT_ReceiverID, CT_TotalCommandItems = @CT_TotalCommandItems, CT_TotalSuccess = @CT_TotalSuccess, CT_TotalFailure = @CT_TotalFailure, CT_UpdateTime = GETDATE(), CT_ErrorDescription = @CT_ErrorDescription WHERE CT_ID = @cmdid";
            m_Logger.DebugFormat("__{0}__: {1}: Update a Command Tracking. GUID = {1}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, commandTracking.CommandTrackingID);
            try
            {
                AddSqlParameter("cmdid", SqlDbType.Char, commandTracking.CommandTrackingID);
                AddSqlParameter("CT_ReceiverID", SqlDbType.Char, commandTracking.ReceiverID);
                AddSqlParameter("CT_TotalCommandItems", SqlDbType.Int, commandTracking.TotalItems);
                AddSqlParameter("CT_TotalSuccess", SqlDbType.Int, commandTracking.TotalSuccess);
                AddSqlParameter("CT_TotalFailure", SqlDbType.Int, commandTracking.TotalFailure);
                AddSqlParameter("CT_ErrorDescription", SqlDbType.NVarChar, commandTracking.ErrorDescription);
                affectedRows = ExecuteSqlCommandNonQuery(cmdText, CommandType.Text);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            if (affectedRows == 1)
            {
                m_Logger.DebugFormat("__{0}__: {1}: Update successfully", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            else
            {
                m_Logger.DebugFormat("__{0}__: {1}: Unexpected result to update", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return commandTracking;
        }

        public CommandTrackingEntity UpdateCommandItemTracking(CommandTrackingEntity commandTracking)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            int affectedRows = 0;
            string cmdText = "UPDATE tb_CommandItemTracking SET CIT_ReceiverID = @CIT_ReceiverID, CIT_Status = @CIT_Status, CIT_ErrorDescription = @CIT_ErrorDescription, CIT_Parameters = @CIT_Parameters, CIT_UpdateTime = GETDATE() WHERE CIT_CommandID = @cmdid AND CIT_ID = @cmditemid";
            try
            {
                foreach (CommandItemTrackingEntity item in commandTracking.Items)
                {
                    AddSqlParameter("cmdid", SqlDbType.Char, commandTracking.CommandTrackingID);
                    m_Logger.DebugFormat("__{0}__: {1}: Add a item. GUID = {1}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, item.ItemID);
                    AddSqlParameter("cmditemid", SqlDbType.Char, item.ItemID);
                    AddSqlParameter("CIT_ReceiverID", SqlDbType.Char, item.ReceiverID);
                    AddSqlParameter("CIT_Status", SqlDbType.Int, item.Status);
                    AddSqlParameter("CIT_ErrorDescription", SqlDbType.NVarChar, item.ErrorDescription);
                    AddSqlParameter("CIT_Parameters", SqlDbType.NVarChar, item.Parameters);
                    affectedRows = ExecuteSqlCommandNonQuery(cmdText, CommandType.Text);
                    if (affectedRows == 1)
                    {
                        m_Logger.DebugFormat("__{0}__: {1}: Add successfully", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
                    }
                    else
                    {
                        m_Logger.DebugFormat("__{0}__: {1}: Unexpected result to add", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
                    }
                    affectedRows = 0;
                }
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception = {2} ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return commandTracking;
        }
    }
}
