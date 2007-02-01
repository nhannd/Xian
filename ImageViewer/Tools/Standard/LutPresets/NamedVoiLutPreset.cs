using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	public sealed class NamedVoiLutPreset : LutPreset
	{
		private string _identifier;
		private bool _isDataLut;
		private string _lutName;

		public override string Label
		{
			get { return _identifier; }
			set { _identifier = value; }
		}

		public string LutName
		{
			get { return _lutName; }
			set { _lutName = value; }
		}

		public bool IsDataLut
		{
		    get { return _isDataLut; }
		    set { _isDataLut = value; }
		}

		private bool ApplyDataLut(IPresentationImage image)
		{
			throw new NotImplementedException("The method or operation is not implemented.");

			//FileDicomImage dicomFile = image.ImageSop.NativeDicomObject as FileDicomImage;
			//DcmDataset dataset = dicomFile.Dataset;
			//dataset.loadAllDataIntoMemory();

			//DcmSequenceOfItems sequenceOfItems = new DcmSequenceOfItems(new DcmTag(Dcm.VOILUTSequence));
			//if (sequenceOfItems == null)
			//    return false;

			//OFCondition condition = dataset.findAndGetSequence(Dcm.VOILUTSequence, sequenceOfItems);

			//uint i = 0;
			//DcmItem item = sequenceOfItems.getItem(i);
			//while (item != null)
			//{
			//    StringBuilder lutExplanation = new StringBuilder(64);
			//    item.findAndGetOFString(Dcm.LUTExplanation, lutExplanation);
				
			//    if (String.Compare(lutExplanation.ToString(), this.LutName, true) == 0)
			//    { 
			//        if (image.ImageSop.PixelRepresentation == 0)
			//        {
			//        }
			//    }

			//    item = sequenceOfItems.getItem(++i);
			//}

			//return false;
		}

		private bool ApplyLinearLut(IPresentationImage image)
		{
			throw new NotImplementedException("The method or operation is not implemented.");

			//Window[] windowCenterAndWidth = image.ImageSop.WindowCenterAndWidth;
			//if (windowCenterAndWidth == null || windowCenterAndWidth.Length == 0)
			//    return false;

			//string[] windowCenterAndWidthExplanations = image.ImageSop.WindowCenterAndWidthExplanation;
			//if (windowCenterAndWidthExplanations == null || windowCenterAndWidthExplanations.Length == 0)
			//    return false;

			//int lutIndex = 0;
			//for (; lutIndex < windowCenterAndWidthExplanations.Length; ++lutIndex)
			//{
			//    if (String.Compare(LutName, windowCenterAndWidthExplanations[lutIndex], true) == 0)
			//        break;
			//}

			//if (lutIndex > windowCenterAndWidth.Length - 1)
			//    return false;

			//WindowLevelApplicator applicator = new WindowLevelApplicator(image);
			//UndoableCommand command = new UndoableCommand(applicator);
			//command.Name = SR.CommandWindowLevel;
			//command.BeginState = applicator.CreateMemento();

			//Window newWindow = windowCenterAndWidth[lutIndex];
			//WindowLevelOperator.InstallVOILUTLinear(image, newWindow.Width, newWindow.Center);
			//image.Draw();

			//command.EndState = applicator.CreateMemento();
			//if (!command.EndState.Equals(command.BeginState))
			//{
			//    applicator.SetMemento(command.EndState);
			//    image.ImageViewer.CommandHistory.AddCommand(command);
			//}

			//return true;
		}

		public override bool Apply(IPresentationImage image)
		{
			try
			{
				if (_isDataLut)
				{
					return ApplyDataLut(image);
				}
				else
				{
					return ApplyLinearLut(image);
				}
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}

			return false;
		}
	}
}
