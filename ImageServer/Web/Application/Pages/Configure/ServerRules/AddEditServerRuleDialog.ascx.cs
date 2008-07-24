    #region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerRules
{
	public partial class AddEditServerRuleDialog : UserControl
	{
		#region private variables

		// The server partitions that the new device can be associated with
		// This list will be determined by the user level permission.
		private ServerPartition _partition;

		private bool _editMode;
		private ServerRule _rule;

		// If new rule types are added, must be updated!
		private static readonly ServerRuleApplyTimeEnum[] _autoRouteList =
			new ServerRuleApplyTimeEnum[] {ServerRuleApplyTimeEnum.SopProcessed};

		private static readonly ServerRuleApplyTimeEnum[] _studyDeleteList =
			new ServerRuleApplyTimeEnum[] {ServerRuleApplyTimeEnum.StudyProcessed};

		private static readonly ServerRuleApplyTimeEnum[] _tier1RetentionList =
			new ServerRuleApplyTimeEnum[] {ServerRuleApplyTimeEnum.StudyArchived, ServerRuleApplyTimeEnum.StudyRestored};

		private static readonly ServerRuleApplyTimeEnum[] _onlineRetentionList =
			new ServerRuleApplyTimeEnum[] {ServerRuleApplyTimeEnum.StudyArchived, ServerRuleApplyTimeEnum.StudyRestored};

		private static readonly ServerRuleApplyTimeEnum[] _studyCompressList =
			new ServerRuleApplyTimeEnum[]
				{
					ServerRuleApplyTimeEnum.StudyProcessed, ServerRuleApplyTimeEnum.StudyArchived,
					ServerRuleApplyTimeEnum.StudyRestored
				};
															
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
				_rule = value;
				// put into viewstate to retrieve later
				if (_rule != null)
					ViewState[ClientID + "_EdittedRule"] = _rule.GetKey();
			}
			get { return _rule; }
		}

		#endregion // public members

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

		#region Protected Methods
		private static string GetJavascriptForSampleRule(ServerRuleTypeEnum typeEnum, ServerRuleApplyTimeEnum[] applyTimeEnumList, object[] extensions )
		{
			string sampleList = string.Empty;

			foreach (ISampleRule extension in extensions)
			{
				if (extension.Type.Equals(typeEnum))
				{
							sampleList +=
								String.Format(
									@"        myEle = document.createElement('option') ;
                    myEle.value = '{0}';
                    myEle.text = '{1}' ;
                    sampleList.add(myEle) ;",
									extension.Name, extension.Description);
				}
			}

			string enumList = string.Empty;
			foreach (ServerRuleApplyTimeEnum applyTimeEnum in applyTimeEnumList)
			{
				enumList += String.Format( @"myEle = document.createElement('option') ;
                    myEle.value = '{0}';
                    myEle.text = '{1}';
                    applyTimeList.add(myEle) ;",applyTimeEnum, applyTimeEnum.Description);
			}
			
			return String.Format(@"if (val == '{0}')
                {{
					{1}
                    myEle = document.createElement('option') ;
                    myEle.value = '';
                    myEle.text = '' ;
                    sampleList.add(myEle) ;
                    {2}
                }}", typeEnum.Lookup, enumList, sampleList);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			SampleRuleExtensionPoint ep = new SampleRuleExtensionPoint();
			object[] extensions = ep.CreateExtensions();

			ServerPartitionTabContainer.ActiveTabIndex = 0;

			SampleRuleDropDownList.Attributes.Add("onchange", "webServiceScript(this, this.SelectedIndex);");
			RuleTypeDropDownList.Attributes.Add("onchange", "selectRuleType(this);");

			string javascript = @"<script type='text/javascript'>
            function ValidationServerRuleParams()
            {
                control = document.getElementById('" +
								RuleXmlTextBox.ClientID +
								@"');
                params = new Array();
                params.serverRule=escape(control.value);
                return params;
            }

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
				" +
								GetJavascriptForSampleRule(ServerRuleTypeEnum.AutoRoute,
														   _autoRouteList, extensions) +
								@"else "
								+ GetJavascriptForSampleRule(ServerRuleTypeEnum.StudyDelete,
															 _studyDeleteList,extensions) +
								@"else "
								+ GetJavascriptForSampleRule(ServerRuleTypeEnum.Tier1Retention,
															 _tier1RetentionList,extensions) +
								@"else "
								+ GetJavascriptForSampleRule(ServerRuleTypeEnum.OnlineRetention,
															 _onlineRetentionList,extensions) +
								@"else "
								+ GetJavascriptForSampleRule(ServerRuleTypeEnum.StudyCompress,
															 _studyCompressList,extensions) +
								@"}

            // This function calls the Web Service method.  
            function webServiceScript(oList)
            {
                var type = oList.value;
             
                ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerRules.ServerRuleSamples.GetXml(type,
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
  
            </script>";

			Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, javascript);

            //Add the CodeMirror Javascript for text highlighting of the Server Rules
           /* ScriptManager.RegisterClientScriptBlock(this, GetType(), "CodeMirrorSrc", "<script language=javascript src='/Scripts/CodeMirror/js/codemirror.js' />", false);

            ScriptManager.RegisterStartupScript(this, GetType(), "CodeMirrorScript", "<script type=\"text/javascript\">" +
                    "alert(\"Hi\"); var editor = CodeMirror.fromTextArea('" + RuleXmlTextBox.ClientID + "',{" +
                        "height: \"350px\"," +
                        "parserfile: \"parsexml.js\"," +
                        "stylesheet: \"" + Page.ResolveClientUrl("~/Scripts/CodeMirror/css/xmlcolors.css") + "\"," + 
                        "path: \"" + Page.ResolveClientUrl("~/Scripts/CodeMirror/js/") + "\")," +
                        "continuousScanning: 500" +
                        "}); alert(editor.toString());</script>", false);*/
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				if (ViewState[ClientID + "_EditMode"] != null)
					_editMode = (bool) ViewState[ClientID + "_EditMode"];

				if (ViewState[ClientID + "_ServerPartition"] != null)
					_partition = (ServerPartition) ViewState[ClientID + "_ServerPartition"];

				if (ViewState[ClientID + "_EdittedRule"] != null)
				{
					ServerEntityKey ruleKey = ViewState[ClientID + "_EdittedRule"] as ServerEntityKey;
					_rule = ServerRule.Load(ruleKey);
				}
			}
		}

		protected void OKButton_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				SaveData();
				if (OKClicked != null)
				{
					OKClicked(ServerRule);
				}

				Close();
			}
			else
			{
				Show();
			}
		}

		protected void CancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}
		#endregion Protected Methods


		#region Private Methods

		private void SaveData()
		{
			if (_rule == null)
			{
				_rule = new ServerRule();
			}


			if (RuleXmlTextBox.Text.Length > 0)
			{
				_rule.RuleXml = new XmlDocument();
				_rule.RuleXml.Load(new StringReader(RuleXmlTextBox.Text));
			}

			_rule.RuleName = RuleNameTextBox.Text;

			_rule.ServerRuleTypeEnum = ServerRuleTypeEnum.GetEnum(RuleTypeDropDownList.SelectedItem.Value);

			// If new rule types are added, must be updated!
			if (_rule.ServerRuleTypeEnum.Equals(ServerRuleTypeEnum.AutoRoute))
				_rule.ServerRuleApplyTimeEnum = _autoRouteList[RuleApplyTimeDropDownList.SelectedIndex];
			else if (_rule.ServerRuleTypeEnum.Equals(ServerRuleTypeEnum.OnlineRetention))
				_rule.ServerRuleApplyTimeEnum = _onlineRetentionList[RuleApplyTimeDropDownList.SelectedIndex];
			else if (_rule.ServerRuleTypeEnum.Equals(ServerRuleTypeEnum.StudyCompress))
				_rule.ServerRuleApplyTimeEnum = _studyCompressList[RuleApplyTimeDropDownList.SelectedIndex];
			else if (_rule.ServerRuleTypeEnum.Equals(ServerRuleTypeEnum.StudyDelete))
				_rule.ServerRuleApplyTimeEnum = _studyDeleteList[RuleApplyTimeDropDownList.SelectedIndex];
			else if (_rule.ServerRuleTypeEnum.Equals(ServerRuleTypeEnum.Tier1Retention))
				_rule.ServerRuleApplyTimeEnum = _tier1RetentionList[RuleApplyTimeDropDownList.SelectedIndex];

			_rule.Enabled = EnabledCheckBox.Checked;
			_rule.DefaultRule = DefaultCheckBox.Checked;
			_rule.ServerPartitionKey = Partition.GetKey();
			_rule.ExemptRule = ExemptRuleCheckBox.Checked;
		}

		#endregion Private Methods

		#region Public methods

		/// <summary>
		/// Displays the add/edit device dialog box.
		/// </summary>
		public void Show()
		{
			// update the dropdown list
			RuleApplyTimeDropDownList.Items.Clear();
			RuleTypeDropDownList.Items.Clear();
			RuleXmlTabPanel.TabIndex = 0;
			ServerPartitionTabContainer.ActiveTabIndex = 0;

			if (EditMode)
			{
				ModalDialog.Title = App_GlobalResources.SR.DialogEditServerRuleTitle;
				OKButton.EnabledImageURL = App_GlobalResources.ImageFileLocation.UpdateButtonEnabled;

				DefaultCheckBox.Checked = _rule.DefaultRule;
				EnabledCheckBox.Checked = _rule.Enabled;
				ExemptRuleCheckBox.Checked = _rule.ExemptRule;

				//if (_rule.DefaultRule)
				//	DefaultCheckBox.Enabled = false;

				RuleNameTextBox.Text = _rule.RuleName;

				SampleRuleDropDownList.Visible = false;
				SelectSampleRuleLabel.Visible = false;

				// Fill in the drop down menus
				RuleTypeDropDownList.Items.Add(new ListItem(
				                               	_rule.ServerRuleTypeEnum.Description,
				                               	_rule.ServerRuleTypeEnum.Lookup));

				ServerRuleApplyTimeEnum[] list = new ServerRuleApplyTimeEnum[0];

				// NOTE:  Needs to be adjusted if new rules types are added!
				if (_rule.ServerRuleTypeEnum == ServerRuleTypeEnum.StudyDelete)
					list = _studyDeleteList;
				else if (_rule.ServerRuleTypeEnum == ServerRuleTypeEnum.Tier1Retention)
					list = _tier1RetentionList;
				else if (_rule.ServerRuleTypeEnum == ServerRuleTypeEnum.OnlineRetention)
					list = _onlineRetentionList;
				else if (_rule.ServerRuleTypeEnum == ServerRuleTypeEnum.AutoRoute)
					list = _autoRouteList;
				else if (_rule.ServerRuleTypeEnum == ServerRuleTypeEnum.StudyCompress)
					list = _studyCompressList;

				foreach (ServerRuleApplyTimeEnum applyTime in list)
					RuleApplyTimeDropDownList.Items.Add(new ListItem(
															applyTime.Description,
															applyTime.Lookup));


				if (RuleApplyTimeDropDownList.Items.FindByValue(_rule.ServerRuleApplyTimeEnum.Lookup) != null)
					RuleApplyTimeDropDownList.SelectedValue = _rule.ServerRuleApplyTimeEnum.Lookup;

				RuleTypeDropDownList.SelectedValue = _rule.ServerRuleTypeEnum.Lookup;


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

				RuleXmlTextBox.Text = sw.ToString();
			}
			else
			{
				ModalDialog.Title = App_GlobalResources.SR.DialogAddServerRuleTitle;
				OKButton.EnabledImageURL = App_GlobalResources.ImageFileLocation.AddButtonEnabled;

				DefaultCheckBox.Checked = false;
				EnabledCheckBox.Checked = true;
				ExemptRuleCheckBox.Checked = false;

				RuleNameTextBox.Text = string.Empty;
				RuleXmlTextBox.Text = string.Empty;

				SampleRuleDropDownList.Visible = true;
				SelectSampleRuleLabel.Visible = true;

				// Do the drop down lists
				RuleTypeDropDownList.Items.Add(new ListItem(
				                               	ServerRuleTypeEnum.AutoRoute.Description,
				                               	ServerRuleTypeEnum.AutoRoute.Lookup));
				RuleTypeDropDownList.Items.Add(new ListItem(
				                               	ServerRuleTypeEnum.StudyDelete.Description,
				                               	ServerRuleTypeEnum.StudyDelete.Lookup));
				RuleTypeDropDownList.Items.Add(new ListItem(
				                               	ServerRuleTypeEnum.StudyCompress.Description,
				                               	ServerRuleTypeEnum.StudyCompress.Lookup));
				RuleTypeDropDownList.Items.Add(new ListItem(
				                               	ServerRuleTypeEnum.OnlineRetention.Description,
				                               	ServerRuleTypeEnum.OnlineRetention.Lookup));

				RuleApplyTimeDropDownList.Items.Add(new ListItem(
				                                    	ServerRuleApplyTimeEnum.SopProcessed.Description,
				                                    	ServerRuleApplyTimeEnum.SopProcessed.Lookup));

				SampleRuleDropDownList.Items.Clear();
				SampleRuleDropDownList.Items.Add(new ListItem(string.Empty, string.Empty));
				SampleRuleDropDownList.Items.Add(new ListItem("Multi-Tag AutoRoute", "MultiTagAutoRoute"));
				SampleRuleDropDownList.Items.Add(new ListItem("Simple AutoRoute", "SimpleAutoRoute"));
			}

			ModalDialog.Show();
			return;
		}

		public void Close()
		{
			ModalDialog.Hide();
		}

		#endregion
	}
}