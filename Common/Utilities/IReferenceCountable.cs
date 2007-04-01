using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public interface IReferenceCountable
	{
		void IncrementReferenceCount();
		void DecrementReferenceCount();
		bool IsReferenceCountZero { get; }

#if UNIT_TESTS
		int ReferenceCount { get; }
#endif
	}
}
