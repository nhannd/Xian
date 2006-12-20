using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public class OpenStudyException : Exception
	{
		private int _totalImages;
		private int _failedImages;

		public OpenStudyException() { }
		
		public OpenStudyException(string message) : base(message) { }
		
		public OpenStudyException(string message, Exception inner) : base(message, inner) { }

		public int TotalImages
		{
			get { return _totalImages; }
			set { _totalImages = value; }
		}

		public int FailedImages
		{
			get { return _failedImages; }
			set { _failedImages = value; }
		}

		public int SuccessfulImages
		{
			get { return this.TotalImages - this.FailedImages; }
		}
	}
}
