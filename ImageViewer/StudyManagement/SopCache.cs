using System.Diagnostics;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal static class SopCache
	{
		private static readonly object _syncLock = new object();
		private static readonly Dictionary<string, Sop> _sopCache;

		static SopCache()
		{
			_sopCache = new Dictionary<string, Sop>();
		}

		//for unit tests only.
		public static ImageSop Get(string sopInstanceUid)
		{
			Platform.CheckForEmptyString(sopInstanceUid, "sopInstanceUid");

			lock (_syncLock)
			{
				if (_sopCache.ContainsKey(sopInstanceUid))
					return (ImageSop) _sopCache[sopInstanceUid];

				return null;
			}
		}

		public static ImageSop Add(ImageSop sop)
		{
			Platform.CheckForNullReference(sop, "sop");

			string sopInstanceUid = sop.SopInstanceUID;
			lock (_syncLock)
			{
				if (!_sopCache.ContainsKey(sopInstanceUid))
				{
					sop.Disposing += OnSopDisposing;
					_sopCache[sopInstanceUid] = sop;
				}

				return (ImageSop)_sopCache[sopInstanceUid];
			}
		}

		private static void OnSopDisposing(object sender, System.EventArgs e)
		{
			Sop sop = (Sop)sender;
			sop.Disposing -= OnSopDisposing;
			
			Remove(sop);
		}

		private static void Remove(Sop sop)
		{
			lock (_syncLock)
			{
				_sopCache.Remove(sop.SopInstanceUID);
				if (_sopCache.Count == 0)
					Trace.WriteLine("The Sop cache is empty.");
			}
		}
	}
}
