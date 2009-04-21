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
	[Cloneable]
	public class RoiCalloutGraphic : CalloutGraphic
	{
		private bool _showAnalysis = true;

		[CloneIgnore]
		private readonly List<IRoiAnalyzer> _roiAnalyzers = new List<IRoiAnalyzer>();

		public RoiCalloutGraphic() : base()
		{
			_roiAnalyzers.AddRange(RoiAnalyzerExtensionPoint.RoiAnalyzers);
			_showAnalysis = RoiSettings.Default.ShowAnalysisByDefault;
		}

		protected RoiCalloutGraphic(RoiCalloutGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
			_roiAnalyzers.AddRange(source._roiAnalyzers);
		}

		public new RoiGraphic ParentGraphic
		{
			get { return base.ParentGraphic as RoiGraphic; }
		}

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

		public IList<IRoiAnalyzer> RoiAnalyzers
		{
			get { return _roiAnalyzers; }
		}

		protected override IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
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
			IActionSet other = base.OnGetExportedActions(site, mouseInformation);
			if (other != null)
				actions = actions.Union(other);

			return actions;
		}

		public void Rename()
		{
			RoiGraphic parent = this.ParentGraphic;
			if (parent == null)
				return;

			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				EditBox editBox = new EditBox(parent.Name ?? string.Empty);
				editBox.Location = Point.Round(base.TextGraphic.AnchorPoint);
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

		public void Update()
		{
			RoiGraphic roiGraphic = this.ParentGraphic;
			this.Update(roiGraphic.CreateRoiInformation(), RoiAnalysisMode.Normal);
		}

		public void Update(Roi roi)
		{
			this.Update(roi, RoiAnalysisMode.Normal);
		}

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

		protected override void OnTextChanged(EventArgs e)
		{
			base.Visible = !(string.IsNullOrEmpty(base.Text));
			base.OnTextChanged(e);
		}
	}
}