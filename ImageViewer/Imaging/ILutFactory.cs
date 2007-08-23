using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface ILutFactory<T, U>
		where T : ILut
		where U : LutCreationParameters
	{
		string Name { get; }
		T Create(U creationParameters);
	}
}
