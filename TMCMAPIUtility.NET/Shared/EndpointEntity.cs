using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendMicro.TMCM.Utilities.TMCMUtilities.TMCMAPIUtility.NET.Shared
{
    public class EndpointEntity
    {
        private string m_Guid = string.Empty;

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
    }
}
