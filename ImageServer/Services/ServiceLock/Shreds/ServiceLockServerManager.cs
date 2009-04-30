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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.ServiceLock;

namespace ClearCanvas.ImageServer.Services.ServiceLock.Shreds
{
	public class ServiceLockServerManager : ThreadedService
	{
		#region Private Members
		private static ServiceLockServerManager _instance;
		private ServiceLockProcessor _theProcessor;
		#endregion

		#region Constructors
		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private ServiceLockServerManager(string name) : base(name)
		{ }
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static ServiceLockServerManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ServiceLockServerManager("ServiceLock");

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

		#region Protected Methods
		protected override void Initialize()
		{
			if (_theProcessor == null)
			{
				// Force a read context to be opened.  When developing the retry mechanism 
				// for startup when the DB was down, there were problems when the type
				// initializer for enumerated values were failng first.  For some reason,
				// when the database went back online, they would still give exceptions.
				// changed to force the processor to open a dummy DB connect and cause an 
				// exception here, instead of getting to the enumerated value initializer.
				using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
				}

				_theProcessor = new ServiceLockProcessor(2, ThreadStop); // 2 threads for processor
			}
		}

		protected override void Run()
		{
			_theProcessor.Run();
		}

		protected override void Stop()
		{
			if (_theProcessor != null)
			{
				_theProcessor.Stop();
				_theProcessor = null;
			}
		}
		#endregion
	}
}