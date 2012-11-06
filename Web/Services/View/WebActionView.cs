#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Web.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common.Messages;

namespace ClearCanvas.Web.Services.View
{
    public interface IWebActionView : IWebView, IActionView
    {
    }

    public abstract class WebActionView : WebView, IWebActionView
	{
        #region Implementation of IActionView

        IActionViewContext IActionView.Context { get; set; }

        #endregion
        
        public abstract bool IsEquivalentTo(WebActionView other);

		public virtual void Update()
		{ }

		public static WebActionView Create(ActionModelNode modelNode)
		{
			if (modelNode is ActionNode)
			{
				IAction action = ((ActionNode)modelNode).Action;
				if (action is DropDownButtonAction)
				{
					WebActionView view = new DropDownButtonActionView();
                    view.SetModelObject(action);
                    return view;
				}
				if (action is DropDownAction)
				{
					WebActionView view = new DropDownActionView();
					view.SetModelObject(action);
					return view;
				}
				if (action is IClickAction)
				{
					WebActionView view = new ClickActionView();
					view.SetModelObject(action);
					return view;
				}

			    try
			    {
                    var view = (WebActionView)ViewFactory.CreateAssociatedView(action.GetType());
                    view.SetModelObject(action);
			        return view;
			    }
			    catch (Exception)
			    {
			        Platform.Log(LogLevel.Debug, "Failed to create associated view for '{0}'", action.GetType());   
			    }
			}
			else if (modelNode.ChildNodes.Count > 0)
			{
				IWebView view = new BranchActionView();
				view.SetModelObject(modelNode);
				return (WebActionView)view;
			}

			//TODO (CR May 2010): although we won't get here, if we did, we should throw
			return null;
		}

		public static List<WebActionView> Create(ActionModelNodeList nodes)
		{
			var views = new List<WebActionView>();
			foreach (ActionModelNode node in nodes)
			{
				//TODO (CR May 2010): remove the try/catch and let it crash?
				try
				{
					WebActionView view = Create(node);
					if (view != null)
						views.Add(view);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, 
					             "Unexpected exception processing action node: {0}", node);
				}
			}

			return views;
		}

		public static bool AreEquivalent(IList<WebActionView> views1, IList<WebActionView> views2)
		{
			if (views1 == null && views2 == null)
				return true;
			if (views1 == null || views2 == null)
				return false;

			if (views1.Count != views2.Count)
				return false;

			for (int i = 0; i < views1.Count; ++i)
			{
				if (!views1[i].IsEquivalentTo(views2[i]))
					return false;
			}

			return true;
		}
	}

	public class BranchActionView : WebActionView
	{
		protected ActionModelNode ActionModelNode { get; private set; }
		private List<WebActionView> ChildViews { get; set; }

		public override bool IsEquivalentTo(WebActionView other)
		{
			if (other.GetType() != GetType())
				return false;

			return AreEquivalent(ChildViews, ((BranchActionView)other).ChildViews);
		}

		protected override Entity CreateEntity()
		{
			return new WebActionNode();
		}

		public override void SetModelObject(object modelObject)
		{
			ActionModelNode = (ActionModelNode)modelObject;
			ChildViews = Create(ActionModelNode.ChildNodes);
		}

        protected override void Initialize()
        {
        }

		protected override void UpdateEntity(Entity entity)
		{
			var webAction = (WebActionNode)entity;
			webAction.LocalizedText = ActionModelNode.PathSegment.LocalizedText;
			webAction.Children = ChildViews.Select(view => (WebActionNode)view.GetEntity()).ToArray();
		}

        public override void Update()
        {
            if (ChildViews == null)
                return;

            foreach (var childView in ChildViews)
                childView.Update();
        }

		public override void ProcessMessage(Message message)
		{
		}

		private void DisposeChildren()
		{
			if (ChildViews == null)
				return;

			foreach (WebActionView child in ChildViews)
				child.Dispose();

			ChildViews.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;

			DisposeChildren();
		}
	}

	public abstract class StandardWebActionView : WebActionView
	{
		protected StandardWebActionView()
		{
		}

		protected IAction Action { get; private set; }

		public override bool IsEquivalentTo(WebActionView other)
		{
			if (other.GetType() != GetType())
				return false;

			return Action.ActionID == ((StandardWebActionView)other).Action.ActionID;
		}

		public override void SetModelObject(object modelObject)
		{
			Action = (IAction)modelObject;
            ((IActionView)this).Context = new ActionViewContext(Action);
		}

		protected override void Initialize()
		{
			Action.VisibleChanged += OnVisibleChanged;
			Action.EnabledChanged += OnEnabledChanged;
			Action.TooltipChanged += OnTooltipChanged;
			Action.LabelChanged += OnLabelChanged;
			Action.IconSetChanged += OnIconSetChanged;
		}

		protected override void UpdateEntity(Entity entity)
		{
			WebAction webAction = (WebAction)entity;
			webAction.ToolTip = Action.Tooltip;
			webAction.Label = Action.Label;
			webAction.Enabled = Action.Enabled;
			webAction.Visible = Action.Visible;
		    webAction.Available = Action.Available;

			if (Action.IconSet == null)
				return;

			webAction.IconSet = CreateWebIconSet(Action);
		}

		protected static WebIconSet CreateWebIconSet(IAction action)
		{
			return new WebIconSet
			       	{
			       		LargeIcon = LoadIcon(action, IconSize.Large),
			       		SmallIcon = LoadIcon(action, IconSize.Small),
			       		MediumIcon = LoadIcon(action, IconSize.Medium),
						//HasOverlay = IconsHaveOverlay(action.IconSet)
			       	};
		}

        //protected static bool IconsHaveOverlay(IconSet iconSet)
        //{
        //    return iconSet is MouseButtonIconSet && ((MouseButtonIconSet)iconSet).ShowMouseButtonIconOverlay;
        //}

		protected static string LoadIcon(IAction action, IconSize size)
		{
			var image = action.IconSet.CreateIcon(size, action.ResourceResolver);
			using (var theStream = new MemoryStream())
			{
				image.Save(theStream, ImageFormat.Png);
				theStream.Position = 0;
				return Convert.ToBase64String(theStream.GetBuffer());
			}
		}

		private void OnVisibleChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Visible", Action.Visible);
		}

		private void OnEnabledChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Enabled", Action.Enabled);
		}

		private void OnTooltipChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("ToolTip", Action.Tooltip);
		}

		private void OnLabelChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Label", Action.Label);
		}

		private void OnIconSetChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("IconSet", CreateWebIconSet(Action));
		}

        protected override string[] GetDebugInfo()
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine(string.Format("Action ID : {0}", Action.ActionID));
            info.AppendLine(string.Format("Action Label : {0}", Action.Label));
            info.AppendLine(string.Format("Action Path : {0}", Action.Path));
            info.AppendLine(string.Format("Action Availablity : {0}", Action.Available)); 
            
            return new[] {info.ToString()};
        }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;
			Action.VisibleChanged -= OnVisibleChanged;
			Action.EnabledChanged -= OnEnabledChanged;
			Action.TooltipChanged -= OnTooltipChanged;
			Action.LabelChanged -= OnLabelChanged;
			Action.IconSetChanged -= OnIconSetChanged;
		}
	}

	public class ClickActionView : StandardWebActionView
	{
		public new IClickAction Action
		{
			get { return (IClickAction)base.Action; }	
		}

		protected override Entity CreateEntity()
		{
			return new WebClickAction();
		}

        protected override void Initialize()
		{
            base.Initialize();
			Action.CheckedChanged += OnCheckChanged;
		}

		protected override void UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);
			WebClickAction webClickAction = (WebClickAction)entity;

			webClickAction.IsCheckAction = Action.IsCheckAction;
			webClickAction.Checked = Action.Checked;
		}

	    public override void ProcessMessage(Message message)
		{
			if (message is ActionClickedMessage)
				Action.Click();
		}

		private void OnCheckChanged(object sender, EventArgs e)
		{
			NotifyEntityPropertyChanged("Checked", Action.Checked);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
				return;

			Action.CheckedChanged -= OnCheckChanged;
		}
	}

	//TODO: the 2 drop-down views share pretty much exactly the same code.  Try to share it.
	public class DropDownButtonActionView : ClickActionView
	{
		private List<WebActionView> ChildViews { get; set; }

		public new DropDownButtonAction Action
		{
			get { return (DropDownButtonAction)base.Action; }
		}

		public override bool IsEquivalentTo(WebActionView other)
		{
			return base.IsEquivalentTo(other) && 
			       AreEquivalent(ChildViews, ((DropDownButtonActionView)other).ChildViews);
		}

		protected override Entity CreateEntity()
		{
			return new WebDropDownButtonAction();
		}

        protected override void Initialize()
		{
            base.Initialize();
			ChildViews = Create(Action.DropDownMenuModel.ChildNodes);
		}

		protected override void UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);
			((WebDropDownButtonAction)entity).DropDownActions = GetDropDownWebActions();
		}

		public override void Update()
		{
			var newChildren = Create(Action.DropDownMenuModel.ChildNodes);
			if (!AreEquivalent(ChildViews, newChildren))
			{
				DisposeChildren();
				ChildViews = newChildren;
				NotifyEntityPropertyChanged("DropDownActions", GetDropDownWebActions());
			}
			else
			{
				foreach (var newChild in newChildren)
					newChild.Dispose();
			}
		}

		private WebActionNode[] GetDropDownWebActions()
		{
			return ChildViews.Select(view => (WebActionNode)view.GetEntity()).ToArray();
		}

		private void DisposeChildren()
		{
			if (ChildViews == null)
				return;

			foreach (StandardWebActionView childView in ChildViews)
				childView.Dispose();

			ChildViews.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;

			DisposeChildren();
		}
	}

	public class DropDownActionView : StandardWebActionView
	{
		private List<WebActionView> ChildViews { get; set; }

		public new DropDownAction Action
		{
			get { return (DropDownAction)base.Action; }
		}

		public override bool IsEquivalentTo(WebActionView other)
		{
			return base.IsEquivalentTo(other) && 
			       AreEquivalent(ChildViews, ((DropDownActionView)other).ChildViews);
		}

		public override void ProcessMessage(Message message)
		{
			// There should be no messages received.
			throw new NotImplementedException();
		}

		protected override Entity CreateEntity()
		{
			return new WebDropDownAction();
		}

        protected override void Initialize()
		{
            base.Initialize();
			ChildViews = Create(Action.DropDownMenuModel.ChildNodes);
		}

		protected override void UpdateEntity(Entity entity)
		{
			base.UpdateEntity(entity);
			((WebDropDownAction)entity).DropDownActions = GetDropDownWebActions();
		}

		public override void Update()
		{
			var newChildren = Create(Action.DropDownMenuModel.ChildNodes);
			if (!AreEquivalent(ChildViews, newChildren))
			{
				DisposeChildren();
				ChildViews = newChildren;
				NotifyEntityPropertyChanged("DropDownActions", GetDropDownWebActions());
			}
			else
			{
				foreach (var newChild in newChildren)
					newChild.Dispose();
			}
		}

		private WebActionNode[] GetDropDownWebActions()
		{
            return ChildViews.Select(view => (WebActionNode)view.GetEntity()).ToArray();
		}

		private void DisposeChildren()
		{
			if (ChildViews == null)
				return;

			foreach (StandardWebActionView childView in ChildViews)
				childView.Dispose();

			ChildViews.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing) return;

			DisposeChildren();
		}
	}
}
