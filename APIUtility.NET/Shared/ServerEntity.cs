using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUtility.NET.Shared
{
    public class ServerEntity
    {
        public string ServerID { get; set; }
        public string ServerName { get; set; }
        public string DisplayName { get; set; }
        public string ProductID { get; set; }
        public string ProductVersion { get; set; }
        public int       ServerType { get; set; }
        public string Protocol { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public int       ProxyEnable { get; set; }
        public string CreatedUserID { get; set; }
        public string Others { get; set; }
        public string ProxyID { get; set; }
        public string ProxyDisplayName { get; set; }
        public string ProxyProtocol { get; set; }
        public string ProxyServerName { get; set; }
        public string ProxyPort { get; set; }
        public string ProxyUserID { get; set; }
        public string ProxyPassword { get; set; }
        public SingleSignOnInfo SSO { get; set; }
    }

    public class SingleSignOnInfo
    {
        public int SSOInfo { get; set; }
        public Object SSOCookie { get; set; }
    }
}
