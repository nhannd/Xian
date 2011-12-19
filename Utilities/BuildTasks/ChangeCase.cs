#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public enum ChangeCaseMode
	{
		LowerCase = 0,
		UpperCase = 1,
		Lower = LowerCase,
		Upper = UpperCase
	}

	public class ChangeCase : Task
	{
		[Output]
		[Required]
		public string String { get; set; }

		[Required]
		public string Mode { get; set; }

		public override bool Execute()
		{
			ChangeCaseMode mode;
			if (!TryGetChangeCaseMode(out mode))
			{
				Log.LogError("Mode should be one of LowerCase or UpperCase");
				return false;
			}

			if (!string.IsNullOrEmpty(String))
			{
				switch (mode)
				{
					case ChangeCaseMode.LowerCase:
						String = String.ToLowerInvariant();
						break;
					case ChangeCaseMode.UpperCase:
						String = String.ToUpperInvariant();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return true;
		}

		/// <summary>
		/// Parses the value of <see cref="Mode"/>.
		/// </summary>
		protected bool TryGetChangeCaseMode(out ChangeCaseMode result)
		{
			if (string.IsNullOrEmpty(Mode))
			{
				result = ChangeCaseMode.LowerCase;
				return true;
			}

			foreach (ChangeCaseMode eValue in Enum.GetValues(typeof (ChangeCaseMode)))
			{
				if (string.Equals(Mode, eValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					result = eValue;
					return true;
				}
			}

			result = ChangeCaseMode.LowerCase;
			return false;
		}
	}
}