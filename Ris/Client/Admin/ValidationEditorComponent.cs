#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Xml;
using System.Reflection;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;
using System.IO;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop.Validation;
using System.Collections;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ValidationEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ValidationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ValidationEditorComponent class
    /// </summary>
    [AssociateView(typeof(ValidationEditorComponentViewExtensionPoint))]
    public class ValidationEditorComponent : ApplicationComponent
    {
        #region Rule class

        class Rule
        {
            private string _name;
            private string _boundProperty;
            private string _ruleXml;
            private IValidationRule _compiledRule;
            private string _status;
            private bool _parseError;
            private readonly ApplicationComponent _liveComponent;


            public Rule(string ruleXml, ApplicationComponent liveComponent)
            {
                _liveComponent = liveComponent;
                _ruleXml = ruleXml;

                Update();
            }

            public string Name
            {
                get { return _name; }
            }

            public string BoundProperty
            {
                get { return _boundProperty; }
            }

            public string RuleXml
            {
                get { return _ruleXml; }
                set { _ruleXml = value; }
            }

            public string Status
            {
                get { return _status; }
            }

            public bool ParseError
            {
                get { return _parseError; }
            }

            public void Update()
            {
                try
                {
                    Compile();
                    _parseError = false;
                    if (_liveComponent != null)
                    {
                        ValidationResult result = _compiledRule.GetResult(_liveComponent);
                        if (result.Success)
                            _status = "Pass";
                        else
                            _status = string.Format("Fail: {0}", result.GetMessageString(", "));
                    }
                    else
                    {
                        _status = null;
                    }

                }
                catch (Exception e)
                {
                    _status = e.Message;
                    _parseError = true;
                }
            }

            private void Compile()
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(_ruleXml);

                _name = xmlDoc.DocumentElement.GetAttribute("id");
                _boundProperty = xmlDoc.DocumentElement.GetAttribute("boundProperty");

                XmlValidationCompiler compiler = new XmlValidationCompiler();
                _compiledRule = compiler.CompileRule(xmlDoc.DocumentElement);
            }
        }

        #endregion

        private const string tagValidationRule = "validation-rule";
        private const string tagValidationRules = "validation-rules";

        private readonly Type _applicationComponentClass;

        private readonly Table<Rule> _rules;
        private Rule _selectedRule;

		private XmlValidationManager _xmlValidationManager;

        private readonly ApplicationComponent _liveComponent;

        private CrudActionModel _rulesActionModel;
        private int _newRuleCounter = 0;
        private List<PropertyInfo> _componentProperties;

        private ChildComponentHost _editorHost;
        private ICodeEditor _editor;


        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationEditorComponent(Type applicationComponentClass)
        {
            _applicationComponentClass = applicationComponentClass;

            _rules = new Table<Rule>();
            _rules.Columns.Add(new TableColumn<Rule, string>("Name",
                delegate(Rule p) { return p.Name; }));
            _rules.Columns.Add(new TableColumn<Rule, string>("Bound Property",
                delegate(Rule p) { return p.BoundProperty; }));
            _rules.Columns.Add(new TableColumn<Rule, string>("Test Result",
                delegate(Rule p) { return p.Status; }));
        }

        public ValidationEditorComponent(ApplicationComponent component)
            :this(component.GetType())
        {
            _liveComponent = component;
        }

        public override void Start()
        {
			_xmlValidationManager = XmlValidationManager.Instance;

			// try to load existing rules
			// if this fails, an exception will be thrown, preventing this component from starting
			var rules = CollectionUtils.Map(_xmlValidationManager.GetRules(_applicationComponentClass),
				(XmlElement node) => new Rule(node.OuterXml, _liveComponent));

			_rules.Items.AddRange(rules);

            _componentProperties = CollectionUtils.Select(_applicationComponentClass.GetProperties(),
                              delegate(PropertyInfo p)
                              {
                                  return !p.DeclaringType.Equals(typeof(IApplicationComponent))
                                         && !p.DeclaringType.Equals(typeof(ApplicationComponent));
                              });


            _rulesActionModel = new CrudActionModel(true, false, true);
            _rulesActionModel.Add.SetClickHandler(AddNewRule);
            _rulesActionModel.Delete.SetClickHandler(DeleteSelectedRule);
            _rulesActionModel.Delete.Enabled = false;

            _editor = CodeEditorFactory.CreateCodeEditor();
            _editor.Language = "xml";

            _editorHost = new ChildComponentHost(this.Host, _editor.GetComponent());
            _editorHost.StartComponent();

            base.Start();
        }


        public override void Stop()
        {
            if (_editorHost != null)
            {
                _editorHost.StopComponent();
                _editorHost = null;
            }

            base.Stop();
        }

        #region Presentation Model

        public ApplicationComponentHost EditorComponentHost
        {
            get { return _editorHost; }
        }

        public IList<PropertyInfo> ComponentPropertyChoices
        {
            get
            {
                return _componentProperties;
            }
        }

        public ITable Rules
        {
            get { return _rules; }
        }

        public ActionModelNode RulesActionModel
        {
            get { return _rulesActionModel; }
        }

        public ISelection SelectedRule
        {
            get { return new Selection(_selectedRule); }
            set
            {
                Rule selected = (Rule)value.Item;
                if (!Equals(_selectedRule, selected))
                {
                    ChangeSelectedRule(selected);
                }
            }
        }

        public void InsertText(string text)
        {
            _editor.InsertText(text);
        }

        public void AddNewRule()
        {
            // get unique rule name
            string ruleName = "rule" + (++_newRuleCounter);
            while (CollectionUtils.Contains(_rules.Items, delegate(Rule r) { return r.Name == ruleName; }))
                ruleName = "rule" + (++_newRuleCounter);

            string blankRuleXml = string.Format("<{0} id=\"{1}\" boundProperty=\"\">\n</{2}>", tagValidationRule, ruleName, tagValidationRule);

            _rules.Items.Add(new Rule(blankRuleXml, _liveComponent));
        }

        public void DeleteSelectedRule()
        {
            if(_selectedRule != null)
            {
                Rule ruleToDelete = _selectedRule;

                // de-select before deleting - otherwise it causes an exception
                ChangeSelectedRule(null);

                _rules.Items.Remove(ruleToDelete);
            }
        }

        public void Accept()
        {
            // commit final changes (hack - just re-select the already selected property)
            ChangeSelectedRule(_selectedRule);

            try
            {
				XmlElement rulesNode;
                if (!CreateRulesXml(out rulesNode))
                    return;

				_xmlValidationManager.SetRules(_applicationComponentClass, rulesNode);
				_xmlValidationManager.Save();

				// invalidate cache for this component to cause rules to re-compile on next load
                ValidationCache.Instance.Invalidate(_applicationComponentClass);

                Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionSaveValidationRules, this.Host.DesktopWindow,
                    delegate
                    {
                        Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        public bool CanTestRules
        {
            get { return _liveComponent != null; }
        }

        public void TestRules()
        {
            // commit final changes (hack - just re-select the already selected property)
            ChangeSelectedRule(_selectedRule);

            foreach (Rule rule in _rules.Items)
            {
                _rules.Items.NotifyItemUpdated(rule);
            }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void ChangeSelectedRule(Rule selected)
        {
            if (_selectedRule != null)
            {
                _selectedRule.RuleXml = _editor.Text;
                _selectedRule.Update();

                // notify updated
                _rules.Items.NotifyItemUpdated(_selectedRule);
            }

            _selectedRule = selected;
            _editor.Text = null;
            
            if (_selectedRule != null)
            {
                _editor.Text = _selectedRule.RuleXml;
                this.NotifyPropertyChanged("ValidationXml");
            }

            _rulesActionModel.Delete.Enabled = (_selectedRule != null);
        }

        private bool CreateRulesXml(out XmlElement rulesNode)
        {
			var xmlDoc = new XmlDocument();
			xmlDoc.PreserveWhitespace = true;
			rulesNode = xmlDoc.CreateElement(tagValidationRules);

			foreach (Rule rule in _rules.Items)
            {
                if(!CollectionUtils.Contains(_componentProperties, delegate (PropertyInfo p) { return p.Name == rule.BoundProperty; }))
                {
                    this.Host.ShowMessageBox(string.Format("Rule {0} is not bound to a valid property.", rule.Name), MessageBoxActions.Ok);
                    return false;
                }

                if (rule.ParseError)
                {
                    this.Host.ShowMessageBox("One or more rules have syntax errors which must be corrected first.", MessageBoxActions.Ok);
                    return false;
                }

				XmlDocumentFragment fragment = xmlDoc.CreateDocumentFragment();
                try
                {
                    fragment.InnerXml = rule.RuleXml;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, string.Format("Error parsing rule {0}", rule.Name), this.Host.DesktopWindow);
                    _selectedRule = rule;
                    NotifyPropertyChanged("SelectedRule");

                    // abort
                    return false;
                }

                // update the xml document
                rulesNode.AppendChild(fragment);
            }

            return true;
        }
    }
}
