using Microsoft.IdentityManagement.WebUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.Web.UI.WebControls;

namespace EXCEED_CustomWFActivities
{
    class TriggerO365SyncSettingsPart : ActivitySettingsPart
    {
        public override string Title
        {
            get
            {
                return "Trigger O365 Sync";
            }
        }

        public override Activity GenerateActivityOnWorkflow(SequentialWorkflow workflow)
        {
            if (!this.ValidateInputs())
            {
                return null;
            }
            TriggerO365Sync triggerO365Sync = new TriggerO365Sync();
            triggerO365Sync.O365AdminUsername = this.GetText("O365AdminUsernameTextBox");
            triggerO365Sync.O365AdminPassword = this.GetText("O365AdminPasswordTextBox");
            triggerO365Sync.Domain = this.GetText("DomainTextBox");
            triggerO365Sync.SyncMachineName = this.GetText("SyncMachineNameTextBox");
            triggerO365Sync.TaskName = this.GetText("TaskNameTextBox");
            return triggerO365Sync;
        }

        public override void LoadActivitySettings(Activity activity)
        {
            TriggerO365Sync triggerO365Sync = activity as TriggerO365Sync;
            if (null != triggerO365Sync)
            {
                this.SetText("O365AdminUsernameTextBox", triggerO365Sync.O365AdminUsername);
                this.SetText("O365AdminPasswordTextBox", triggerO365Sync.O365AdminPassword);
                this.SetText("DomainTextBox", triggerO365Sync.Domain);
                this.SetText("SyncMachineNameTextBox", triggerO365Sync.SyncMachineName);
                this.SetText("TaskNameTextBox", triggerO365Sync.TaskName);
            }
        }

        public override ActivitySettingsPartData PersistSettings()
        {
            ActivitySettingsPartData data = new ActivitySettingsPartData();
            data["O365AdminUsername"] = this.GetText("O365AdminUsernameTextBox");
            data["O365AdminPassword"] = this.GetText("O365AdminPasswordTextBox");
            data["Domain"] = this.GetText("DomainTextBox");
            data["SyncMachineName"] = this.GetText("SyncMachineNameTextBox");
            data["TaskName"] = this.GetText("TaskNameTextBox");
            return data;
        }

        public override void RestoreSettings(ActivitySettingsPartData data)
        {
            if (null != data)
            {
                this.SetText("O365AdminUsernameTextBox", (string)data["O365AdminUsername"]);
                this.SetText("O365AdminPasswordTextBox", (string)data["O365AdminPassword"]);
                this.SetText("DomainTextBox", (string)data["Domain"]);
                this.SetText("SyncMachineNameTextBox", (string)data["SyncMachineName"]);
                this.SetText("TaskNameTextBox", (string)data["TaskName"]);
            }
        }

        public override void SwitchMode(ActivitySettingsPartMode mode)
        {
            bool readOnly = (mode == ActivitySettingsPartMode.View);
            this.SetTextBoxReadOnlyOption("O365AdminUsernameTextBox", readOnly);
            this.SetTextBoxReadOnlyOption("O365AdminPasswordTextBox", readOnly);
            this.SetTextBoxReadOnlyOption("DomainTextBox", readOnly);
            this.SetTextBoxReadOnlyOption("SyncMachineNameTextBox", readOnly);
            this.SetTextBoxReadOnlyOption("TaskNameTextBox", readOnly);
        }

        public override bool ValidateInputs()
        {
            return true;
        }


        protected override void CreateChildControls()
        {
            Table controlLayoutTable;
            controlLayoutTable = new Table();

            //Width is set to 100% of the control size
            controlLayoutTable.Width = Unit.Percentage(100.0);
            controlLayoutTable.BorderWidth = 0;
            controlLayoutTable.CellPadding = 2;
            //Add a TableRow for each textbox in the UI 
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("O365 Admin Username:", "O365AdminUsernameTextBox", 400, 100, false, "", false));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("O365 Admin Password:", "O365AdminPasswordTextBox", 400, 100, false, "", true));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("Domain:", "DomainTextBox", 400, 100, false, "", false));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("Sync Machine Name (FQDN):", "SyncMachineNameTextBox", 400, 100, false, "", false));
            controlLayoutTable.Rows.Add(this.AddTableRowTextBox("Task Name:", "TaskNameTextBox", 400, 100, false, "", false));

            this.Controls.Add(controlLayoutTable);

            base.CreateChildControls();
        }

        #region Utility Functions

        //Create a TableRow that contains a label and a textbox.
        private TableRow AddTableRowTextBox(String labelText, String controlID, int width, int
                                             maxLength, Boolean multiLine, String defaultValue, bool secure)
        {
            TableRow row = new TableRow();
            TableCell labelCell = new TableCell();
            TableCell controlCell = new TableCell();
            Label oLabel = new Label();
            TextBox oText = new TextBox();

            oLabel.Text = labelText;
            oLabel.CssClass = base.LabelCssClass;
            labelCell.Controls.Add(oLabel);
            oText.ID = controlID;
            oText.CssClass = base.TextBoxCssClass;
            oText.Text = defaultValue;
            oText.MaxLength = maxLength;
            oText.Width = width;

            if (secure)
            {
                oText.TextMode = TextBoxMode.Password;
            }

            if (multiLine)
            {
                oText.TextMode = TextBoxMode.MultiLine;
                oText.Rows = System.Math.Min(6, (maxLength + 60) / 60);
                oText.Wrap = true;
            }
            controlCell.Controls.Add(oText);
            row.Cells.Add(labelCell);
            row.Cells.Add(controlCell);
            return row;
        }

        string GetText(string textBoxID)
        {
            TextBox textBox = (TextBox)this.FindControl(textBoxID);
            return textBox.Text ?? String.Empty;
        }
        void SetText(string textBoxID, string text)
        {
            TextBox textBox = (TextBox)this.FindControl(textBoxID);
            if (textBox != null)
                textBox.Text = text;
            else
                textBox.Text = "";
        }

        //Set the text box to read mode or read/write mode
        void SetTextBoxReadOnlyOption(string textBoxID, bool readOnly)
        {
            TextBox textBox = (TextBox)this.FindControl(textBoxID);
            textBox.ReadOnly = readOnly;
        }
        #endregion
    }
}
