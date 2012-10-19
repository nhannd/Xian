#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Common.Manifest
{
	public static class RisServerManifestVerification
	{
		private static readonly object _syncRoot = new object();
		private static int _lastCheck;
		private static bool? _lastValid;

		public static bool Valid
		{
			get
			{
				const int cacheTimeout = 90000;

				if (_lastValid.HasValue && Math.Abs(Environment.TickCount - _lastCheck) < cacheTimeout) return _lastValid.Value;

				lock (_syncRoot)
				{
					if (!(_lastValid.HasValue && Math.Abs(Environment.TickCount - _lastCheck) < cacheTimeout))
					{
						_lastValid = Check();
						_lastCheck = Environment.TickCount;
					}
					return _lastValid.GetValueOrDefault(true); // just return true if server status unknown, since there will likely be other (more pressing) issues
				}
			}
		}

		private static bool? Check()
		{
			bool? valid = null;
			try
			{
				Platform.GetService<IRisManifestService>(s => valid = s.GetStatus(new GetStatusRequest()).IsValid);
			}
			catch (Exception)
			{
				// if the manifest service errors out, eat the exception and 
				valid = null;
			}
			return valid;
		}
	}
}