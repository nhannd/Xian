#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class RegexIsMatch : Task
	{
		private string _pattern;
		private string _input;
		private bool _isMatch;

		[Output]
		public bool IsMatch
		{
			get { return _isMatch; }
		}

		[Required]
		public string Pattern
		{
			get { return _pattern; }
			set { _pattern = value; }
		}

		[Required]
		public string Input
		{
			get { return _input; }
			set { _input = value; }
		}

		public override bool Execute()
		{
			if (String.IsNullOrEmpty(_input))
				return false;

			if (String.IsNullOrEmpty(_pattern))
				return false;

			_isMatch = Regex.IsMatch(_input, _pattern);

			return true;
		}
	}
}
