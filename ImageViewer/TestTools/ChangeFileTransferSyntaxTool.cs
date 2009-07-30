using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.Common;
using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class ChangeFileTransferSyntaxTool : Tool<ILocalImageExplorerToolContext>
	{
		public ChangeFileTransferSyntaxTool()
		{
		}

		public override IActionSet Actions
		{
			get
			{
				List<IAction> actions = new List<IAction>();
				IResourceResolver resolver = new ResourceResolver(typeof(ChangeTransferSyntaxTool).GetType().Assembly);

				actions.Add(CreateAction(TransferSyntax.ExplicitVrLittleEndian, resolver));
				actions.Add(CreateAction(TransferSyntax.ImplicitVrLittleEndian, resolver));

				foreach (IDicomCodecFactory factory in ClearCanvas.Dicom.Codec.DicomCodecRegistry.GetCodecFactories())
				{
					actions.Add(CreateAction(factory.CodecTransferSyntax, resolver));
				}

				return new ActionSet(actions);
			}
		}

		private IAction CreateAction(TransferSyntax syntax, IResourceResolver resolver)
		{
			ClickAction action = new ClickAction(syntax.UidString,
					new ActionPath("explorerlocal-contextmenu/Change Transfer Syntax/" + syntax.ToString(), resolver),
					ClickActionFlags.None, resolver);
			action.SetClickHandler(delegate { ChangeToSyntax(syntax); });
			action.Label = syntax.ToString();
			return action;
		}

		private void ChangeToSyntax(TransferSyntax syntax)
		{
			string[] files = BuildFileList();
			const string directory = @"C:\Stewart\tmp";

			try
			{
				foreach (string file in files)
				{
					DicomFile dicomFile = new DicomFile(file);
					dicomFile.Load();
					dicomFile.ChangeTransferSyntax(syntax);
					string fileName = System.IO.Path.Combine(directory, dicomFile.MediaStorageSopInstanceUid);
					fileName += ".dcm";
					dicomFile.Save(fileName);
				}
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private string[] BuildFileList()
		{
			List<string> fileList = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList.ToArray();
		}
	}
}
