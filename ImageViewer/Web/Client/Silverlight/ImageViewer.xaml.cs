#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Views;
using System.Windows.Browser;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Interop;
using System.Windows.Input;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls.Primitives;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
	public partial class ImageViewer : UserControl, IDisposable
	{
		private volatile ViewerApplication _serverApplication;

		private StudyView _studyView;
		private volatile ApplicationContext _context;

        private bool _shuttingDown;

        private event EventHandler Shuttingdown;

        bool _suppressError;

        public ImageViewer(StartViewerApplicationRequest startRequest)
        {
            InitializeComponent();
            
			_context = ApplicationContext.Current;

            ErrorHandler.OnCriticalError += new EventHandler(ErrorHandler_OnCriticalError);

			ApplicationContext.Current.ServerEventBroker.RegisterEventHandler(typeof(ApplicationStartedEvent), ApplicationStarted);
            ApplicationContext.Current.ServerEventBroker.RegisterEventHandler(typeof(SessionUpdatedEvent), OnSessionUpdated);
            ApplicationContext.Current.ServerEventBroker.RegisterEventHandler(typeof(MessageBoxShownEvent), OnMessageBox);
            ApplicationContext.Current.ServerEventBroker.ServerApplicationStopped += OnServerApplicationStopped;
            
            _studyView = new StudyView();
			StudyViewContainer.Children.Add(_studyView);
            MouseHelper.SetBackgroundElement(LayoutRoot);

            if (startRequest == null)
            {
                //TODO: replace this with the custom dialog. For some reason, it doesn't work here.
                System.Windows.MessageBox.Show("Unable to start the viewer: parameters are missing");
            }
            else
            {
                
                ToolstripViewComponent.EventDispatcher = ApplicationContext.Current.ServerEventBroker;

                LayoutRoot.MouseLeftButtonDown += ToolstripViewComponent.OnLoseFocus;
                LayoutRoot.MouseRightButtonDown += ToolstripViewComponent.OnLoseFocus;

                ApplicationContext.Current.ServerEventBroker.StartApplication(startRequest);

                TileView.ApplicationRootVisual = _studyView.StudyViewCanvas;
                
                this.LayoutRoot.KeyUp += new System.Windows.Input.KeyEventHandler(OnKeyUp);
            }
		}

        void ErrorHandler_OnCriticalError(object sender, EventArgs e)
        {
            Shutdown();
        }        

        void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        //TODO: close this on error/timeout
                        StackPanel panel = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                        panel.Children.Add(new StatisticsPanel() { Margin = new Thickness(10) });
                        panel.Children.Add(new ThrottlePanel(){ Margin = new Thickness(10)});

                        PopupHelper.PopupContent("Throttle Settings", panel);
                    }
                    break;
                }

            }
        }

        private void OnServerApplicationStopped(object sender, ServerApplicationStopEventArgs e)
        {
            UIThread.Execute(() =>
            {
                if (!_suppressError)
                {
                    _suppressError = true;

                    Shutdown();

                    ApplicationStoppedEvent @event = e.ServerEvent as ApplicationStoppedEvent;
                    List<Button> buttons = new List<Button>();
                    
                    Button closeButton = new Button { Content = "Close", Margin = new Thickness(5) };
                    closeButton.Click += (s, ev) => { BrowserWindow.Close(); };
                    buttons.Add(closeButton);

                    String title= @event.IsTimedOut? "Timeout":"Error";
                    String mesage = @event.Message;


                    var window = PopupHelper.PopupMessage(title, mesage);            
                }
            });
        }
        
        private void OnSessionUpdated(object sender, ServerEventArgs ev)
        {
            UIThread.Execute(() =>
            {
                SessionUpdatedEvent @event = ev.ServerEvent as SessionUpdatedEvent;
                ApplicationBridge.Current.OnViewerSessionUpdated(this, @event);
            });
        }

        private void OnMessageBox(object sender, ServerEventArgs ev)
        {
            MessageBoxShownEvent @event = ev.ServerEvent as MessageBoxShownEvent;

			//TODO (CR May 2010): can this be consolidate or split up?
            List<Button> buttonList = new List<Button>();

            if (@event.MessageBox.Actions == WebMessageBoxActions.Ok)
            {
                Button okButton = new Button { Content = "Ok", Margin = new Thickness(5) };
                okButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.Ok, 
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(okButton);
            }
            else if (@event.MessageBox.Actions == WebMessageBoxActions.OkCancel)
            {
                Button okButton = new Button { Content = "Ok", Margin = new Thickness(5) };
                okButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.Ok,
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(okButton);
                Button cancelButton = new Button { Content = "Cancel", Margin = new Thickness(5) };
                cancelButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.Cancel,
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(cancelButton);
            }
            else if (@event.MessageBox.Actions == WebMessageBoxActions.YesNo)
            {
                Button yesButton = new Button { Content = "Yes", Margin = new Thickness(5) };
                yesButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.Yes,
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(yesButton);
                Button noButton = new Button { Content = "No", Margin = new Thickness(5) };
                noButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.No,
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(noButton);
            }
            else if (@event.MessageBox.Actions == WebMessageBoxActions.YesNoCancel)
            {
                Button yesButton = new Button { Content = "Yes", Margin = new Thickness(5) };
                yesButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.Yes,
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(yesButton);
                Button noButton = new Button { Content = "No", Margin = new Thickness(5) };
                noButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage()
                    {
                        TargetId = @event.MessageBox.Identifier,
                        Result = WebDialogBoxAction.No,
                        Identifier = Guid.NewGuid()
                    });
                };
                buttonList.Add(noButton);
                Button cancelButton = new Button { Content = "Cancel", Margin = new Thickness(5) };
                cancelButton.Click += (s, e) =>
                {
                    ApplicationContext.Current.ServerEventBroker.DispatchMessage(
                        new DismissMessageBoxMessage()
                        {
                            TargetId = @event.MessageBox.Identifier,
                            Result = WebDialogBoxAction.Cancel,
                            Identifier = Guid.NewGuid()
                        });
                };
                buttonList.Add(cancelButton);
            }
                           
            PopupHelper.PopupContent(@event.MessageBox.Title, @event.MessageBox.Message, buttonList.ToArray());      
        }

		private void ApplicationStarted(object sender, ServerEventArgs e)
		{
            if (ApplicationContext.Current.ServerEventBroker == null)
            {
                return;
            }

			ApplicationStartedEvent ev = (ApplicationStartedEvent)e.ServerEvent;
            if (ev == null)
            {
                ErrorHandler.HandleCriticalError("Unexpected event type: {0}", e.ServerEvent);
                return;
            }

			Visibility = System.Windows.Visibility.Visible;

            if (_serverApplication != null)
            {
                // Stop the prior application, note that it may have been stopped already, still send the
                // message just in case
                ApplicationContext.Current.ServerEventBroker.StopApplication(_serverApplication.Identifier);
            }

			//TODO (CR May 2010): we don't unregister these
			ApplicationContext.Current.ServerEventBroker.RegisterEventHandler(ev.StartRequestId, OnApplicationEvent);
            _context.ID = ev.SenderId;
        }

        private void OnApplicationEvent(object sender, ServerEventArgs e)
        {
            if (!(e.ServerEvent is PropertyChangedEvent))
                return;

            PropertyChangedEvent ev = (PropertyChangedEvent)e.ServerEvent;

            if (ev.PropertyName == "Application")
            {
                _serverApplication = (ViewerApplication)ev.Value;
                _context.ID = _serverApplication.Identifier;

                ApplicationContext.Current.ViewerVersion = _serverApplication.VersionString;

				//TODO (CR May 2010): we don't unregister these
                ApplicationContext.Current.ServerEventBroker.RegisterEventHandler(_serverApplication.Viewer.Identifier, OnViewerEvent);
                ToolstripViewComponent.SetIconSize(_serverApplication.Viewer.ToolStripIconSize);
                ToolstripViewComponent.SetActionModel(_serverApplication.Viewer.ToolbarActions);
                _studyView.SetImageBoxes(_serverApplication.Viewer.ImageBoxes);
                return;
            }
        }

		private void OnViewerEvent(object sender, ServerEventArgs e)
		{
			if (!(e.ServerEvent is PropertyChangedEvent))
				return;

			PropertyChangedEvent ev = (PropertyChangedEvent)e.ServerEvent;

			//TODO (CR May 2010): this is in the method above, too.  Which one works?
            if (ev.PropertyName == "Application")
            {
                _serverApplication = (ViewerApplication)ev.Value;
                _context.ID = _serverApplication.Identifier;
				//TODO (CR May 2010): we don't unregister these
				ApplicationContext.Current.ServerEventBroker.RegisterEventHandler(_serverApplication.Viewer.Identifier, OnViewerEvent);
                ToolstripViewComponent.SetIconSize(_serverApplication.Viewer.ToolStripIconSize);
                ToolstripViewComponent.SetActionModel(_serverApplication.Viewer.ToolbarActions);
                _studyView.SetImageBoxes(_serverApplication.Viewer.ImageBoxes);
                return;
            }

            if (ev.PropertyName == "ImageBoxes")
            {
                Collection<ImageBox> imageBoxes = (Collection<ImageBox>)ev.Value;
                _serverApplication.Viewer.ImageBoxes = imageBoxes;
                _studyView.SetImageBoxes(imageBoxes);
                return;
            }

            if (ev.PropertyName == "ToolbarActions")
            {
                Collection<WebActionNode> actionModel = (Collection<WebActionNode>)ev.Value;
                _serverApplication.Viewer.ToolbarActions = actionModel;
                ToolstripViewComponent.SetActionModel(_serverApplication.Viewer.ToolbarActions);
                return;
            }
		}

        public void Shutdown()
        {
            if (!_shuttingDown)
            {
                _shuttingDown = true;

                Visibility = System.Windows.Visibility.Collapsed;

                if (Shuttingdown != null)
                {
                    Shuttingdown(this, EventArgs.Empty);
                }


                try
                {
                    if (!ApplicationContext.Current.ServerEventBroker.Faulted)
                    {
                        ApplicationContext.Current.ServerEventBroker.StopApplication(_serverApplication.Identifier);

                        // Free the connection on the server
                        ApplicationContext.Current.ServerEventBroker.Disconnect("WebViewer is actively shutting down");
                    }
                }
                catch (Exception)
                {
                }  
            }
                      
        }

	    public void Dispose()
	    {
            if (ApplicationContext.Current.ServerEventBroker != null)
			{
				if (_serverApplication != null)
				{
                    ApplicationContext.Current.ServerEventBroker.UnregisterEventHandler(typeof(ApplicationStartedEvent), ApplicationStarted);
                    ApplicationContext.Current.ServerEventBroker.UnregisterEventHandler(_serverApplication.Viewer.Identifier);
                    ApplicationContext.Current.ServerEventBroker.ServerApplicationStopped -= OnServerApplicationStopped;
                    ApplicationContext.Current.ServerEventBroker.StopApplication(_serverApplication.Identifier);
                    _serverApplication = null;
				}
			}


			if (_studyView != null)
			{
                MouseHelper.SetBackgroundElement(null);
				_studyView.Destroy();
				_studyView = null;
			}
	    }
    }
}
