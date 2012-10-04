#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using System.IO;

namespace ClearCanvas.VSWizards
{
    /// <summary>
    /// Abstract base class for all wizards.  Provides default implementations of all <see cref="IWizard"/> methods.
    /// Subclasses should be sure to call the base class methods.
    /// </summary>
    public abstract class Wizard : IWizard
    {
        private DTE _dte;
        private bool _abort;
        private IDictionary<string, string> _tags;

        

        #region IWizard Members

        /// <summary>
        /// Called when the wizard first starts up.  If a subclass overrides this method, it should call the base 
        /// method first before doing further processing.
        /// </summary>
        /// <param name="automationObject"></param>
        /// <param name="replacementsDictionary"></param>
        /// <param name="runKind"></param>
        /// <param name="customParams"></param>
        public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _dte = (DTE)automationObject;
            _tags = replacementsDictionary;
        }

        /// <summary>
        /// Called for each item in the template, before the item is processed. Returning false from 
        /// this method will cancel the generation of that particular item.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public virtual bool ShouldAddProjectItem(string filePath)
        {
            return !_abort;
        }

        /// <summary>
        /// Called when the project has been generated, if the template was a project template.
        /// (This method is not called for item templates)
        /// </summary>
        /// <param name="project"></param>
        public virtual void ProjectFinishedGenerating(Project project)
        {
        }

        /// <summary>
        /// Called when an item has been generated, for item templates.
        /// (This method is not called for project templates)
        /// </summary>
        /// <param name="projectItem"></param>
        public virtual void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Called before opening a generated project item, if that item is supposed to open automatically.
        /// </summary>
        /// <param name="projectItem"></param>
        public virtual void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Called when the wizard is no longer needed
        /// </summary>
        public virtual void RunFinished()
        {
        }

        #endregion

        /// <summary>
        /// Returns the DTE automation object
        /// </summary>
        protected DTE Dte
        {
            get { return _dte; }
        }

        protected bool Abort
        {
            get { return _abort; }
            set { _abort = value; }
        }

        protected IDictionary<string, string> Tags
        {
            get { return _tags; }
        }


        /// <summary>
        /// Convenience method to find a project in the solution by its name
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        protected Project FindProject(string projectName)
        {
            foreach (Project proj in Dte.Solution.Projects)
            {
                if (proj.Name == projectName)
                    return proj;
            }
            return null;
        }

        /// <summary>
        /// Convenience method to do tag replacement in a file, in order to support replacing tags
        /// that are not supported natively by Visual Studio.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="replacements"></param>
        protected void ReplaceTagsInFile(string filename, IDictionary<string, string> replacements)
        {
            string content = File.ReadAllText(filename);
            foreach (string tag in replacements.Keys)
            {
                content = content.Replace(tag, replacements[tag]);
            }

            File.WriteAllText(filename, content);
        }

    }
}
