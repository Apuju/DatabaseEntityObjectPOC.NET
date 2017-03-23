using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using Microsoft.Win32;
using System.Xml.XPath;
using TrendMicro.TMCM.Utilities.WebUtilities;
using System.Data.SqlClient;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Data
{
    public class ConfigUtility
    {
        private static string m_strConn;
        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(m_strConn))
            {
                try
                {
                    string strConn = string.Empty;
                    string strSQLServer = "(local)\\SQLEXPRESS";
                    string strDBName = "db_ControlManager";
                    bool bWindowsAuthentication = false;

                    RegistryKey rkRegTVCS = Registry.LocalMachine.OpenSubKey("SOFTWARE\\TrendMicro\\TVCS");
                    strSQLServer = (string)rkRegTVCS.GetValue("SQLServer");
                    bWindowsAuthentication = (string)rkRegTVCS.GetValue("SQLWinAccount") == "1" ? true : false;
                    string strHomeDirectory = (string)rkRegTVCS.GetValue("HomeDirectory");
                    strDBName = (string)rkRegTVCS.GetValue("DBName");

                    using (StreamReader srDataSourceReader = new StreamReader(new FileStream(Path.Combine(strHomeDirectory, "DataSource.xml"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        XPathDocument xpdDataSourceDocument = new XPathDocument(srDataSourceReader);
                        XPathNodeIterator xpniSourceNode = xpdDataSourceDocument.CreateNavigator().Select("/DataSource/Source");
                        if (xpniSourceNode == null)
                        {
                            return strConn;
                        }

                        xpniSourceNode.MoveNext();
                        string strUserID = xpniSourceNode.Current.GetAttribute("ID", "");
                        string strPassword = new EncryptDecrypt().NewDecryptStr(xpniSourceNode.Current.GetAttribute("Password", ""));

                        SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
                        if (bWindowsAuthentication)
                        {
                            // strConnString = String.Format("Persist Security Info=False;Integrated Security=SSPI;User ID={0};Password='{1}';Initial Catalog={2};Server={3};Pooling=False", strUserID, strPassword, strDBName, strSQLServer);
                            connBuilder.PersistSecurityInfo = false;
                            connBuilder.IntegratedSecurity = true;
                            connBuilder.UserID = strUserID;
                            connBuilder.Password = strPassword;
                            connBuilder.InitialCatalog = strDBName;
                            connBuilder.DataSource = strSQLServer;
                            connBuilder.Pooling = false;
                        }
                        else
                        {
                            //strConnString = String.Format("Persist Security Info=False;User ID='{0}';Password='{1}';Initial Catalog={2};Server={3};Pooling=False", strUserID, strPassword, strDBName, strSQLServer);
                            connBuilder.PersistSecurityInfo = false;
                            connBuilder.UserID = strUserID;
                            connBuilder.Password = strPassword;
                            connBuilder.InitialCatalog = strDBName;
                            connBuilder.DataSource = strSQLServer;
                            connBuilder.Pooling = false;
                        }
                        strConn = connBuilder.ConnectionString;
                    }
                    m_strConn = strConn;
                }
                catch
                {//Static method cannot invoke log4net instance, throw it to caller for logging.
                    throw;
                }
            }

            return m_strConn;
        }
    }
}
