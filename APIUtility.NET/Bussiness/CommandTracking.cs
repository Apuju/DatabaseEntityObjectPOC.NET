using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using log4net;
using APIUtility.NET.Shared;
using APIUtility.NET.Data;

namespace APIUtility.NET.Bussiness
{
    public class CommandTracking
    {
        private const string SUCCESS = "success", IN_PROGRESS = "in_progress", FAILURE = "failure";
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(CommandTracking));
        private readonly Dictionary<string, Tuple<int, int>> commandItemStatusDefinition = new Dictionary<string, Tuple<int, int>>()
        {
            { SUCCESS, new Tuple<int,int>(100,399)},
            { IN_PROGRESS, new Tuple<int,int>(400,699)},
            { FAILURE, new Tuple<int,int>(700,999)}
        };
        private string m_DbConnectionString = string.Empty;

        public CommandTracking(string dbConnectionString)
        {
            m_DbConnectionString = dbConnectionString;
        }

        public CommandTrackingEntity GetCommandTrackingByCommandTrackingID(string comandTrackingID)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            CommandTrackingEntity commandTracking = new CommandTrackingEntity();
            try
            {
                using (CommandTrackingDatabaseUtility db = new CommandTrackingDatabaseUtility(m_DbConnectionString))
                {
                    commandTracking = db.QueryCommandTrackingByCommandTrackingID(comandTrackingID);
                    commandTracking.Items = db.QueryCommandItemTrackingByCommandTrackingID(comandTrackingID);
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
            return commandTracking;
        }

        public CommandTrackingEntity CreateNewCommandTracking(CommandTrackingEntity commandTracking)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            try
            {
                commandTracking.TotalItems = commandTracking.Items.Count;
                commandTracking.TotalSuccess = commandTracking.Items.Count<CommandItemTrackingEntity>(i => ((int)i.Status > commandItemStatusDefinition[SUCCESS].Item1 && (int)i.Status < commandItemStatusDefinition[SUCCESS].Item2));
                commandTracking.TotalFailure = commandTracking.Items.Count<CommandItemTrackingEntity>(i => ((int)i.Status > commandItemStatusDefinition[FAILURE].Item1 && (int)i.Status < commandItemStatusDefinition[FAILURE].Item2));
                using (CommandTrackingDatabaseUtility db = new CommandTrackingDatabaseUtility(m_DbConnectionString))
                {
                    commandTracking = db.AddCommandItemTracking(db.AddCommandTracking(commandTracking));
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
            return commandTracking;
        }

        public CommandTrackingEntity UpdateCommandTracking(CommandTrackingEntity commandTracking)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            List<CommandItemTrackingEntity> newItems = new List<CommandItemTrackingEntity>();
            List<CommandItemTrackingEntity> existedItems = new List<CommandItemTrackingEntity>();
            try
            {
                using (CommandTrackingDatabaseUtility db = new CommandTrackingDatabaseUtility(m_DbConnectionString))
                {
                    List<CommandItemTrackingEntity> currentItems = db.QueryCommandItemTrackingByCommandTrackingID(commandTracking.CommandTrackingID);
                    foreach (CommandItemTrackingEntity item in commandTracking.Items)
                    {
                        if (item.ItemID != null)
                        {
                            m_Logger.DebugFormat("__{0}__: {1}: It is an item which is not a new one", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
                            if (currentItems.Count<CommandItemTrackingEntity>(i => (i.ItemID == item.ItemID) && (i.ReceiverID != item.ReceiverID || i.Status != item.Status || i.ErrorDescription != item.ErrorDescription)) > 0)
                            {
                                m_Logger.DebugFormat("__{0}__: {1}: It must be updated", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
                                existedItems.Add(item);
                            }
                        }
                        else
                        {
                            m_Logger.DebugFormat("__{0}__: {1}: It is a new item", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
                            newItems.Add(item);
                        }
                    }
                    commandTracking.Items = existedItems;
                    commandTracking = db.UpdateCommandItemTracking(commandTracking);
                    commandTracking.Items = newItems;
                    commandTracking = db.AddCommandItemTracking(commandTracking);
                    commandTracking.Items = db.QueryCommandItemTrackingByCommandTrackingID(commandTracking.CommandTrackingID);
                    commandTracking.TotalItems = commandTracking.Items.Count;
                    commandTracking.TotalSuccess = commandTracking.Items.Count<CommandItemTrackingEntity>(i => ((int)i.Status > commandItemStatusDefinition[SUCCESS].Item1 && (int)i.Status < commandItemStatusDefinition[SUCCESS].Item2));
                    commandTracking.TotalFailure = commandTracking.Items.Count<CommandItemTrackingEntity>(i => ((int)i.Status > commandItemStatusDefinition[FAILURE].Item1 && (int)i.Status < commandItemStatusDefinition[FAILURE].Item2));
                    commandTracking = db.UpdateCommandTracking(commandTracking);
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
            return commandTracking;
        }
    }
}
