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
using System.Drawing;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;
using CoordinateSystem = ClearCanvas.ImageViewer.Graphics.CoordinateSystem;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// A specialized callout graphic used by <see cref="RoiGraphic"/> whose text content is automatically computed by a list of <see cref="IRoiAnalyzer"/>s.
	/// </summary>
	[Cloneable]
	public class RoiCalloutGraphic : CalloutGraphic
	{
		private bool _showAnalysis = true;

		[CloneIgnore]
		private readonly List<IRoiAnalyzer> _roiAnalyzers = new List<IRoiAnalyzer>();

		/// <summary>
		/// Constructs a new instance of <see cref="RoiCalloutGraphic"/> with all <see cref="IRoiAnalyzer"/>s extending <see cref="RoiAnalyzerExtensionPoint"/>.
		/// </summary>
		public RoiCalloutGraphic()
			: this(RoiAnalyzerExtensionPoint.CreateRoiAnalyzers()) {}

		/// <summary>
		/// Constructs a new instance of <see cref="RoiCalloutGraphic"/> witht the specified <see cref="IRoiAnalyzer"/>s.
		/// </summary>
		/// <param name="analyzers"></param>
		public RoiCalloutGraphic(IEnumerable<IRoiAnalyzer> analyzers)
			: base()
		{
			_roiAnalyzers.AddRange(analyzers);
			_showAnalysis = RoiSettings.Default.ShowAnalysisByDefault;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected RoiCalloutGraphic(RoiCalloutGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
			_roiAnalyzers.AddRange(source._roiAnalyzers);
		}

		/// <summary>
		/// Gets the parent <see cref="RoiGraphic"/>.
		/// </summary>
		public new RoiGraphic ParentGraphic
		{
			get { return base.ParentGraphic as RoiGraphic; }
		}

		/// <summary>
		/// Gets or sets a value indicating if any analysis should be displayed.
		/// </summary>
		public bool ShowAnalysis
		{
			get { return _showAnalysis; }
			set
			{
				if (_showAnalysis != value)
				{
					_showAnalysis = value;
					this.Update();
				}
			}
		}

		/// <summary>
		/// Gets the list of <see cref="IRoiAnalyzer"/>s that determines the text of the callout.
		/// </summary>
		public IList<IRoiAnalyzer> RoiAnalyzers
		{
			get { return _roiAnalyzers; }
		}

		/// <summary>
		/// Gets a set of exported <see cref="IAction"/>s.
		/// </summary>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		public override IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			IResourceResolver resolver = new ResourceResolver(this.GetType(), true);
			string @namespace = typeof(RoiCalloutGraphic).FullName;

			MenuAction hideAction = new MenuAction(@namespace + ":toggle", new ActionPath(site + "/MenuShowAnalysis", resolver), ClickActionFlags.None, resolver);
			hideAction.GroupHint = new GroupHint("Tools.Measurements.Display");
			hideAction.Label = SR.MenuShowAnalysis;
			hideAction.Checked = this.ShowAnalysis;
			hideAction.Persistent = true;
			hideAction.SetClickHandler(this.ToggleShowAnalysis);

			MenuAction renameAction = new MenuAction(@namespace + ":rename", new ActionPath(site + "/MenuRename", resolver), ClickActionFlags.None, resolver);
			renameAction.GroupHint = new GroupHint("Tools.Measurements.Properties");
			renameAction.Label = SR.MenuRename;
			renameAction.Persistent = true;
			renameAction.SetClickHandler(this.Rename);

			IActionSet actions = new ActionSet(new IAction[] {hideAction, renameAction});
			IActionSet other = base.GetExportedActions(site, mouseInformation);
			if (other != null)
				actions = actions.Union(other);

			return actions;
		}

		/// <summary>
		/// Invokes an interactive edit box on the name of the graphic, allowing the user to assign a name to the <see cref="RoiGraphic"/>.
		/// </summary>
		public void Rename()
		{
			RoiGraphic parent = this.ParentGraphic;
			if (parent == null)
				return;

			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				EditBox editBox = new EditBox(parent.Name ?? string.Empty);
				editBox.Location = Point.Round(base.TextGraphic.Location);
				editBox.Size = Size.Round(base.TextGraphic.BoundingBox.Size);
				editBox.FontName = base.TextGraphic.Font;
				editBox.FontSize = base.TextGraphic.SizeInPoints;
				editBox.Multiline = false;
				editBox.ValueAccepted += OnEditBoxAccepted;
				editBox.ValueCancelled += OnEditBoxCancelled;
				base.ParentPresentationImage.Tile.EditBox = editBox;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		private void ToggleShowAnalysis()
		{
			this.ShowAnalysis = !_showAnalysis;

			RoiGraphic parent = this.ParentGraphic;
			if (parent != null)
			{
				if (_showAnalysis)
					parent.Refresh();
				else 
					parent.Draw();
			}
		}

		private void OnEditBoxCancelled(object sender, EventArgs e)
		{
			EditBox editBox = (EditBox) sender;
			editBox.ValueAccepted -= OnEditBoxAccepted;
			editBox.ValueCancelled -= OnEditBoxCancelled;
			if(base.ParentPresentationImage != null)
				base.ParentPresentationImage.Tile.EditBox = null;
		}

		private void OnEditBoxAccepted(object sender, EventArgs e)
		{
			EditBox editBox = (EditBox) sender;
			editBox.ValueAccepted -= OnEditBoxAccepted;
			editBox.ValueCancelled -= OnEditBoxCancelled;
			if (base.ParentPresentationImage != null)
				base.ParentPresentationImage.Tile.EditBox = null;

			RoiGraphic parent = base.ParentGraphic as RoiGraphic;
			if (parent != null)
			{
				parent.Name = editBox.Value;
				this.Update();
				this.Draw();
			}
		}

		/// <summary>
		/// Forces the callout to update, allowing all the analyzers to recompute and update the text content of the callout immediately.
		/// </summary>
		public void Update()
		{
			RoiGraphic roiGraphic = this.ParentGraphic;
			this.Update(roiGraphic.GetRoi(), RoiAnalysisMode.Normal);
		}

		/// <summary>
		/// Forces the callout to update, allowing all the analyzers to recompute and update the text content of the callout immediately.
		/// </summary>
		/// <param name="mode">A value indicating whether or not the current region of interest is in the state of changing, and therefore whether or not analyzers should skip expensive computations.</param>
		public void Update(RoiAnalysisMode mode)
		{
			RoiGraphic roiGraphic = this.ParentGraphic;
			this.Update(roiGraphic.GetRoi(), mode);
		}

		/// <summary>
		/// Forces the callout to update, allowing all the analyzers to recompute and update the text content of the callout immediately.
		/// </summary>
		/// <param name="roi">A particular region of interest information object to use when computing statistics.</param>
		public void Update(Roi roi)
		{
			this.Update(roi, RoiAnalysisMode.Normal);
		}

		/// <summary>
		/// Forces the callout to update, allowing all the analyzers to recompute and update the text content of the callout.
		/// </summary>
		/// <param name="roi">A particular region of interest information object to use when computing statistics.</param>
		/// <param name="mode">A value indicating whether or not the current region of interest is in the state of changing, and therefore whether or not analyzers should skip expensive computations.</param>
		public void Update(Roi roi, RoiAnalysisMode mode)
		{
			if (this.ImageViewer == null)
			{
				return;
			}

			StringBuilder builder = new StringBuilder();
			RoiGraphic parent = this.ParentGraphic;
			if (parent != null && !string.IsNullOrEmpty(parent.Name))
				builder.AppendLine(parent.Name);

			if (_showAnalysis && _roiAnalyzers.Count > 0 && roi != null)
			{
				try
				{
					foreach (IRoiAnalyzer analyzer in _roiAnalyzers)
					{
						if (analyzer.SupportsRoi(roi))
						{
							string analysis = analyzer.Analyze(roi, mode);
							if (!string.IsNullOrEmpty(analysis))
								builder.AppendLine(analysis);
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					builder.AppendLine(SR.MessageRoiAnalysisError);
				}
			}

			base.Text = builder.ToString().Trim();
		}

		/// <summary>
		/// Called when the value of <see cref="CalloutGraphic.Text"/> changes.
		/// </summary>
		protected override void OnTextChanged()
		{
			base.Visible = !(string.IsNullOrEmpty(base.Text));
			base.OnTextChanged();
		}
	}
}