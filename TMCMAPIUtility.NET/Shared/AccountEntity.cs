using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Shared
{
    public class LocalAccountEntity
    {
        private string m_Guid = string.Empty;
        private string m_UserName = string.Empty;

        [DisplayName("Guid")]
        public string Guid
        {
            get
            {
                return m_Guid;
            }
            set
            {
                m_Guid = value;
            }
        }
        [DisplayName("ID")]
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
            }
        }
    }
}
