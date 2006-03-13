using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
	public interface IObservableList<TItem, TItemEventArgs> 
		: IList<TItem>
		where TItemEventArgs : CollectionEventArgs<TItem>
	{
		event EventHandler<TItemEventArgs> ItemAdded;

		event EventHandler<TItemEventArgs> ItemRemoved;
	}
}
