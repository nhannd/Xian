using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public class ReferenceCountedObjectCache
	{
		private Dictionary<string, IReferenceCountable> _cache = new Dictionary<string, IReferenceCountable>();

		public ReferenceCountedObjectCache()
		{

		}

		public void Add(string key, IReferenceCountable referenceCountedObject)
		{
			if (_cache.ContainsKey(key))
			{
				_cache[key].IncrementReferenceCount();
			}
			else
			{
				referenceCountedObject.IncrementReferenceCount();
				_cache.Add(key, referenceCountedObject);
			}
		}

		public void Remove(string key)
		{
			if (key == String.Empty)
				return;

			if (_cache.ContainsKey(key))
			{
				IReferenceCountable referenceCountedObject = _cache[key];
				referenceCountedObject.DecrementReferenceCount();

				if (referenceCountedObject.IsReferenceCountZero)
				{
					_cache.Remove(key);

					IDisposable disposable = referenceCountedObject as IDisposable;
					
					if (disposable != null)
						disposable.Dispose();
				}
			}
		}

		public IReferenceCountable this[string key]
		{
			get
			{
				if (_cache.ContainsKey(key))
					return _cache[key];
				else
					return null;
			}
		}

		public void Clear()
		{
			_cache.Clear();
		}
	}
}
