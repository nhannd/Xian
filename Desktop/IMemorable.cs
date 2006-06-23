using System;

namespace ClearCanvas.Desktop
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
