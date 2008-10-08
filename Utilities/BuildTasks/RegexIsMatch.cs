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
