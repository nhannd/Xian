using System;

namespace ClearCanvas.Common.Application
{
	/// <summary>
	/// Summary description for IMemorable.
	/// </summary>
	public interface IMemorable
	{
		IMemento CreateMemento();

		void SetMemento(IMemento memento);
	}
}
