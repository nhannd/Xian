using System;

namespace ClearCanvas.Desktop
{
	public interface IMemorable
	{
		IMemento CreateMemento();

		void SetMemento(IMemento memento);
	}
}
