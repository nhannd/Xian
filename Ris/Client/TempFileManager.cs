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
using System.IO;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using Timer=System.Threading.Timer;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Manages a cache of temporary files on disk.
	/// </summary>
	/// <remarks>
	/// Allows arbitrary data to be stored on disk in a Windows temporary file, with a specified time-to-live.
	/// Expired temporary files will be periodically cleaned up.  All temporary files, regardless of whether expired,
	/// will be deleted when the application shuts down, except in the case of a crash.
	/// </remarks>
    public class TempFileManager
	{
		#region Entry class

		/// <summary>
		/// Represents an entry in the map.
		/// </summary>
		class Entry
		{
			private readonly TimeSpan _timeToLive;
			private readonly string _file;
			private DateTime _expiryTime;

			public Entry(string file, TimeSpan ttl)
			{
				_file = file;
				_timeToLive = ttl;
				Renew();	// initialize expiry time
			}

			public string File
			{
				get { return _file; }
			}

			public bool IsExpired
			{
				get { return DateTime.Now > _expiryTime; }
			}

			public void Renew()
			{
				_expiryTime = DateTime.Now + _timeToLive;
			}
		}

		#endregion

		private static readonly TempFileManager _instance = new TempFileManager();

		private readonly Dictionary<EntityRef, Entry> _entryMap = new Dictionary<EntityRef, Entry>();
    	private readonly Timer _timer;
    	private readonly uint _sweepInterval = 15000;	// 15 seconds
		private readonly object _syncObj = new object();


		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
    	public static TempFileManager Instance
        {
            get { return _instance; }
        }

		#region Public API

		/// <summary>
		/// Creates a temporary file associated with the specified key, and containing the specified data.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="fileExtension"></param>
		/// <param name="data"></param>
		/// <param name="timeToLive"></param>
		/// <returns>The path of the file.</returns>
		public string CreateFile(EntityRef key, string fileExtension, byte[] data, TimeSpan timeToLive)
		{
			// write data to a file on disk
			string tempFileName = String.Format("{0}.{1}", System.IO.Path.GetTempFileName(), fileExtension);
			File.WriteAllBytes(tempFileName, data);

			// lock while we update the map
			lock (_syncObj)
			{
				// add entry for this file
				_entryMap.Add(key, new Entry(tempFileName, timeToLive));
				return tempFileName;
			}
		}

		/// <summary>
		/// Gets the temporary file associated with the specified key, if it exists, otherwise null.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetFile(EntityRef key)
		{
			// lock so that we don't read anything that is being removed concurrently by the sweep
			lock (_syncObj)
			{
				Entry entry;
				if (!_entryMap.TryGetValue(key, out entry))
					return null;

				// if the file does not actually exist for whatever reason, return null
				if (!File.Exists(entry.File))
					return null;

				// renew the item since it has just been accessed
				entry.Renew();

				return entry.File;
			}
		}

		#endregion

		private TempFileManager()
		{
			// set up timer to periodically remove expired entries
			_timer = new Timer(TimerCallback, null, _sweepInterval, _sweepInterval);

			// also remove all entries when the desktop is shutdown
			// (it isn't nice to have this dependency here, but seems to be no other easy way to do this)
			Desktop.Application.Quitting +=
				delegate
				{
					_timer.Dispose();
					Clean(delegate { return true; });
				};
		}

		private void TimerCallback(object state)
		{
			// clean expired entries
			Clean(delegate(Entry entry) { return entry.IsExpired; });
		}

		/// <summary>
		/// Delete all entries and associated files, matching the specified condition.
		/// </summary>
		/// <param name="condition"></param>
		private void Clean(Predicate<Entry> condition)
		{
			List<KeyValuePair<EntityRef, Entry>> kvps;

			// lock here, so that this operation is atomic
			lock (_syncObj)
			{
				// select list of entries that match the condition
				kvps = CollectionUtils.Select(
					_entryMap,
					delegate(KeyValuePair<EntityRef, Entry> kvp) { return condition(kvp.Value); });

				// remove all selected entries from map
				foreach (KeyValuePair<EntityRef, Entry> kvp in kvps)
				{
					_entryMap.Remove(kvp.Key);
				}
			}

			// do the more expensive operation of deleting the files outside of the lock
			// this is fine because we are not modifying the map anymore
			foreach (KeyValuePair<EntityRef, Entry> kvp in kvps)
			{
				try
				{
					File.Delete(kvp.Value.File);	// this is a nop if the file does not exist
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, SR.ExceptioinFailedToDeleteTemporaryFiles);
				}
			}
		}

    }
}
