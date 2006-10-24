using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public class InformationBoxChangedEventArgs : EventArgs
	{
		private InformationBox _informationBox;

		public InformationBoxChangedEventArgs(InformationBox informationBox)
		{
			_informationBox = informationBox;
		}

		public InformationBox InformationBox 
		{
			get { return _informationBox; }
		}
	}
}
