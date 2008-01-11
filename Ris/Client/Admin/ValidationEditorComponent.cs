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
        class Rule
        {
            private readonly PropertyInfo _property;
            private string _ruleXml;
            private string _parseError;

            public Rule(PropertyInfo property, string ruleXml)
            {
                _property = property;
                _ruleXml = ruleXml;
            }

            public PropertyInfo Property
            {
                get { return _property; }
            }

            public string RuleXml
            {
                get { return _ruleXml; }
                set
                {
                    if (_ruleXml != value)
                    {
                        _ruleXml = value;
                        Validate();
                    }
                }
            }

            public string ParseError
            {
                get { return _parseError; }
            }

            private void Validate()
            {
                try
                {
                    string xml = string.Format("<specifications>{0}</specifications>", _ruleXml);
                    SpecificationFactory factory = new SpecificationFactory(new StringReader(xml));
                    factory.GetSpecification(_property.Name);
                    _parseError = null;
                }
                catch (Exception e)
                {
                    _parseError = e.Message;
                }
            }
        }


        private const string _specTagName = "spec";

        private readonly Type _applicationComponent;
        private readonly XmlDocument _xmlDoc;

        private readonly Table<Rule> _rules;
        private Rule _selectedRule;

        private string _ruleXml;

        private IConfigurationStore _configStore;


        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationEditorComponent(Type applicationComponent)
        {
            _applicationComponent = applicationComponent;
            _xmlDoc = new XmlDocument();
            _xmlDoc.PreserveWhitespace = true;

            _rules = new Table<Rule>();
            _rules.Columns.Add(new TableColumn<Rule, string>("Property",
                delegate(Rule p) { return p.Property.Name; }));
            _rules.Columns.Add(new TableColumn<Rule, string>("Type",
                delegate(Rule p) { return p.Property.PropertyType.FullName; }));
            _rules.Columns.Add(new TableColumn<Rule, string>("Parse Error",
                delegate(Rule p) { return p.ParseError; }));
        }

        public override void Start()
        {
            // try to get the config store
            // if this fails, an exception will be thrown, preventing this component from starting
            _configStore = ConfigurationStoreFactory.GetDefaultStore();

            // load existing validation rules
            // if this fails, an exception will be thrown, preventing this component from starting
            LoadValidationDocument();

            // select the settable properties excluding those defined by IApplicationComponent
            _rules.Items.AddRange(
                CollectionUtils.Map<PropertyInfo, Rule>(
                    CollectionUtils.Select(_applicationComponent.GetProperties(),
                        delegate(PropertyInfo p)
                        {
                            return p.GetSetMethod() != null
                                && !p.DeclaringType.Equals(typeof(IApplicationComponent))
                                && !p.DeclaringType.Equals(typeof(ApplicationComponent));
                        }),
                    delegate(PropertyInfo p)
                    {
                        XmlElement specNode = GetSpecification(p.Name);
                        string ruleXml = specNode != null ? 
                            specNode.OuterXml : string.Format("<{0} id=\"{1}\">\n</{2}>", _specTagName, p.Name, _specTagName);
                        return new Rule(p, ruleXml);
                    }));


            base.Start();
        }


        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model


        public ITable Rules
        {
            get { return _rules; }
        }

        public ISelection SelectedRule
        {
            get { return new Selection(_selectedRule); }
            set
            {
                Rule selected = (Rule)value.Item;
                if (!Equals(_selectedRule, selected))
                {
                    ChangeSelectedProperty(selected);
                }
            }
        }

        public string RuleXml
        {
            get { return _ruleXml; }
            set { _ruleXml = value; }
        }

        public void Accept()
        {
            // commit final changes (hack - just re-select the already selected property)
            ChangeSelectedProperty(_selectedRule);

            try
            {
                if (SaveChanges())
                {
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionSaveValidationRules, this.Host.DesktopWindow,
                    delegate
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void ChangeSelectedProperty(Rule selected)
        {
            if (_selectedRule != null)
            {
                _selectedRule.RuleXml = _ruleXml;

                // notify updated in case error state changed
                _rules.Items.NotifyItemUpdated(_selectedRule);
            }

            _selectedRule = selected;
            _ruleXml = null;
            
            if (_selectedRule != null)
            {
                _ruleXml = _selectedRule.RuleXml;
                this.NotifyPropertyChanged("ValidationXml");
            }
        }

        private XmlElement GetSpecification(string id)
        {
            return (XmlElement)CollectionUtils.SelectFirst(_xmlDoc.GetElementsByTagName(_specTagName),
                delegate(object node) { return ((XmlElement)node).GetAttribute("id") == id; });
        }

        private bool SaveChanges()
        {
            foreach (Rule rule in _rules.Items)
            {
                if (rule.ParseError != null)
                {
                    this.Host.ShowMessageBox("One or more rules have errors.  Correct the errors before saving.", MessageBoxActions.Ok);
                    return false;
                }

                XmlDocumentFragment fragment = _xmlDoc.CreateDocumentFragment();
                try
                {
                    fragment.InnerXml = rule.RuleXml;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, string.Format("Error parsing rule {0}", rule.Property.Name), this.Host.DesktopWindow);
                    _selectedRule = rule;
                    NotifyPropertyChanged("SelectedProperty");

                    // abort
                    return false;
                }

                // update the xml document
                XmlElement specNode = GetSpecification(rule.Property.Name);
                if (specNode == null)
                {
                    _xmlDoc.DocumentElement.AppendChild(fragment);
                }
                else
                {
                    _xmlDoc.DocumentElement.ReplaceChild(fragment, specNode);
                }
            }

            SaveValidationDocument();

            return true;
        }

        private void LoadValidationDocument()
        {
            try
            {
                TextReader reader = _configStore.GetDocument(
                    GetDocumentName(_applicationComponent),
                    _applicationComponent.Assembly.GetName().Version,
                    null,
                    null);
                _xmlDoc.Load(reader);
            }
            catch (ConfigurationDocumentNotFoundException e)
            {
                // create blank document
                _xmlDoc.LoadXml("<specifications/>");
            }
        }

        private void SaveValidationDocument()
        {
            StringBuilder sb = new StringBuilder();
            XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb));
            writer.Formatting = System.Xml.Formatting.Indented;
            _xmlDoc.Save(writer);
            _configStore.PutDocument(
                GetDocumentName(_applicationComponent),
                _applicationComponent.Assembly.GetName().Version,
                null,
                null,
                new StringReader(sb.ToString())
                );
        }

        private static string GetDocumentName(Type appComponentClass)
        {
            return string.Format("{0}.val.xml", appComponentClass.FullName);
        }
    }
}
