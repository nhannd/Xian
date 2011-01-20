#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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