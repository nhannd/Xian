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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	// TODO (Stewart): Rename and rewrite so that the command remembers the graphic
	// that was removed upon undo; upon redo, the graphic is simply 
	// reinserted into the scene graph.

	public class PositionGraphicCommand : UndoableCommand
	{
		public enum CreateOperation { None, Create, Delete };

		private CreateOperation _createOperation;

		public PositionGraphicCommand(IGraphic graphic)
			: this(graphic, CreateOperation.None)
		{
		}

		public PositionGraphicCommand(
			IGraphic graphic, CreateOperation createOperation) 
			: base(graphic as IMemorable)
		{
			Platform.CheckForNullReference(graphic, "graphic");

			_createOperation = createOperation;
		}

		public override void Execute()
		{
			IGraphic graphic = base.Originator as IGraphic;

			if (_createOperation != CreateOperation.None)
				graphic.Visible = _createOperation == CreateOperation.Create;
			else
				base.Execute();

			graphic.Draw();
		}

		public override void Unexecute()
		{
			// If this command created the ruler, we simulate an undo not
			// by actually destroying the ruler, but by just making it invisible.
			// If we actually destroyed the ruler and removed it from the
			// layer hierarchy, then subsequently recreated
			// a new one through a redo operation, any commands in the command
			// history that depended on the old instance of the ruler would
			// be useless, since that ruler is no longer part of the layer hiearchy,
			// and thus would never be rendered
			IGraphic graphic = base.Originator as IGraphic;

			if (_createOperation != CreateOperation.None)
				graphic.Visible = _createOperation == CreateOperation.Delete;
			else
				base.Unexecute();

			graphic.Draw();
		}
	}
}
