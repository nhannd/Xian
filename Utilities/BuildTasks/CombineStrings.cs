using System;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class CombineStrings : Task
	{
		private string[] _inputStrings;
		private string _delimiter;
		private string _combinedString;

		[Required]
		public string[] InputStrings
		{
			get { return _inputStrings; }
			set { _inputStrings = value; }
		}

		public string Delimiter
		{
			get { return _delimiter; }
			set { _delimiter = value; }
		}

		[Output]
		public string CombinedString
		{
			get { return _combinedString; }
		}

		public override bool Execute()
		{
			if (_inputStrings == null)
				return false;

			StringBuilder builder = new StringBuilder();
			int i = 0;
			foreach (string str in _inputStrings)
			{
				if (String.IsNullOrEmpty(str))
					continue;

				if (i++ > 0)
					builder.Append(_delimiter ?? "");
				builder.Append(str);
			}

			_combinedString = builder.ToString();

			return true;
		}
	}
}
