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
using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public abstract class ExternalBase : External, IPresentationImageExternal, IDisplaySetExternal
	{
		protected abstract bool CanLaunch(IArgumentHintResolver hintResolver);
		protected abstract bool PerformLaunch(IArgumentHintResolver hintResolver);

		protected virtual IArgumentHintResolver BuildArgumentHints(IPresentationImage image)
		{
			List<IArgumentHint> hints = new List<IArgumentHint>();
			if (image is IImageSopProvider)
			{
				IImageSopProvider imageSopProvider = (IImageSopProvider) image;
				hints.Add(new SopArgumentHint(imageSopProvider.Sop));

				if (imageSopProvider.Sop.DataSource is ILocalSopDataSource)
				{
					ILocalSopDataSource localSopDataSource = (ILocalSopDataSource) imageSopProvider.Sop.DataSource;
					hints.Add(new FileArgumentHint(localSopDataSource.Filename));
				}
			}
			return new ArgumentHintResolver(hints);
		}

		protected virtual IArgumentHintResolver BuildArgumentHints(IEnumerable<IPresentationImage> images)
		{
			List<IArgumentHint> hints = new List<IArgumentHint>();
			if (images != null)
			{
				List<string> filenames = new List<string>();
				foreach (IPresentationImage image in images)
				{
					if (image is IImageSopProvider)
					{
						IImageSopProvider imageSopProvider = (IImageSopProvider) image;
						if (imageSopProvider.Sop.DataSource is ILocalSopDataSource)
						{
							ILocalSopDataSource localSopDataSource = (ILocalSopDataSource) imageSopProvider.Sop.DataSource;
							filenames.Add(localSopDataSource.Filename);
						}
					}
				}
				hints.Add(new FileSetArgumentHint(filenames));
			}
			return new ArgumentHintResolver(hints);
		}

		protected virtual IArgumentHintResolver BuildArgumentHints(IDisplaySet displaySet)
		{
			if (displaySet == null)
				return new ArgumentHintResolver();
			return BuildArgumentHints(displaySet.PresentationImages);
		}

		protected virtual bool HandleLaunchException(Exception ex)
		{
			return false;
		}

		private bool TryCanLaunch(IArgumentHintResolver hintResolver)
		{
			try
			{
				return this.CanLaunch(hintResolver);
			}
			finally
			{
				hintResolver.Dispose();
			}
		}

		private bool TryPerformLaunch(IArgumentHintResolver hintResolver)
		{
			try
			{
				return this.PerformLaunch(hintResolver);
			}
			catch (Exception ex)
			{
				// if the sub class doesn't handle the exception, throw it
				if (!HandleLaunchException(ex))
					throw;
				return false;
			}
			finally
			{
				hintResolver.Dispose();
			}
		}

		#region IPresentationImageExternal Members

		public bool CanLaunch(IPresentationImage image)
		{
			return TryCanLaunch(BuildArgumentHints(image));
		}

		public bool Launch(IPresentationImage image)
		{
			return TryPerformLaunch(BuildArgumentHints(image));
		}

		#endregion

		#region IDisplaySetExternal Members

		public bool CanLaunch(IEnumerable<IPresentationImage> images)
		{
			return TryCanLaunch(BuildArgumentHints(images));
		}

		public bool Launch(IEnumerable<IPresentationImage> images)
		{
			return TryPerformLaunch(BuildArgumentHints(images));
		}

		public bool CanLaunch(IDisplaySet displaySet)
		{
			return TryCanLaunch(BuildArgumentHints(displaySet));
		}

		public bool Launch(IDisplaySet displaySet)
		{
			return TryPerformLaunch(BuildArgumentHints(displaySet));
		}

		#endregion
	}
}