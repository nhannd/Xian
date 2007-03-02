using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The exception that is thrown when a study or image fails to open.
	/// </summary>
	public class OpenStudyException : Exception
	{
		private int _totalImages;
		private int _failedImages;

		/// <summary>
		/// Instantiates a new instance of <see cref="OpenStudyException"/>.
		/// </summary>
		public OpenStudyException() { }

		/// <summary>
		/// Instantiates a new instance of <see cref="OpenStudyException"/> with
		/// a specified message.
		/// </summary>
		public OpenStudyException(string message) : base(message) { }

		/// <summary>
		/// Instantiates a new instance of <see cref="OpenStudyException"/> with
		/// a specified message and inner exception.
		/// </summary>
		public OpenStudyException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Gets or sets the total number of images the Framework tried to open.
		/// </summary>
		public int TotalImages
		{
			get { return _totalImages; }
			set { _totalImages = value; }
		}

		/// <summary>
		/// Gets or sets the number of images that failed to open.
		/// </summary>
		public int FailedImages
		{
			get { return _failedImages; }
			set { _failedImages = value; }
		}

		/// <summary>
		/// Gets the number of images that open successfully.
		/// </summary>
		public int SuccessfulImages
		{
			get { return this.TotalImages - this.FailedImages; }
		}
	}
}
