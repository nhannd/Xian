using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public class DoubleFormatter
	{
		private int _precision;
		private const char _separator = ',';
		private string _formatString;

		public DoubleFormatter()
			: this(2)
		{
		}

		public DoubleFormatter(int precision)
		{
			_precision = precision;
			_formatString = String.Format("F{0}", _precision);
		}

		public string Format(double input)
		{
			return input.ToString(_formatString);
		}

		public string FormatList(IEnumerable<double> input)
		{
			string result = "";
			foreach (double inputValue in input)
				result += String.Format("{0}{1}\n", inputValue.ToString(_formatString), _separator);

			if (!String.IsNullOrEmpty(result))
				result = result.Remove(result.Length - 2);

			return result;
		}
	}
}
