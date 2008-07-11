using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using System.IO;

namespace ClearCanvas.Desktop.Actions
{
#if DEBUG   // only include this tool in debug builds

	/// <summary>
	/// Exports the in-memory action model to a file.
	/// </summary>
	[MenuAction("apply", "global-menus/MenuTools/MenuUtilities/Export Action Model", "Apply")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ExportActionModelTool : Tool<IDesktopToolContext>
	{
		internal void Apply()
		{
			FileDialogResult result = this.Context.DesktopWindow.ShowSaveFileDialogBox(new FileDialogCreationArgs("actionmodel.xml"));
			if(result.Action == DialogBoxAction.Ok)
			{
				try
				{
					using (StreamWriter sw = File.CreateText(result.FileName))
					{
						using (XmlTextWriter writer = new XmlTextWriter(sw))
						{
							writer.Formatting = Formatting.Indented;
							ActionModelSettings.Default.Export(writer);
						}
					}
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
		}
	}

#endif
}
