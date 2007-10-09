using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal interface ICacheableSop : IReferenceCountable, IDisposable
	{
		string SopInstanceUID { get; }

		void Load();
		void Unload();
	}
}
