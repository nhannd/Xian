#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal interface ISopDataCacheItemReference : IDisposable
	{
		ISopDataSource RealDataSource { get; }
		IList<VoiDataLut> VoiDataLuts { get; }
		ISopDataCacheItemReference Clone();
	}

	//TODO: implement memory manager for the cache.
	internal static class SopDataCache
	{
		#region Cache Item

		private class Item
		{
			private readonly ISopDataSource _realDataSource;
			private IList<VoiDataLut> _sopVoiDataLuts;
			private int _referenceCount = 0;

			public Item(ISopDataSource realDataSource)
			{
				_realDataSource = realDataSource;
			}

			~Item()
			{
				lock (_syncLock)
				{
					// perform dictionary pruning if we are being finalized yet some client(s) has not properly disposed us yet
					if (_referenceCount >= 0)
					{
						string key = null;
						foreach (KeyValuePair<string, Item> item in _items)
						{
							if (item.Value == this)
							{
								key = item.Key;
								break;
							}
						}
						if (key != null)
							_items.Remove(key);
					}
				}
			}

			public ISopDataSource RealDataSource
			{
				get { return _realDataSource; }	
			}

			public IList<VoiDataLut> VoiDataLuts
			{
				get
				{
					if (_sopVoiDataLuts == null)
					{
						try
						{
							_sopVoiDataLuts = VoiDataLut.Create(_realDataSource).AsReadOnly();
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Warn, ex, SR.MessageFailedToGetVOIDataLUTs);
							_sopVoiDataLuts = new List<VoiDataLut>().AsReadOnly();
						}
					}

					return _sopVoiDataLuts;
				}
			}

			public void OnReferenceCreated()
			{
				lock (_syncLock)
				{
					if (_referenceCount < 0)
						throw new ObjectDisposedException("The underlying sop data source has been disposed.");

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

			public IList<VoiDataLut> VoiDataLuts
			{
				get { return _item.VoiDataLuts; }
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
		private static readonly Dictionary<string, Item> _items = new Dictionary<string, Item>();
		
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
				Item item = null;

				if (_items.ContainsKey(dataSource.SopInstanceUid))
				{
					item = _items[dataSource.SopInstanceUid];
					if (!ReferenceEquals(item.RealDataSource, dataSource))
						dataSource.Dispose(); //silently dispose the new one, we already have it.
				}
				else
				{
					item = new Item(dataSource);
					_items.Add(dataSource.SopInstanceUid, item);
				}

				return new ItemReference(item);
			}
		}
	}
}
