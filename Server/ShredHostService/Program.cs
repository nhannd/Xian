#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System;

namespace ClearCanvas.Server.ShredHostService
{
    static class Program
    {
		private static Assembly _assembly;
		private static Type _shredHostType;
		
		internal static void Start()
		{
			// the default startup path is in the system folder
			// we need to change this to be able to scan for plugins and to log
			string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
			System.IO.Directory.SetCurrentDirectory(startupPath);

			// we choose to dynamically load the ShredHost dll so that we can bypass
			// the requirement that the ShredHost dll be Strong Name signed, i.e.
			// if we were to reference it directly in the the project at design time
			// ClearCanvas.Server.ShredHost.dll would also need to be Strong Name signed
			_assembly = Assembly.Load("ClearCanvas.Server.ShredHost");
			_shredHostType = _assembly.GetType("ClearCanvas.Server.ShredHost.ShredHost");
			_shredHostType.InvokeMember("Start", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
				null, null, new object[] { });
		}

		internal static void Stop()
		{
			_shredHostType.InvokeMember("Stop", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
				null, null, new object[] { });
		}

    	/// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
			Start();
			Console.WriteLine("Press <Enter> to terminate the ShredHost.");
			Console.WriteLine();
			Console.ReadLine();
			Stop();
		}
    }
}