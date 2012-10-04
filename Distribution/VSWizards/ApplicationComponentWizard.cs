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
using System.IO;
using System.Windows.Forms;

using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace ClearCanvas.VSWizards
{
    public class ApplicationComponentWizard : Wizard
    {
        private string _viewProjectName;

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            // create default values for all of the custom tags that we are going to use later
            string componentFile = Tags["$rootname$"];
            string componentName = Tags["$app_comp$"] = componentFile.Replace(".cs", "");
            Tags["$app_comp_view$"] = componentName + "View";

            Tags["$app_comp_control$"] = componentName + "Control";
            Tags["$app_comp_view_extpoint$"] = componentName + "ViewExtensionPoint";

            string viewFile = Tags["$app_comp_view$"] + ".cs";
            string controlFile = Tags["$app_comp_control$"] + ".cs";

            // create a list of the classes that will be generated
            List<string> classNames = new List<string>();
            classNames.Add(string.Format("{0} ({1})", componentName, componentFile));
            classNames.Add(string.Format("{0} ({1})", Tags["$app_comp_view_extpoint$"], componentFile));
            classNames.Add(string.Format("{0} ({1})", Tags["$app_comp_view$"], viewFile));
            classNames.Add(string.Format("{0} ({1})", Tags["$app_comp_control$"], controlFile));

            // create a list of project names, and default view project
            List<string> projectNames = new List<string>();
            string defaultViewProject = null;
            foreach (Project proj in this.Dte.Solution.Projects)
            {
                // take a guess for the default view project - choose any project with "View.WinForms" in the name
                if (proj.Name.IndexOf("View.WinForms") > -1)
                    defaultViewProject = proj.Name;

                projectNames.Add(proj.Name);
            }

            // show a form to confirm, and allow the user to select the project where the view should go
            ApplicationComponentForm form = new ApplicationComponentForm(classNames, projectNames, defaultViewProject);
            if (form.ShowDialog() == DialogResult.Cancel)
            {
                this.Abort = true;
                return;
            }

            _viewProjectName = form.ViewProject;
            if (_viewProjectName == null || _viewProjectName.Length == 0)
            {
                MessageBox.Show("Please select a View project, or create one before adding this item");
                this.Abort = true;
                return;
            }
            
            // set the view namespace
            Tags["$app_comp_view_namespace$"] = _viewProjectName;
        }

        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            base.ProjectItemFinishedGenerating(projectItem);

            // do our own tag replacement, that is more sophisticated than what is natively offered by VS
            string filepath = projectItem.get_FileNames(0);
            ReplaceTagsInFile(filepath, Tags);

            if (projectItem.Name == Tags["$rootname$"])
            {
                // this is the component, so no special processing needed
            }
            else
            {
                // this is the view or the control, so it needs to be copied
                // to the view project and deleted from the component project
                Project viewProject = FindProject(_viewProjectName);
                viewProject.ProjectItems.AddFromFileCopy(filepath);
                projectItem.Delete();
            }
        }

    }
}
