#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS
// ReSharper disable LocalizableElement

using System;
using System.Threading;

namespace ClearCanvas.Common.Utilities.Tests
{
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	internal sealed class ExclusiveLockTestApplication : IApplicationRoot
	{
		private class CommandLine : Utilities.CommandLine
		{
			public CommandLine()
			{
				HoldTime = 15;
				LockName = string.Empty;
				AddGlobalPrefix = false;
			}

			[CommandLineParameter("g", "global", @"Prefixes lock name with Global\", Required = false)]
			public bool AddGlobalPrefix { get; set; }

			[CommandLineParameter("n", "name", @"Name of lock", Required = false)]
			public string LockName { get; set; }

			[CommandLineParameter("t", "time", "Time to hold lock in seconds", Required = false)]
			public int HoldTime { get; set; }
		}

		private ExclusiveLock CreateLock(string baseName, bool useGlobalPrefix)
		{
			if (useGlobalPrefix) return new NamedMutexLock(@"Global\" + baseName);
			return ExclusiveLock.CreateFileSystemLock(baseName);
		}

		public void RunApplication(string[] args)
		{
			var commandLine = new CommandLine();
			commandLine.Parse(args);

			using (var @lock = CreateLock(string.IsNullOrEmpty(commandLine.LockName) ? @"F17BFCF1-0832-4575-9C7D-F30D8C359159" : commandLine.LockName, commandLine.AddGlobalPrefix))
			{
				Console.WriteLine("Acquiring lock");

				@lock.Lock();
				Console.WriteLine("Acquired lock");
				try
				{
					Console.WriteLine("Holding lock for {0} seconds", commandLine.HoldTime);
					Thread.Sleep(1000*commandLine.HoldTime);
				}
				finally
				{
					Console.WriteLine("Released lock");
					@lock.Unlock();
				}
			}

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey(true);
		}
	}
}

// ReSharper restore LocalizableElement
#endif