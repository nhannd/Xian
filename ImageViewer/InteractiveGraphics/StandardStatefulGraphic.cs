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
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A <see cref="StatefulCompositeGraphic"/> that is <see cref="ISelectableGraphic">selectable</see> and
	/// <see cref="IFocussableGraphic">focusable</see>.
	/// </summary>
	/// <remarks>
	/// Factory methods can be overridden so that customized graphic states
	/// can be created.
	/// </remarks>
	[Cloneable]
	public class StandardStatefulGraphic : StatefulCompositeGraphic, IStandardStatefulGraphic, ISelectableGraphic, IFocussableGraphic
	{
		protected static readonly Color DefaultFocusColor = Color.Orange;
		protected static readonly Color DefaultFocusSelectedColor = Color.Tomato;
		protected static readonly Color DefaultSelectedColor = Color.Tomato;
		protected static readonly Color DefaultInactiveColor = Color.Yellow;

		[CloneIgnore]
		private bool _selected = false;

		[CloneIgnore]
		private bool _focused = false;

		[CloneCopyReference]
		private Color _focusColor = DefaultFocusColor;

		[CloneCopyReference]
		private Color _focusSelectedColor = DefaultFocusSelectedColor;

		[CloneCopyReference]
		private Color _selectedColor = DefaultSelectedColor;

		[CloneCopyReference]
		private Color _inactiveColor = DefaultInactiveColor;

		/// <summary>
		/// Constructs a new instance of <see cref="StandardStatefulGraphic"/>.
		/// </summary>
		public StandardStatefulGraphic(IGraphic subject) : base(subject)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected StandardStatefulGraphic(StandardStatefulGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			if (this.State == null)
				this.State = this.CreateInactiveState();
		}

		public Color FocusColor
		{
			get { return _focusColor; }
			set { _focusColor = value; }
		}

		public Color SelectedColor
		{
			get { return _selectedColor; }
			set { _selectedColor = value; }
		}

		public Color FocusSelectedColor
		{
			get { return _focusSelectedColor; }
			set { _focusSelectedColor = value; }
		}

		public Color InactiveColor
		{
			get { return _inactiveColor; }
			set { _inactiveColor = value; }
		}

		private static void UpdateGraphicStyle(IGraphic graphic, Color color, bool controlGraphics)
		{
			if (graphic is IControlGraphic)
			{
				IControlGraphic controlGraphic = (IControlGraphic) graphic;
				controlGraphic.Show = controlGraphics;
				controlGraphic.Color = color;
			}
			else if (graphic is IVectorGraphic)
			{
				((IVectorGraphic) graphic).Color = color;
			}
			
			if (graphic is CompositeGraphic)
			{
				foreach (IGraphic childGraphic in ((CompositeGraphic) graphic).Graphics)
					UpdateGraphicStyle(childGraphic, color, controlGraphics);
			}
		}

		protected override void OnStateInitialized()
		{
			base.OnStateInitialized();

			if (this.State is InactiveGraphicState)
				UpdateGraphicStyle(this, this.InactiveColor, false);
			else if ( this.State is FocussedGraphicState)
				UpdateGraphicStyle(this, this.FocusColor, true);
			else if (this.State is SelectedGraphicState)
				UpdateGraphicStyle(this, this.SelectedColor, false);
			else if (this.State is FocussedSelectedGraphicState)
				UpdateGraphicStyle(this, this.FocusSelectedColor, true);
		}

		protected override void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			base.OnStateChanged(e);

			if (typeof (InactiveGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterInactiveState(e.MouseInformation);
			else if (typeof (FocussedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterFocusState(e.MouseInformation);
			else if (typeof (SelectedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterSelectedState(e.MouseInformation);
			else if (typeof (FocussedSelectedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				OnEnterFocusSelectedState(e.MouseInformation);
		}

		protected virtual void OnEnterInactiveState(IMouseInformation mouseInformation)
		{
			// If the currently selected graphic is this one,
			// and we're about to go inactive, set the selected graphic
			// to null, indicating that no graphic is currently selected
			if (this.ParentPresentationImage != null)
			{
				if (this.ParentPresentationImage.SelectedGraphic == this)
					this.ParentPresentationImage.SelectedGraphic = null;

				if (this.ParentPresentationImage.FocussedGraphic == this)
					this.ParentPresentationImage.FocussedGraphic = null;
			}

			UpdateGraphicStyle(this, this.InactiveColor, false);
			Draw();
			//Trace.Write("EnterInactiveState\n");
		}

		protected virtual void OnEnterFocusState(IMouseInformation mouseInformation)
		{
			this.Focussed = true;

			UpdateGraphicStyle(this, this.FocusColor, true);
			Draw();
			//Trace.Write("EnterFocusState\n");
		}

		protected virtual void OnEnterSelectedState(IMouseInformation mouseInformation)
		{
			this.Selected = true;

			if (this.ParentPresentationImage != null && this.ParentPresentationImage.FocussedGraphic == this)
				this.ParentPresentationImage.FocussedGraphic = null;

			UpdateGraphicStyle(this, this.SelectedColor, false);
			Draw();
			//Trace.Write("EnterSelectedState\n");
		}

		protected virtual void OnEnterFocusSelectedState(IMouseInformation mouseInformation)
		{
			this.Selected = true;
			this.Focussed = true;

			UpdateGraphicStyle(this, this.FocusSelectedColor, true);
			Draw();
			//Trace.Write("EnterFocusSelectedState\n");
		}

		#region IStandardStatefulGraphic Members

		/// <summary>
		/// Creates a new instance of <see cref="InactiveGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="FocussedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="SelectedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedGraphicState(this);
		}

		#endregion

		#region ISelectable Members

		/// <summary>
		/// Gets or set a value indicating whether the <see cref="StandardStatefulGraphic"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected && this.ParentPresentationImage != null)
						this.ParentPresentationImage.SelectedGraphic = this;

					if (_focused)
					{
						if (_selected)
							this.State = CreateFocussedSelectedState();
						else
							this.State = CreateFocussedState();
					}
					else
					{
						if (_selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		#endregion

		#region IFocussable Members

		/// <summary>
		/// Gets or set a value indicating whether the <see cref="StandardStatefulGraphic"/> is in focus.
		/// </summary>
		public bool Focussed
		{
			get { return _focused; }
			set
			{
				if (_focused != value)
				{
					_focused = value;

					if (_focused)
					{
						if (this.ParentPresentationImage != null)
							this.ParentPresentationImage.FocussedGraphic = this;

						if (this.Selected)
							this.State = CreateFocussedSelectedState();
						else
							this.State = CreateFocussedState();
					}
					else
					{
						if (this.Selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		#endregion
	}
}