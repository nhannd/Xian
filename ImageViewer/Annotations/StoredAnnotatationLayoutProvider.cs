#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	internal abstract class StoredAnnotatationLayoutProvider : AnnotationLayoutProvider
	{
		protected StoredAnnotatationLayoutProvider()
		{
		}

		protected abstract string StoredLayoutId { get; }

		#region IAnnotationLayoutProvider Members

		public override IAnnotationLayout  AnnotationLayout
		{
			get
			{
				IAnnotationLayout returnLayout;

				try
				{
					returnLayout = AnnotationLayoutStore.Instance.GetLayout(this.StoredLayoutId, this.AvailableAnnotationItems);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);

					StoredAnnotationLayout layout = new StoredAnnotationLayout("error");
					layout.AnnotationBoxGroups.Add(new StoredAnnotationBoxGroup("errorgroup"));
					IAnnotationItem item = new BasicTextAnnotationItem("errorbox", "errorbox", SR.LabelError, SR.MessageErrorLoadingAnnotationLayout);

					AnnotationBox box = new AnnotationBox(new RectangleF(0.5F,0.90F, 0.5F, 0.10F), item);
					box.Bold = true;
					box.Color = "Red";
					box.Justification = AnnotationBox.JustificationBehaviour.Far;
					box.NumberOfLines = 5;
					box.VerticalAlignment = AnnotationBox.VerticalAlignmentBehaviour.Bottom;

					layout.AnnotationBoxGroups[0].AnnotationBoxes.Add(box);
					returnLayout = layout;
				}

				return returnLayout;
			}
		}

		#endregion
	}
}
