#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
