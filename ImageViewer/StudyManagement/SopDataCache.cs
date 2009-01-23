using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal interface ISopDataCacheItemReference : IDisposable
	{
		ISopDataSource RealDataSource { get; }
		ISopDataCacheItemReference Clone();
	}

	internal static class SopDataCache
	{
		#region Cache Item

		private class Item
		{
			private readonly ISopDataSource _realDataSource;
			private int _referenceCount = 0;

			public Item(ISopDataSource realDataSource)
			{
				_realDataSource = realDataSource;
			}

			public ISopDataSource RealDataSource
			{
				get { return _realDataSource; }	
			}

			public void OnReferenceCreated()
			{
				lock (_syncLock)
				{
					if (_referenceCount < 0)
						throw new ObjectDisposedException("The underlying object has been disposed.");

					++_referenceCount;
				}
			}

			public void OnReferenceDisposed()
			{
				lock(_syncLock)
				{
					if (_referenceCount > 0)
						--_referenceCount;

					if (_referenceCount == 0)
					{
						_referenceCount = -1;
						_items.Remove(RealDataSource.SopInstanceUid);
						_realDataSource.Dispose();

						if (_items.Count == 0)
							Trace.WriteLine("The sop data cache is empty.");
					}
				}
			}
		}

		#endregion
		
		#region Cached Data Source

		private class ItemReference : ISopDataCacheItemReference
		{
			private Item _item;

			public ItemReference(Item item)
			{
				_item = item;
				_item.OnReferenceCreated();
			}

			#region ICachedSopDataSource Members

			public ISopDataSource RealDataSource
			{
				get { return _item.RealDataSource; }
			}

			public ISopDataCacheItemReference Clone()
			{
				return new ItemReference(_item);
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_item != null)
				{
					_item.OnReferenceDisposed();
					_item = null;
				}
			}

			#endregion
		}
		
		#endregion

		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, WeakReference> _items = new Dictionary<string, WeakReference>();
		
#if UNIT_TESTS

		internal static int ItemCount
		{
			get { return _items.Count; }
		}

#endif
		public static ISopDataCacheItemReference Add(ISopDataSource dataSource)
		{
			lock(_syncLock)
			{
				WeakReference weakReference = null;
				Item item = null;

				if (_items.ContainsKey(dataSource.SopInstanceUid))
				{
					weakReference = _items[dataSource.SopInstanceUid];
					try
					{
						item = weakReference.Target as Item;
					}
					catch (InvalidOperationException)
					{
						weakReference = null;
					}
				}

				if (weakReference == null)
				{
					weakReference = new WeakReference(null);
					_items[dataSource.SopInstanceUid] = weakReference;
				}


				if (item != null && weakReference.IsAlive)
				{
					if (!ReferenceEquals(item.RealDataSource, dataSource))
						dataSource.Dispose(); //silently dispose the new one, we already have it.
				}
				else
				{
					item = new Item(dataSource);
					weakReference.Target = item;
				}

				return new ItemReference(item);
			}
		}
	}
}
