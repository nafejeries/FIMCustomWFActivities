using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace EXCEED_CustomWFActivities
{
    public partial class TriggerO365Sync
    {
        #region Activity Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        [System.CodeDom.Compiler.GeneratedCode("", "")]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            this.TriggerSyncCodeActivity = new System.Workflow.Activities.CodeActivity();
            this.ReadTargetResourceActivity = new Microsoft.ResourceManagement.Workflow.Activities.ReadResourceActivity();
            this.InitializeReadTargetResourceCodeActivity = new System.Workflow.Activities.CodeActivity();
            this.ReadCurrentRequestActivity = new Microsoft.ResourceManagement.Workflow.Activities.CurrentRequestActivity();
            // 
            // TriggerSyncCodeActivity
            // 
            this.TriggerSyncCodeActivity.Name = "TriggerSyncCodeActivity";
            this.TriggerSyncCodeActivity.ExecuteCode += new System.EventHandler(this.TriggerSyncCodeActivity_ExecuteCode);
            // 
            // ReadTargetResourceActivity
            // 
            this.ReadTargetResourceActivity.ActorId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.ReadTargetResourceActivity.Name = "ReadTargetResourceActivity";
            this.ReadTargetResourceActivity.Resource = null;
            this.ReadTargetResourceActivity.ResourceId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.ReadTargetResourceActivity.SelectionAttributes = null;
            // 
            // InitializeReadTargetResourceCodeActivity
            // 
            this.InitializeReadTargetResourceCodeActivity.Name = "InitializeReadTargetResourceCodeActivity";
            this.InitializeReadTargetResourceCodeActivity.ExecuteCode += new System.EventHandler(this.InitializeReadTargetResourceCodeActivity_ExecuteCode);
            // 
            // ReadCurrentRequestActivity
            // 
            activitybind1.Name = "TriggerO365Sync";
            activitybind1.Path = "ReadCurrentRequestActivity_CurrentRequest";
            this.ReadCurrentRequestActivity.Name = "ReadCurrentRequestActivity";
            this.ReadCurrentRequestActivity.SetBinding(Microsoft.ResourceManagement.Workflow.Activities.CurrentRequestActivity.CurrentRequestProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // TriggerO365Sync
            // 
            this.Activities.Add(this.ReadCurrentRequestActivity);
            this.Activities.Add(this.InitializeReadTargetResourceCodeActivity);
            this.Activities.Add(this.ReadTargetResourceActivity);
            this.Activities.Add(this.TriggerSyncCodeActivity);
            this.Name = "TriggerO365Sync";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.ResourceManagement.Workflow.Activities.ReadResourceActivity ReadTargetResourceActivity;
        private CodeActivity InitializeReadTargetResourceCodeActivity;
        private CodeActivity TriggerSyncCodeActivity;
        private Microsoft.ResourceManagement.Workflow.Activities.CurrentRequestActivity ReadCurrentRequestActivity;
    }
}
