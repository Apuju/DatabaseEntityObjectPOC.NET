using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APIUtility.NET.Shared
{
    public enum CommandItemTrackingStatus
    {
        //Success
        Success_Success = 110,
        Success_Not_Supported = 120,
        Success_Skip = 130,
        //In Progress
        In_Progress_Submit = 410,
        In_Progress_Tracking = 420,
        In_Progress_Acceped = 430,
        //Failure
        Failure_Time_Out = 710,
        Failure_Cancelled = 720,
        Failure_Not_Available = 730,
        Failure_Failure = 990
    }

    public class CommandTrackingEntity
    {
        private string m_CommandTrackingID = string.Empty;
        private string m_ReceiverID = string.Empty;
        private string m_UserAccessID = string.Empty;
        private int m_MessageID = 0;
        private int m_TotalItems = 0;
        private int m_TotalSuccess = 0;
        private int m_TotalFailure = 0;
        private string m_ErrorDescription = string.Empty;
        private string m_Parameters = string.Empty;
        private List<CommandItemTrackingEntity> m_Items = new List<CommandItemTrackingEntity>();

        [DisplayName("CT_ID")]
        public string CommandTrackingID { 
            get
            {
                return m_CommandTrackingID;
            }
            set
            { 
                m_CommandTrackingID = value; 
            }
        }
        [DisplayName("CT_ReceiverID")]
        public string ReceiverID
        {
            get
            {
                return m_ReceiverID;
            }
            set
            {
                m_ReceiverID = value;
            }
        }
        [DisplayName("CT_UserAccessID")]
        public string UserAccessID
        {
            get
            {
                return m_UserAccessID;
            }
            set
            {
                m_UserAccessID = value;
            }
        }
        [DisplayName("CT_MessageID"), Required(ErrorMessage = "The property is required")]
        public int MessageID
        {
            get
            {
                return m_MessageID;
            }
            set
            {
                m_MessageID = value;
            }
        }
        [DisplayName("CT_TotalCommandItems")]
        public int TotalItems
        {
            get
            {
                return m_TotalItems;
            }
            set
            {
                m_TotalItems = value;
            }
        }
        [DisplayName("CT_TotalSuccess")]
        public int TotalSuccess
        {
            get
            {
                return m_TotalSuccess;
            }
            set
            {
                m_TotalSuccess = value;
            }
        }
        [DisplayName("CT_TotalFailure")]
        public int TotalFailure
        {
            get
            {
                return m_TotalFailure;
            }
            set
            {
                m_TotalFailure = value;
            }
        }
        [DisplayName("CT_ErrorDescription")]
        public string ErrorDescription
        {
            get
            {
                return m_ErrorDescription;
            }
            set
            {
                m_ErrorDescription = value;
            }
        }
        [DisplayName("CT_Parameters")]
        public string Parameters
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
        public List<CommandItemTrackingEntity> Items
        {
            get
            {
                return m_Items;
            }
            set
            {
                m_Items = value;
            }
        }
    }

    public class CommandItemTrackingEntity
    {
        private string m_ItemID = string.Empty;
        private string m_ReceiverID = string.Empty;
        private int m_MessageID = 0;
        private CommandItemTrackingStatus m_Status = CommandItemTrackingStatus.Success_Success;
        private string m_ErrorDescription = string.Empty;
        private string m_Parameters = string.Empty;

        [DisplayName("CIT_ID")]
        public string ItemID
        {
            get
            {
                return m_ItemID;
            }
            set
            {
                m_ItemID = value;
            }
        }
        [DisplayName("CIT_ReceiverID")]
        public string ReceiverID
        {
            get
            {
                return m_ReceiverID;
            }
            set
            {
                m_ReceiverID = value;
            }
        }
        [DisplayName("CIT_MessageID")]
        public int MessageID
        {
            get
            {
                return m_MessageID;
            }
            set
            {
                m_MessageID = value;
            }
        }
        [DisplayName("CIT_Status")]
        public CommandItemTrackingStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }
        [DisplayName("CIT_ErrorDescription")]
        public string ErrorDescription
        {
            get
            {
                return m_ErrorDescription;
            }
            set
            {
                m_ErrorDescription = value;
            }
        }
        [DisplayName("CIT_Parameters")]
        public string Parameters
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
    }
}
