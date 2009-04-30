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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public sealed class PresetVoiLutConfiguration : IEnumerable<KeyValuePair<string, string>>
	{
		private readonly IPresetVoiLutOperationFactory _factory;
		private readonly Dictionary<string, string> _configurationValues;

		private PresetVoiLutConfiguration(IPresetVoiLutOperationFactory factory)
		{
			_factory = factory;
			_configurationValues = new Dictionary<string, string>();
		}

		public string FactoryName
		{
			get { return _factory.Name; }
		}

		public static PresetVoiLutConfiguration FromFactory(IPresetVoiLutOperationFactory factory)
		{
			Platform.CheckForNullReference(factory, "factory");
			Platform.CheckForEmptyString(factory.Name, "factory.Name");
			return new PresetVoiLutConfiguration(factory);
		}

		public string this[string key]
		{
			get
			{
				if (!_configurationValues.ContainsKey(key))
					return null;

				return _configurationValues[key];
			}
			set
			{
				if (String.IsNullOrEmpty(key))
					return;

				if (_configurationValues.ContainsKey(key) && String.IsNullOrEmpty(value))
					_configurationValues.Remove(key);

				if (!String.IsNullOrEmpty(value))
					_configurationValues[key] = value;
			}
		}

		public void Clear()
		{
			_configurationValues.Clear();
		}

		public void CopyTo(IDictionary<string, string> dictionary)
		{
			dictionary.Clear();
			foreach (KeyValuePair<string, string> pair in _configurationValues)
				dictionary[pair.Key] = pair.Value;
		}

		#region IEnumerable<KeyValuePair<string,string>> Members

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _configurationValues.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _configurationValues.GetEnumerator();
		}

		#endregion
	}
}
