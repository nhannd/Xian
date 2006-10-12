using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="JscriptComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class JscriptComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// JscriptComponent class
    /// </summary>
    [AssociateView(typeof(JscriptComponentViewExtensionPoint))]
    public class JscriptComponent : ApplicationComponent
    {
        private string _script;
        private string _result;

        private IScriptEngine _scriptEngine;


        /// <summary>
        /// Constructor
        /// </summary>
        public JscriptComponent()
        {
        }

        public override void Start()
        {
            _scriptEngine = ScriptEngineFactory.CreateEngine("jscript");

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public string Script
        {
            get { return _script; }
            set { _script = value; }
        }

        public string Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    NotifyPropertyChanged("Result");
                }
            }
        }

        public void RunScript()
        {
            Dictionary<string, object> context = new Dictionary<string,object>();

            PatientProfile patient = PatientProfile.New();
            patient.Name.FamilyName = "Johnson";
            patient.Name.GivenName = "Ben";

            context["patient"] = patient;
            
            try
            {
                object result = _scriptEngine.Run(_script, context);
                this.Result = result == null ? null : result.ToString();
            }
            catch (Exception e)
            {
                this.Result = e.Message;
            }
        }
	
    }
}
