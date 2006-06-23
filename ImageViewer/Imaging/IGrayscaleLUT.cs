using System;

namespace ClearCanvas.Workstation.Model.Imaging
{
	/// <summary>
	/// Summary description for IGrayscaleLUT.
	/// </summary>
	public interface IGrayscaleLUT : ILUT
	{
		int MinInputValue
		{
			get;
		}

		int MaxInputValue
		{
			get;
		}

		int MinOutputValue
		{
			get;
		}

		int MaxOutputValue
		{
			get;
		}
	}
}
