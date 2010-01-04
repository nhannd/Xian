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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Xml;
using System.Reflection;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Validation;
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
        	private IValidationRule _compiledRule;
        	private readonly ApplicationComponent _liveComponent;


            public Rule(string ruleXml, ApplicationComponent liveComponent)
            {
                _liveComponent = liveComponent;
                RuleXml = ruleXml;

                Update();
            }

        	public string Name { get; private set; }

        	public string BoundProperty { get; private set; }

        	public string RuleXml { get; set; }

        	public string Status { get; private set; }

        	public bool ParseError { get; private set; }

        	public void Update()
            {
                try
                {
                    Compile();
                    ParseError = false;
                    if (_liveComponent != null)
                    {
                    	var result = _compiledRule.GetResult(_liveComponent);
                    	Status = result.Success ? "Pass" : string.Format("Fail: {0}", result.GetMessageString(", "));
                    }
                    else
                    {
                        Status = null;
                    }

                }
                catch (Exception e)
                {
                    Status = e.Message;
                    ParseError = true;
                }
            }

            private void Compile()
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(RuleXml);

            	var rootNode = xmlDoc.DocumentElement;
            	if (rootNode == null)
					return;

            	Name = rootNode.GetAttribute("id");
            	BoundProperty = rootNode.GetAttribute("boundProperty");

            	var compiler = new XmlValidationCompiler();
            	_compiledRule = compiler.CompileRule(xmlDoc.DocumentElement);
            }
        }

        #endregion

        private const string TagValidationRule = "validation-rule";
        private const string TagValidationRules = "validation-rules";

        private readonly Type _applicationComponentClass;

        private readonly Table<Rule> _rules;
        private Rule _selectedRule;

		private XmlValidationManager _xmlValidationManager;

        private readonly ApplicationComponent _liveComponent;

        private CrudActionModel _rulesActionModel;
        private int _newRuleCounter;
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
            _rules.Columns.Add(new TableColumn<Rule, string>("Name", p => p.Name));
            _rules.Columns.Add(new TableColumn<Rule, string>("Bound Property", p => p.BoundProperty));
            _rules.Columns.Add(new TableColumn<Rule, string>("Test Result", p => p.Status));
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
                                                          p => !p.DeclaringType.Equals(typeof (IApplicationComponent))
                                                               && !p.DeclaringType.Equals(typeof (ApplicationComponent)));

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
                var selected = (Rule)value.Item;
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
            var ruleName = "rule" + (++_newRuleCounter);
            while (CollectionUtils.Contains(_rules.Items, r => r.Name == ruleName))
                ruleName = "rule" + (++_newRuleCounter);

            var blankRuleXml = string.Format("<{0} id=\"{1}\" boundProperty=\"\">\n</{2}>", TagValidationRule, ruleName, TagValidationRule);

            _rules.Items.Add(new Rule(blankRuleXml, _liveComponent));
        }

        public void DeleteSelectedRule()
        {
        	if (_selectedRule == null)
				return;

        	var ruleToDelete = _selectedRule;

        	// de-select before deleting - otherwise it causes an exception
        	ChangeSelectedRule(null);

        	_rules.Items.Remove(ruleToDelete);
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
                                        () => Exit(ApplicationComponentExitCode.Error));
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

            foreach (var rule in _rules.Items)
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
			var xmlDoc = new XmlDocument {PreserveWhitespace = true};
        	rulesNode = xmlDoc.CreateElement(TagValidationRules);

			foreach (var rule in _rules.Items)
            {
                if(!CollectionUtils.Contains(_componentProperties, p => p.Name == rule.BoundProperty))
                {
                    this.Host.ShowMessageBox(string.Format("Rule {0} is not bound to a valid property.", rule.Name), MessageBoxActions.Ok);
                    return false;
                }

                if (rule.ParseError)
                {
                    this.Host.ShowMessageBox("One or more rules have syntax errors which must be corrected first.", MessageBoxActions.Ok);
                    return false;
                }

				var fragment = xmlDoc.CreateDocumentFragment();
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
