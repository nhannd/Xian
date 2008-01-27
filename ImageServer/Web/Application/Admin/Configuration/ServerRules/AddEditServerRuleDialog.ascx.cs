using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules
{
    public partial class AddEditServerRuleDialog : UserControl
    {
        #region private variables

        // The server partitions that the new device can be associated with
        // This list will be determined by the user level permission.
        private ServerPartition _partition;

        private bool _editMode;
        private ServerRule _rule;

        #endregion

        #region public members

        /// <summary>
        /// Sets the list of partitions users allowed to pick.
        /// </summary>
        public ServerPartition Partition
        {
            set
            {
                _partition = value;
                ViewState[ClientID + "_ServerPartition"] = value;
            }

            get { return _partition; }
        }

        /// <summary>
        /// Sets or gets the value which indicates whether the dialog is in edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;
                ViewState[ClientID + "_EditMode"] = value;
            }
        }

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public ServerRule ServerRule
        {
            set
            {
                this._rule = value;
                // put into viewstate to retrieve later
                if (_rule != null)
                    ViewState[ClientID + "_EdittedRule"] = _rule.GetKey();
            }
            get { return _rule; }
        }

        #endregion // public members

        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);

            this.ServerPartitionTabContainer.ActiveTabIndex = 0;

            // Set up the popup extender
            // These settings could been done in the aspx page as well
            // but if we are to javascript to display, that won't work.
            ModalPopupExtender1.PopupControlID = DialogPanel.UniqueID;
            ModalPopupExtender1.TargetControlID = DummyPanel.UniqueID;
            ModalPopupExtender1.BehaviorID = ModalPopupExtender1.UniqueID;

            ModalPopupExtender1.PopupDragHandleControlID = TitleBarPanel.UniqueID;

            this.SampleRuleDropDownList.Attributes.Add("onchange", "webServiceScript(this, this.SelectedIndex);");
            this.RuleTypeDropDownList.Attributes.Add("onchange", "selectRuleType(this);");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID,
                                                        @"<script type='text/javascript'>
  
            function selectRuleType(oList, selectedIndex)
            {         
                var val = oList.value; 
                var sampleList = document.getElementById('" +
                                                        SampleRuleDropDownList.ClientID +
                                                        @"');
                var applyTimeList = document.getElementById('" +
                                                        RuleApplyTimeDropDownList.ClientID +
                                                        @"');
                for (var q=sampleList.options.length; q>=0; q--) sampleList.options[q]=null;
                for (var q=applyTimeList.options.length; q>=0; q--) applyTimeList.options[q]=null;
                if (val == '" +
                                                        ServerRuleTypeEnum.GetEnum("AutoRoute").Enum +
                                                        @"')
                {
                    myEle = document.createElement('option') ;
                    myEle.value = '';
                    myEle.text = '' ;
                    sampleList.add(myEle) ;
                    myEle = document.createElement('option') ;
                    myEle.value = 'MultiTagAutoRoute';
                    myEle.text = 'Multi-Tag AutoRoute' ;
                    sampleList.add(myEle) ;
                    myEle = document.createElement('option') ;
                    myEle.value = 'SimpleAutoRoute';
                    myEle.text = 'Simple AutoRoute' ;
                    sampleList.add(myEle) ;

                    myEle = document.createElement('option') ;
                    myEle.value = '" +
                                                        ServerRuleApplyTimeEnum.GetEnum("SopProcessed").Enum +
                                                        @"';
                    myEle.text = '" +
                                                        ServerRuleApplyTimeEnum.GetEnum("SopProcessed").Description +
                                                        @"';
                    applyTimeList.add(myEle) ;

                }
                else if (val == '" +
                                                        ServerRuleTypeEnum.GetEnum("StudyDelete").Enum +
                                                        @"')
                {
                    myEle = document.createElement('option') ;
                    myEle.value = '';
                    myEle.text = '' ;
                    sampleList.add(myEle) ;
                    myEle = document.createElement('option') ;
                    myEle.value = 'AgeBasedDelete';
                    myEle.text = 'Age Based Delete' ;
                    sampleList.add(myEle) ;
                    myEle = document.createElement('option') ;
                    myEle.value = 'TagBasedDelete';
                    myEle.text = 'Tag Based Delete' ;
                    sampleList.add(myEle) ;

                    myEle = document.createElement('option') ;
                    myEle.value = '" +
                                                        ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").Enum +
                                                        @"';
                    myEle.text = '" +
                                                        ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").Description +
                                                        @"';
                    applyTimeList.add(myEle) ;
                }
                else if (val == '" +
                                                        ServerRuleTypeEnum.GetEnum("Tier1Retention").Enum +
                                                        @"')
                {
                    myEle = document.createElement('option') ;
                    myEle.value = '';
                    myEle.text = '' ;
                    sampleList.add(myEle) ;
                    myEle = document.createElement('option') ;
                    myEle.value = 'AgeBasedRetention';
                    myEle.text = 'Age Based Retention' ;
                    sampleList.add(myEle) ;

                    myEle = document.createElement('option') ;
                    myEle.value = '" +
                                                        ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").Enum +
                                                        @"';
                    myEle.text = '" +
                                                        ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").Description +
                                                        @"';
                    applyTimeList.add(myEle) ;
                }
            }

            // This function calls the Web Service method.  
            function webServiceScript(oList)
            {
                var type = oList.value;
             
                ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRuleSamples.GetXml(type,
                    OnSucess, OnError);
            }
            function OnError(result)
            {
                alert('Error: ' + result.get_message());
            }

            // This is the callback function that
            // processes the Web Service return value.
            function OnSucess(result)
            {
                var oList = document.getElementById('" +
                                                        SampleRuleDropDownList.ClientID +
                                                        @"');
                var sValue = oList.options[oList.selectedIndex].value;
             
                RsltElem = document.getElementById('" +
                                                        RuleXmlTextBox.ClientID +
                                                        @"');
                RsltElem.value = result;
            }
  
            </script>");
        }

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OKClicked"/>.
        /// </summary>
        /// <param name="rule">The device being added.</param>
        public delegate void OnOKClickedEventHandler(ServerRule rule);

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler OKClicked;

        #endregion Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (ViewState[ClientID + "_EditMode"] != null)
                    _editMode = (bool) ViewState[ClientID + "_EditMode"];

                if (ViewState[ClientID + "_ServerPartition"] != null)
                    _partition = (ServerPartition)ViewState[ClientID + "_ServerPartition"];

                if (ViewState[ClientID + "_EdittedRule"] != null)
                {
                    ServerEntityKey ruleKey = ViewState[ClientID + "_EdittedRule"] as ServerEntityKey;
                    _rule = ServerRule.Load(ruleKey);
                }
            }
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            ServerRule theRule;
            if (_rule == null)
                theRule = new ServerRule();
            else
                theRule = _rule;

            if (RuleXmlTextBox.Text.Length > 0)
            {
                theRule.RuleXml = new XmlDocument();
                theRule.RuleXml.Load(new StringReader(RuleXmlTextBox.Text));
            }

            theRule.RuleName = this.RuleNameTextBox.Text;

            theRule.ServerRuleTypeEnum = new ServerRuleTypeEnum();
            theRule.ServerRuleTypeEnum.SetEnum(short.Parse(this.RuleTypeDropDownList.SelectedItem.Value));

            if (theRule.ServerRuleTypeEnum == ServerRuleTypeEnum.GetEnum("AutoRoute"))
                theRule.ServerRuleApplyTimeEnum = ServerRuleApplyTimeEnum.GetEnum("SopProcessed");
            else
                theRule.ServerRuleApplyTimeEnum = ServerRuleApplyTimeEnum.GetEnum("StudyProcessed");

            theRule.Enabled = this.EnabledCheckBox.Checked;
            theRule.DefaultRule = this.DefaultCheckBox.Checked;
            theRule.ServerPartitionKey = this.Partition.GetKey();

            if (Page.IsValid)
            {
                if (OKClicked != null)
                {
                    OKClicked(theRule);
                }

                Close();
            }
            else
            {
                Show();
            }
            
        }

        #region Public methods

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            // update the dropdown list
            this.RuleApplyTimeDropDownList.Items.Clear();
            this.RuleTypeDropDownList.Items.Clear();
            this.RuleXmlTabPanel.TabIndex = 0;
            this.ServerPartitionTabContainer.ActiveTabIndex = 0;

            if (EditMode)
            {
                TitleLabel.Text = "Edit Server Rule";
                OKButton.Text = "Update";

                this.DefaultCheckBox.Checked = _rule.DefaultRule;
                this.EnabledCheckBox.Checked = _rule.Enabled;

                this.RuleNameTextBox.Text = _rule.RuleName;

                this.SampleRuleDropDownList.Visible = false;
                this.SelectSampleRuleLabel.Visible = false;

                // Fill in the drop down menus
                RuleTypeDropDownList.Items.Add(new ListItem(
                                   _rule.ServerRuleTypeEnum.Description,
                                   _rule.ServerRuleTypeEnum.Enum.ToString()));

                if (_rule.ServerRuleTypeEnum.Enum == ServerRuleTypeEnum.GetEnum("StudyDelete").Enum)
                {
                    RuleApplyTimeDropDownList.Items.Add(new ListItem(
                                                            ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").
                                                                Description,
                                                            ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").Enum.
                                                                ToString()));
                }
                else if (_rule.ServerRuleTypeEnum.Enum == ServerRuleTypeEnum.GetEnum("Tier1Retention").Enum)
                {
                    RuleApplyTimeDropDownList.Items.Add(new ListItem(
                                                            ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").
                                                                Description,
                                                            ServerRuleApplyTimeEnum.GetEnum("StudyProcessed").Enum.
                                                                ToString()));
                }
                else if (_rule.ServerRuleTypeEnum.Enum == ServerRuleTypeEnum.GetEnum("AutoRoute").Enum)
                {
                    RuleApplyTimeDropDownList.Items.Add(new ListItem(
                                                            ServerRuleApplyTimeEnum.GetEnum("SopProcessed").Description,
                                                            ServerRuleApplyTimeEnum.GetEnum("SopProcessed").Enum.
                                                                ToString()));
                }
                RuleApplyTimeDropDownList.SelectedValue = _rule.ServerRuleApplyTimeEnum.Enum.ToString();
                RuleTypeDropDownList.SelectedValue = _rule.ServerRuleTypeEnum.Enum.ToString();


                // Fill in the Rule XML
                StringWriter sw = new StringWriter();

                XmlWriterSettings xmlSettings = new XmlWriterSettings();

                xmlSettings.Encoding = Encoding.UTF8;
                xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
                xmlSettings.Indent = true;
                xmlSettings.NewLineOnAttributes = false;
                xmlSettings.CheckCharacters = true;
                xmlSettings.IndentChars = "  ";

                XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

                _rule.RuleXml.WriteTo(tw);

                tw.Close();

                this.RuleXmlTextBox.Text = sw.ToString();
            }
            else
            {
                TitleLabel.Text = "Add Server Rule";
                OKButton.Text = "Add";

                this.DefaultCheckBox.Checked = false;
                this.EnabledCheckBox.Checked = true;

                this.RuleNameTextBox.Text = "";
                this.RuleXmlTextBox.Text = "";

                this.SampleRuleDropDownList.Visible = true;
                this.SelectSampleRuleLabel.Visible = true;

                // Do the drop down lists
                RuleTypeDropDownList.Items.Add(new ListItem(
                                                ServerRuleTypeEnum.GetEnum("AutoRoute").Description,
                                                ServerRuleTypeEnum.GetEnum("AutoRoute").Enum.ToString()));

                RuleTypeDropDownList.Items.Add(new ListItem(
                                                   ServerRuleTypeEnum.GetEnum("StudyDelete").Description,
                                                   ServerRuleTypeEnum.GetEnum("StudyDelete").Enum.ToString()));

                RuleApplyTimeDropDownList.Items.Add(new ListItem(
                                                        ServerRuleApplyTimeEnum.GetEnum("SopProcessed").Description,
                                                        ServerRuleApplyTimeEnum.GetEnum("SopProcessed").Enum.ToString()));

                SampleRuleDropDownList.Items.Clear();
                SampleRuleDropDownList.Items.Add(new ListItem("", ""));
                SampleRuleDropDownList.Items.Add(new ListItem("Multi-Tag AutoRoute", "MultiTagAutoRoute"));
                SampleRuleDropDownList.Items.Add(new ListItem("Simple AutoRoute", "SimpleAutoRoute"));
            }

            ModalPopupExtender1.Show();
            this.AddEditUpdatePanel.Update();
            return;
        }

        public void Close()
        {
            ModalPopupExtender1.Hide();
        }

        #endregion
    }
}