using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface ILutManager<T, U> : IMemorable
		where T : ILut
		where U : LutCreationParameters
	{
		T GetLut();
		void InstallLut(U creationParameters);
	}
}
