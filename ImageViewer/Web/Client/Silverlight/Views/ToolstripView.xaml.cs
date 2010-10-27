#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Actions;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class ToolstripView : UserControl
    {
        private Dictionary<Guid, IToolstripButton> _buttonLookup = new Dictionary<Guid, IToolstripButton>();
        private ActionDispatcher _dispatcher;
        private ServerEventDispatcher _eventDispatcher;
        WebIconSize _desiredIconSize = WebIconSize.Medium;

        public ServerEventDispatcher EventDispatcher
        {
            set
            {
                _eventDispatcher = value;
                _dispatcher = new ActionDispatcher(_eventDispatcher);
            }
        }

        public ToolstripView()
        {
            InitializeComponent();
            System.Windows.Application.Current.Host.Content.Resized += OnApplicationResized;

            EventBroker.TileHasCaptureChanged += new EventHandler(EventBroker_TileHasCapture);
        }

        public void OnLoseFocus(object sender, EventArgs e)
        {
             foreach (IToolstripButton stripButton in _buttonLookup.Values)
            {
                IToolstripDropdownButton dropButton = stripButton as IToolstripDropdownButton;
                if (dropButton == null) continue;

                if (dropButton.IsVisible)
                    dropButton.Hide();
            }
        }

        private void OnApplicationResized(object sender, EventArgs e)
        {
            if (Height != LayoutRoot.DesiredSize.Height && LayoutRoot.DesiredSize.Height > 0)
            {
                Height = LayoutRoot.DesiredSize.Height;
                UpdateLayout();
            }
            if (Height != LayoutRoot.ActualHeight && LayoutRoot.ActualHeight > 0)
            {
                Height = LayoutRoot.ActualHeight;
                UpdateLayout();
            }
        }

        public void SetActionModel(IEnumerable<WebActionNode> actionModel)
        {
            LayoutRoot.Children.Clear();

			foreach (WebActionNode action in actionModel)
            {
				//TODO: what if there are children?
				if (action is WebDropDownButtonAction)
                {
                    DropDownButton theButton = new DropDownButton(_dispatcher, action as WebDropDownButtonAction,_desiredIconSize);

					_buttonLookup.Add(action.Identifier, theButton);
                    theButton.RegisterOnMouseEnter(OnMouseEnter);
                    theButton.RegisterOnMouseLeave(OnMouseLeave);

                    LayoutRoot.Children.Add(theButton);
                }
                else if (action is WebDropDownAction)
                {

                    LayoutDropDown theButton = new LayoutDropDown(_dispatcher, action as WebDropDownAction,_desiredIconSize);

                    _buttonLookup.Add(action.Identifier, theButton);
                    theButton.RegisterOnMouseEnter(OnMouseEnter);
                    theButton.RegisterOnMouseLeave(OnMouseLeave);

                    LayoutRoot.Children.Add(theButton);
                }
                else
                {
                    StandardButton theButton = new StandardButton(_dispatcher, action as WebClickAction, _desiredIconSize);

                    _buttonLookup.Add(action.Identifier, theButton);
                    theButton.RegisterOnMouseEnter(OnMouseEnter);
                    theButton.RegisterOnMouseLeave(OnMouseLeave);

                    LayoutRoot.Children.Add(theButton);
                }
            }

            OnActionModelChanged();
        }

        private void OnActionModelChanged()
        {
            AddHelpButton();

            UpdateLayout();
            Height = LayoutRoot.ActualHeight;
            UpdateLayout();
        }

        private void AddHelpButton()
        {
            HelpButton theButton = new HelpButton();
            theButton.SetIconSize(_desiredIconSize);
            LayoutRoot.Children.Add(theButton);
            theButton.RegisterOnMouseEnter(OnMouseEnter);
            theButton.RegisterOnMouseLeave(OnMouseLeave);
        }
        
        void EventBroker_TileHasCapture(object sender, EventArgs e)
        {
            Tile tile = sender as Tile;
            LayoutRoot.IsHitTestVisible = !tile.HasCapture;
        }

        

		private void OnMouseEnter(IToolstripButton button)
        {
            foreach (IToolstripButton stripButton in _buttonLookup.Values)
            {
                if (stripButton == button) continue;

                IToolstripDropdownButton dropButton = stripButton as IToolstripDropdownButton;
                if (dropButton == null) continue;

                if (dropButton.IsVisible)
                    dropButton.Hide();
            }
        }

        private void OnMouseLeave(IToolstripButton button)
        {
            
        }

        internal void SetIconSize(WebIconSize webIconSize)
        {
            _desiredIconSize = webIconSize;
            foreach (IToolstripButton stripButton in _buttonLookup.Values)
            {
                stripButton.SetIconSize(_desiredIconSize);
            }
        }
    }
}