using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal interface ICacheableSop : IDisposable
	{
		string SopInstanceUID { get; }
		bool IsReferenceCountZero { get; }

		void Load();
		void Unload();
		void IncrementReferenceCount();
		void DecrementReferenceCount();

#if UNIT_TESTS
		int ReferenceCount { get; }
#endif
	}
}
