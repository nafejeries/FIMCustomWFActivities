using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using Microsoft.ResourceManagement.Workflow.Activities;
using System.Security;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Microsoft.Win32.TaskScheduler;

namespace EXCEED_CustomWFActivities
{
    public partial class TriggerO365Sync : SequenceActivity
    {
        string fimAdminGuid = "7fb2b853-24f0-4498-9534-4e10589723c4";

        public TriggerO365Sync()
        {
            InitializeComponent();
        }

        #region Dependencies
        public static DependencyProperty ReadCurrentRequestActivity_CurrentRequestProperty = DependencyProperty.Register("ReadCurrentRequestActivity_CurrentRequest", typeof(Microsoft.ResourceManagement.WebServices.WSResourceManagement.RequestType), typeof(TriggerO365Sync));

        /// <summary>
        ///  Stores information about the current request
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("")]
        public RequestType ReadCurrentRequestActivity_CurrentRequest
        {
            get
            {
                return ((Microsoft.ResourceManagement.WebServices.WSResourceManagement.RequestType)(base.GetValue(TriggerO365Sync.ReadCurrentRequestActivity_CurrentRequestProperty)));
            }
            set
            {
                base.SetValue(TriggerO365Sync.ReadCurrentRequestActivity_CurrentRequestProperty, value);
            }
        }


        public static DependencyProperty O365AdminUsernameProperty = DependencyProperty.Register("O365AdminUsername", typeof(string), typeof(TriggerO365Sync));

        [Description("O365AdminUsername")]
        [Category("")]
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public string O365AdminUsername
        {
            get
            {
                return ((string)(base.GetValue(TriggerO365Sync.O365AdminUsernameProperty)));
            }
            set
            {
                base.SetValue(TriggerO365Sync.O365AdminUsernameProperty, value);
            }
        }


        public static DependencyProperty O365AdminPasswordProperty = DependencyProperty.Register("O365AdminPassword", typeof(string), typeof(TriggerO365Sync));

        [Description("O365AdminPassword")]
        [Category("")]
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public string O365AdminPassword
        {
            get
            {
                return ((string)(base.GetValue(TriggerO365Sync.O365AdminPasswordProperty)));
            }
            set
            {
                base.SetValue(TriggerO365Sync.O365AdminPasswordProperty, value);
            }
        }

        public static DependencyProperty DomainProperty = DependencyProperty.Register("Domain", typeof(string), typeof(TriggerO365Sync));

        [Description("Domain")]
        [Category("")]
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public string Domain
        {
            get
            {
                return ((string)(base.GetValue(TriggerO365Sync.DomainProperty)));
            }
            set
            {
                base.SetValue(TriggerO365Sync.DomainProperty, value);
            }
        }

        public static DependencyProperty SyncMachineNameProperty = DependencyProperty.Register("SyncMachineName", typeof(string), typeof(TriggerO365Sync));

        [Description("SyncMachineName")]
        [Category("")]
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public string SyncMachineName
        {
            get
            {
                return ((string)(base.GetValue(TriggerO365Sync.SyncMachineNameProperty)));
            }
            set
            {
                base.SetValue(TriggerO365Sync.SyncMachineNameProperty, value);
            }
        }

        public static DependencyProperty TaskNameProperty = DependencyProperty.Register("TaskName", typeof(string), typeof(TriggerO365Sync));

        [Description("TaskName")]
        [Category("")]
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public string TaskName
        {
            get
            {
                return ((string)(base.GetValue(TriggerO365Sync.TaskNameProperty)));
            }
            set
            {
                base.SetValue(TriggerO365Sync.TaskNameProperty, value);
            }
        }

        #endregion

        private void InitializeReadTargetResourceCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            ReadTargetResourceActivity.ActorId = new Guid(fimAdminGuid);
            ReadTargetResourceActivity.ResourceId = ReadCurrentRequestActivity.CurrentRequest.Target.GetGuid();
            ReadTargetResourceActivity.SelectionAttributes = new string[] { "Email" };
        }

        private void TriggerSyncCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            string targetEmailAddress = ReadTargetResourceActivity.Resource["Email"] as string;

            if (UserHasExchangeOnline(targetEmailAddress))
                RunSyncTask();
        }

        private bool UserHasExchangeOnline(string emailAddress)
        {
            SecureString securePassword = new SecureString();
            foreach (char c in this.O365AdminPassword)
            {
                securePassword.AppendChar(c);
            }

            PSCredential psCredential = new PSCredential(this.O365AdminUsername, securePassword);

            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(
                new Uri("https://ps.outlook.com/powershell"),
                "http://schemas.microsoft.com/powershell/Microsoft.Exchange",
                psCredential);

            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;
            connectionInfo.MaximumConnectionRedirectionCount = 2;

            using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                runspace.Open();

                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = runspace;

                    powershell.AddCommand("Get-Mailbox");

                    powershell.AddParameter("RecipientTypeDetails", "UserMailbox");

                    powershell.AddParameter("Identity", emailAddress);

                    Collection<PSObject> results = powershell.Invoke();

                    if (results.Count > 0)
                        return true;

                    return false;
                }

            }
        }

        private void RunSyncTask()
        {
            using (TaskService tasksrvc = new TaskService(
                this.SyncMachineName, this.O365AdminUsername, this.Domain, this.O365AdminPassword, false))
            {
                Task task = tasksrvc.FindTask(this.TaskName, false);

                if (task == null)
                    throw new ItemNotFoundException("Unable to find the specified scheduled task");

                while (task.State == TaskState.Running)
                {
                    System.Threading.Thread.Sleep(15000);
                }

                task.Run();
            }
        }

    }
}
