#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	#region Exception Handler

	[ExceptionPolicyFor(typeof(LoadSopsException))]
	[ExceptionPolicyFor(typeof(LoadStudyException))]

	[ExceptionPolicyFor(typeof(NotFoundLoadStudyException))]
	[ExceptionPolicyFor(typeof(InUseLoadStudyException))]
	[ExceptionPolicyFor(typeof(OfflineLoadStudyException))]
	[ExceptionPolicyFor(typeof(NearlineLoadStudyException))]

	[ExceptionPolicyFor(typeof(StudyLoaderNotFoundException))]
	
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	internal sealed class LoadStudyExceptionHandlingPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LoadStudyExceptionHandlingPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			exceptionHandlingContext.Log(LogLevel.Error, e);

			if (e is LoadSopsException)
				Handle((LoadSopsException)e, exceptionHandlingContext);
			else if (e is StudyLoaderNotFoundException)
				Handle((StudyLoaderNotFoundException)e, exceptionHandlingContext);
		}

		private static void Handle(LoadSopsException e, IExceptionHandlingContext exceptionHandlingContext)
		{
			string message;
			if (e is InUseLoadStudyException)
			{
				message = SR.MessageStudyInUse;
			}
			else if (e is NearlineLoadStudyException)
			{
				message = SR.MessageStudyNearline;
			}
			else if (e is OfflineLoadStudyException)
			{
				message = SR.MessageStudyOffline;
			}
			else if (e is NotFoundLoadStudyException)
			{
				message = SR.MessageStudyNotFound;
			}
			else
			{
				if (e.PartiallyLoaded)
					message = String.Format(SR.MessageFormatLoadStudyIncomplete, e.SuccessfulImages, e.TotalImages);
				else
					message = SR.MessageLoadStudyCompleteFailure;

				message = String.Format("{0}\n{1}", message, SR.MessageContactPacsAdmin);
			}

			exceptionHandlingContext.ShowMessageBox(message);
		}

		private static void Handle(StudyLoaderNotFoundException e, IExceptionHandlingContext exceptionHandlingContext)
		{
			String message = String.Format("{0}\n{1}", SR.MessageLoadStudyCompleteFailure, SR.MessageContactPacsAdmin);
			exceptionHandlingContext.ShowMessageBox(message);
		}

		#endregion
	}

	#endregion

	#region Exception Classes

	#region Generic Study Loading Exceptions

	public class InUseLoadStudyException : LoadStudyException
	{
		public InUseLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		public InUseLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		public InUseLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		public InUseLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The study '{0}' is currently in use and cannot be loaded.", studyInstanceUid);
		}
	}

	public class NearlineLoadStudyException : LoadStudyException
	{
		public NearlineLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		public NearlineLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		public NearlineLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		public NearlineLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The study '{0}' is nearline and cannot be loaded.", studyInstanceUid);
		}
	}

	public class OfflineLoadStudyException : LoadStudyException
	{
		public OfflineLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		public OfflineLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		public OfflineLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		public OfflineLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The study '{0}' is offline and cannot be loaded.", studyInstanceUid);
		}
	}

	public class NotFoundLoadStudyException : LoadStudyException
	{
		public NotFoundLoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		public NotFoundLoadStudyException(string studyInstanceUid, string message)
			: base(studyInstanceUid, message)
		{
		}

		public NotFoundLoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		public NotFoundLoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: base(studyInstanceUid, message, innerException)
		{
		}

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("The specified study '{0}' was not found.", studyInstanceUid);
		}
	}

	public class LoadStudyException : LoadSopsException
	{
		public LoadStudyException(string studyInstanceUid)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid))
		{
		}

		public LoadStudyException(string studyInstanceUid, Exception innerException)
			: this(studyInstanceUid, FormatMessage(studyInstanceUid), innerException)
		{
		}

		public LoadStudyException(string studyInstanceUid, string message)
			: this(studyInstanceUid, 0, 0, message)
		{
		}

		public LoadStudyException(string studyInstanceUid, string message, Exception innerException)
			: this(studyInstanceUid, 0, 0, message, innerException)
		{
		}

		public LoadStudyException(string studyInstanceUid, int total, int failed)
			: this(studyInstanceUid, total, failed, FormatMessage(studyInstanceUid, total, failed))
		{
		}

		public LoadStudyException(string studyInstanceUid, int total, int failed, string message)
			: base(total, failed, message)
		{
			StudyInstanceUid = studyInstanceUid;
		}

		public LoadStudyException(string studyInstanceUid, int total, int failed, Exception innerException)
			: this(studyInstanceUid, total, failed, FormatMessage(studyInstanceUid, total, failed), innerException)
		{
		}

		public LoadStudyException(string studyInstanceUid, int total, int failed, string message, Exception innerException)
			: base(total, failed, message, innerException)
		{
			StudyInstanceUid = studyInstanceUid;
		}

		public readonly string StudyInstanceUid;

		private static string FormatMessage(string studyInstanceUid)
		{
			return String.Format("An error occurred while attempting to load study '{0}'.", studyInstanceUid);
		}

		private static string FormatMessage(string studyInstanceUid, int total, int failed)
		{
			return String.Format("{0} of {1} images failed to load for study '{2}'.", failed, total, studyInstanceUid);
		}
	}

	public class LoadSopsException : Exception
	{
		private readonly int _totalImages;
		private readonly int _failedImages;

		public LoadSopsException(int total, int failed)
			: this(total, failed, FormatMessage(total, failed))
		{
		}

		public LoadSopsException(int total, int failed, Exception innerException)
			: this(total, failed, FormatMessage(total, failed), innerException)
		{
		}

		public LoadSopsException(int total, int failed, string message)
			: this(message)
		{
			_totalImages = total;
			_failedImages = failed;
		}

		public LoadSopsException(int total, int failed, string message, Exception innerException)
			: this(message, innerException)
		{
			_totalImages = total;
			_failedImages = failed;
		}

		public LoadSopsException(string message)
			: base(message)
		{
		}

		public LoadSopsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Gets or sets the total number of images the Framework tried to load.
		/// </summary>
		public int TotalImages
		{
			get { return _totalImages; }
		}

		/// <summary>
		/// Gets or sets the number of images that failed to loaded.
		/// </summary>
		public int FailedImages
		{
			get { return _failedImages; }
		}

		/// <summary>
		/// Gets the number of images that loaded successfully.
		/// </summary>
		public int SuccessfulImages
		{
			get { return this.TotalImages - this.FailedImages; }
		}

		public bool AnyLoaded
		{
			get { return SuccessfulImages > 0; }
		}

		public bool PartiallyLoaded
		{
			get { return AnyLoaded && FailedImages > 0; }
		}

		private static string FormatMessage(int total, int failed)
		{
			return String.Format("{0} of {1} images have failed to load.", failed, total);
		}
	}

	#endregion
	#endregion
}
