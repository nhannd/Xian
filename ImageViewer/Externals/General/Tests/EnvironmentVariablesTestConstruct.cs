#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	public sealed class EnvironmentVariablesTestConstruct : IDisposable
	{
		private const string _format = "__CC_TEST_{0}";
		private readonly EnvironmentVariableTarget _target;
		private readonly IList<string> _keys;

		public EnvironmentVariablesTestConstruct(EnvironmentVariableTarget target)
		{
			_target = target;
			_keys = new List<string>();
		}

		public string this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
					throw new ArgumentNullException("key");
				string fullKey = string.Format(_format, key);

				return Environment.GetEnvironmentVariable(fullKey, _target);
			}
			set
			{
				if (string.IsNullOrEmpty(key))
					throw new ArgumentNullException("key");
				string fullKey = string.Format(_format, key);

				Environment.SetEnvironmentVariable(fullKey, value, _target);
				if (!_keys.Contains(key))
					_keys.Add(key);
			}
		}

		public string Format(string key)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");
			return string.Format("%{0}%", string.Format(_format, key));
		}

		public string PercentChar
		{
			get { return "%%"; }
		}

		public void Dispose()
		{
			foreach (var key in _keys)
			{
				try
				{
					string fullKey = string.Format(_format, key);
					Environment.SetEnvironmentVariable(fullKey, string.Empty, _target);
				}
				catch (Exception)
				{
					// don't throw exceptions in a Dispose()
				}
			}
		}
	}
}

#endif