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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// A default implementation of <see cref="IConfigurationPage"/>.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="IConfigurationApplicationComponent"/>-derived 
	/// class that will be hosted in this page.  The class must have a parameterless default constructor.</typeparam>
	public class ConfigurationPage<T> : ConfigurationPage
		 where T : IConfigurationApplicationComponent, new()
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to the <see cref="ConfigurationPage{T}"/>.</param>
		public ConfigurationPage(string path)
			: base(path, new T())
		{
		}
	}
	
	public class ConfigurationPage : IConfigurationPage
	{
		private readonly IConfigurationApplicationComponent _component;
		private readonly string _path;

		public ConfigurationPage(string path, IConfigurationApplicationComponent component)
		{
			_path = path;
			_component = component;
		}

		#region IConfigurationPage Members

		/// <summary>
		/// Gets the path to this page.
		/// </summary>
		public string GetPath()
		{
			return _path;
		}

		/// <summary>
		/// Gets the <see cref="IApplicationComponent"/> that is hosted in this page.
		/// </summary>
		public IApplicationComponent GetComponent()
		{
			return _component;
		}

		/// <summary>
		/// Saves any configuration changes that were made.
		/// </summary>
		public void SaveConfiguration()
		{
			_component.Save();
		}

		#endregion
	}
}
