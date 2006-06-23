using System;

namespace ClearCanvas.Workstation.Model.Imaging
{
	/// <summary>
	/// Summary description for ILUT.
	/// </summary>
	public interface ILUT
	{
		int NumEntries
		{
			get;
		}

		int this[int index]
		{
			get;
			set;
		}
	}
}
