#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    public abstract class ZoomFixedTool : ImageViewerTool
    {
    	private readonly SpatialTransformImageOperation _operation;
		private float _selectedScale;

        public ZoomFixedTool()
		{
			_operation = new SpatialTransformImageOperation(Apply);
		}

		public abstract void Activate();

        protected void ApplyZoom(float scale)
        {
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

        	_selectedScale = scale;

			ImageOperationApplicator applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
            UndoableCommand command = new UndoableCommand(applicator);
            command.Name = SR.CommandZoom;
            command.BeginState = applicator.CreateMemento();

        	applicator.ApplyToAllImages();

			command.EndState = applicator.CreateMemento();
			if (!command.EndState.Equals(command.BeginState))
				this.Context.Viewer.CommandHistory.AddCommand(command);
        }

		public void Apply(IPresentationImage image)
		{
			ISpatialTransform transform = (ISpatialTransform)_operation.GetOriginator(image);
			transform.Scale = _selectedScale;
		}
   }
}
