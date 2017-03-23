using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using log4net;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Data
{
    public class DatabaseUtility: IDisposable
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(DatabaseUtility));
        private Boolean m_disposed = false;
        private const int SQL_TIMEOUT = 30;
        private string m_ConnectionString = null;
        private SqlConnection m_Connection = null;
        private SqlCommand m_Command = null;
        private SqlDataReader m_Reader = null;
        private List<SqlParameter> m_Parameters = new List<SqlParameter>();

        public DatabaseUtility() { }

        public DatabaseUtility(string dbConnectionString)
        {
            try
            {
                ConnectionString = dbConnectionString;
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Create SqlConnection failed = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
        }

        #region Data object convert
        public DataTable ConvertToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        public List<T> ConvertToList<T>(DataTable dtData) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (DataRow row in dtData.Rows)
            {
                var item = CreateItemFromRow<T>(row, properties);
                result.Add(item);
            }

            return result;
        }
        private T CreateItemFromRow<T>(DataRow row, List<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                property.SetValue(item, row[property.Name], null);
            }
            return item;
        }
        #endregion

        #region DB Connection Utility
        protected string ConnectionString
        {
            get
            {
                return m_ConnectionString;
            }
            set
            {
                try
                {
                    m_ConnectionString = value;
                    SQLConnection = new SqlConnection(m_ConnectionString);
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorFormat("__{0}__: {1}: Create SqlConnection failed = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                    throw;
                }
            }
        }
        protected SqlConnection SQLConnection
        {
            get
            {
                return m_Connection;
            }
            set
            {
                m_Connection = value;
            }
        }
        protected void OpenConnection()
        {
            try
            {
                SQLConnection.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Command Utility
        protected SqlCommand SQLCommand
        {
            get
            {
                return m_Command;
            }
            set
            {
                m_Command = value;
            }
        }
        protected List<SqlParameter> SQLParameters
        {
            get
            {
                return m_Parameters;
            }
            set
            {
                m_Parameters = value;
            }
        }
        protected void AddSQLParameter(SqlParameter parameter)
        {
            m_Parameters.Add(parameter);
        }
        protected void ClearSQLParameters()
        {
            m_Parameters.Clear();
        }
        protected void AddSqlParameter(string parameterName, SqlDbType dbType, object value)
        {
            AddSQLParameter(new SqlParameter() { ParameterName = "@" + parameterName, SqlDbType = dbType, Value = value });
        }
        protected void InitSqlCommand(string command, SqlParameter[] parameters, CommandType type, SqlConnection connection)
        {
            SQLCommand = new SqlCommand(command, connection);
            SQLCommand.CommandType = type;
            SQLCommand.CommandTimeout = SQL_TIMEOUT;
            SQLCommand.Parameters.AddRange(parameters);
        }
        protected int ExecuteNonQuery(string sqlCommand, CommandType type, SqlTransaction transaction)
        {
            int affectedRows = 0;
            try
            {
                InitSqlCommand(sqlCommand, SQLParameters.ToArray(), type, SQLConnection);
                m_Logger.DebugFormat("__{0}__: {1}: Command Type = {2} , SQL Command = {3}, Parameters = {4}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, SQLCommand.CommandType, SQLCommand.CommandText, SQLCommand.Parameters.Count);
                SQLCommand.Transaction = transaction;
                affectedRows = SQLCommand.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception)
                {
                    throw;
                }
                throw;
            }
            finally
            {
                ClearSQLParameters();
                SQLCommand.Parameters.Clear();
            }
            return affectedRows;
        }
        protected virtual int ExecuteSqlCommandNonQuery(string sqlCommand, CommandType type)
        {
            int affectedRows = 0;
            try
            {
                m_Logger.DebugFormat("__{0}__: {1}: SQL Command = {2}: ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, sqlCommand);
                SqlTransaction transaction = SQLConnection.BeginTransaction();
                affectedRows = ExecuteNonQuery(sqlCommand, type, transaction);
                transaction.Dispose();
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: SQL Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            return affectedRows;
        }
        #endregion

        #region Reader Utility
        protected SqlDataReader SQLReader
        {
            get
            {
                return m_Reader;
            }
            set
            {
                m_Reader = value;
            }
        }
        protected void ExecuteSqlReader(string sqlCommand, CommandType type)
        {
            try
            {
                InitSqlCommand(sqlCommand, SQLParameters.ToArray(), type, SQLConnection);
                m_Logger.DebugFormat("__{0}__: {1}: Command Type = {2} , SQL Command = {3}, Parameters = {4}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, SQLCommand.CommandType, SQLCommand.CommandText, SQLCommand.Parameters.Count);
                SQLReader = SQLCommand.ExecuteReader();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ClearSQLParameters();
                SQLCommand.Parameters.Clear();
            }
        }
        protected void CloseSqlReader()
        {
            SQLCommand.Cancel();
            SQLReader.Close();
        }
        protected virtual void ExecuteSqlReaderWithTable(string sqlCommand, CommandType type, ref DataTable data)
        {
            try
            {
                m_Logger.DebugFormat("__{0}__: {1}: SQL Command = {2}: ", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, sqlCommand);
                ExecuteSqlReader(sqlCommand, type);
                data.Load(SQLReader);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: SQL Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            finally
            {
                CloseSqlReader();
            }
        }
        protected virtual void ExecuteSqlReaderWithDataSet(string sqlCommand, CommandType type, ref DataSet data)
        {
            List<DataTable> tables = new List<DataTable>();
            foreach (DataTable table in data.Tables)
            {
                tables.Add(table);
            }
            try
            {
                ExecuteSqlReader(sqlCommand, type);
                data.Load(SQLReader, LoadOption.OverwriteChanges, tables.ToArray());
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: SQL Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            finally
            {
                CloseSqlReader();
            }
        }
        protected virtual List<T> ExecuteSqlReaderWithObject<T>(string sqlCommand, CommandType type)
            where T : new()
        {
            m_Logger.DebugFormat("__{0}__: {1}: SQL Command = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, sqlCommand);
            Type entityType = typeof(T);
            m_Logger.DebugFormat("__{0}__: {1}: entityType.FullName = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, entityType.FullName);
            List<T> entitys = new List<T>();
            Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = entityType.GetProperties();
            m_Logger.DebugFormat("__{0}__: {1}: properties.Length = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, properties.Length);
            foreach (PropertyInfo info in properties)
            {
                m_Logger.DebugFormat("__{0}__: {1}: Property Info = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, info.Name);
                if (Attribute.IsDefined(info, typeof(DisplayNameAttribute)))
                {
                    DisplayNameAttribute displayName = (DisplayNameAttribute)Attribute.GetCustomAttribute(info, typeof(DisplayNameAttribute));
                    if (displayName != null && !string.IsNullOrWhiteSpace(displayName.DisplayName))
                    {
                        m_Logger.DebugFormat("__{0}__: {1}: Display Name = {2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, displayName.DisplayName);
                        hashtable[displayName.DisplayName.ToUpper()] = info;
                    }
                    else
                        hashtable[info.Name.ToUpper()] = info;
                }
                else
                    hashtable[info.Name.ToUpper()] = info;
            }
            try
            {
                ExecuteSqlReader(sqlCommand, type);
                if (SQLReader.HasRows)
                {
                    while (SQLReader.Read())
                    {
                        T newObject = new T();
                        for (int index = 0; index < SQLReader.FieldCount; index++)
                        {
                            PropertyInfo info = (PropertyInfo)hashtable[SQLReader.GetName(index).ToUpper()];
                            if ((info != null) && info.CanWrite)
                            {
                                var value = SQLReader.GetValue(index);
                                if (info.PropertyType.IsEnum)
                                    info.SetValue(newObject, Enum.ToObject(info.PropertyType, value), null);
                                else
                                    info.SetValue(newObject, value == DBNull.Value ? null : value, null);
                            }
                        }
                        entitys.Add(newObject);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: SQL Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            finally
            {
                CloseSqlReader();
            }
            return entitys;
        }
        #endregion

        #region Discrad the object
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(Boolean disposing)
        {
            if (m_disposed)
            {
                return;
            }
            if (disposing)
            {
                m_ConnectionString = null;
                if (m_Command != null)
                {
                    m_Command.Cancel();
                }
                if (m_Reader != null)
                {
                    m_Reader.Close();
                }
                if (m_Connection != null)
                {
                    m_Connection.Close();
                }
            }
            m_disposed = true;
        }
        ~DatabaseUtility()
        {
            Dispose(false);
        }
        #endregion
    }
}
