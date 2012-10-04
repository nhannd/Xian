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
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using System.IO;


namespace ClearCanvas.VSWizards
{
    public class ToolWizard : Wizard
    {
        private ToolTemplate _selectedTemplate;

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            // create default values for all of the custom tags that we are going to use later
            string toolFile = Tags["$rootname$"];
            string toolName = Tags["$tool$"] = toolFile.Replace(".cs", "");

            List<string> classNames = new List<string>();
            classNames.Add(string.Format("{0} ({1})", toolName, toolFile));

            List<ToolTemplate> templates = new List<ToolTemplate>();
            templates.Add(new ToolTemplate("General Tool", "GeneralTool.cs", null, null));
			templates.Add(new ToolTemplate("Image Viewer Tool", "ImageViewerTool.cs", "ClearCanvas.ImageViewer.ImageViewerToolExtensionPoint", "ClearCanvas.ImageViewer.BaseTools.IImageViewerToolContext"));
            templates.Add(new ToolTemplate("Mouse Image Viewer Tool", "MouseImageViewerTool.cs", "ClearCanvas.ImageViewer.ImageViewerToolExtensionPoint", "ClearCanvas.ImageViewer.BaseTools.IImageViewerToolContext"));

            List<ToolExtPoint> extensionPoints = new List<ToolExtPoint>();
            extensionPoints.Add(new ToolExtPoint("ClearCanvas.Desktop.DesktopToolExtensionPoint", "ClearCanvas.Desktop.IDesktopToolContext"));
            extensionPoints.Add(new ToolExtPoint("ClearCanvas.ImageViewer.ImageViewerToolExtensionPoint", "ClearCanvas.ImageViewer.BaseTools.IImageViewerToolContext"));

            ToolForm form = new ToolForm(classNames, templates, extensionPoints);
            if (form.ShowDialog() == DialogResult.Cancel)
            {
                this.Abort = true;
                return;
            }
            _selectedTemplate = form.SelectedTemplate;

            ToolExtPoint selectedExtPoint = form.SelectedExtensionPoint;

            Tags["$tool_ext_point$"] = selectedExtPoint != null ? selectedExtPoint.ExtensionPoint : form.CustomExtensionPoint;
            Tags["$tool_context$"] = selectedExtPoint != null ? selectedExtPoint.ContextInterface : form.CustomToolContext;
        }

        public override bool ShouldAddProjectItem(string filePath)
        {
            // only add the selected item and the png files
            return base.ShouldAddProjectItem(filePath) && (filePath == _selectedTemplate.File || filePath.EndsWith(".png"));
        }

        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            // do our own tag replacement, that is more sophisticated than what is natively offered by VS
            string filepath = projectItem.get_FileNames(0);
            if (filepath.EndsWith(".cs"))
            {
                ReplaceTagsInFile(filepath, Tags);
            }
            else if (filepath.EndsWith(".png"))
            {
/*                StringBuilder sb = new StringBuilder();
                foreach (Property p in projectItem.Properties)
                {
                    sb.Append(string.Format("{0} - {1}\n", p.Name, p.Value));
                }
                MessageBox.Show(sb.ToString());
                */

                Project project = projectItem.ContainingProject;
                ProjectItem iconsFolder = FindIconsFolder(project);

                // icons folder, doesn't exist, try to create it in the project
                if (iconsFolder == null)
                {
                    try
                    {
                        iconsFolder = project.ProjectItems.AddFolder("Icons", null);
                    }
                    catch
                    {
                        // couldn't create the icons folder
                        // it probably exists on disk, even though it is not in the project
                        MessageBox.Show("Unable to create Icons folder.  Check if the folder already exists on disk.");
                        this.Abort = true;
                    }
                }

                // do we have an icons folder
                if (iconsFolder != null)
                {
                    // copy the icon to the Icons folder
                    ProjectItem iconItem = iconsFolder.ProjectItems.AddFromFileCopy(filepath);

                    // set its build action to "embedded resource"
                    iconItem.Properties.Item("BuildAction").Value = 3;
                }

                // delete it from the main folder
                projectItem.Delete();
            }
        }

        private ProjectItem FindIconsFolder(Project project)
        {
            foreach (ProjectItem item in project.ProjectItems)
            {
                if (item.Name == "Icons")
                    return item;
            }
            return null;
        }
    }
}
