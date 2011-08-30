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
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using System.Windows.Input;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Resources;
using ClearCanvas.Web.Client.Silverlight.Utilities;

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

            if (ApplicationContext.Current != null)
                ApplicationContext.Initialize();

			_context = ApplicationContext.Current;
            ErrorHandler.OnCriticalError += ErrorHandler_OnCriticalError;

			_context.ServerEventBroker.RegisterEventHandler(typeof(ApplicationStartedEvent), ApplicationStarted);
            _context.ServerEventBroker.RegisterEventHandler(typeof(SessionUpdatedEvent), OnSessionUpdated);
            _context.ServerEventBroker.RegisterEventHandler(typeof(MessageBoxShownEvent), OnMessageBox);
            _context.ServerEventBroker.ServerApplicationStopped += OnServerApplicationStopped;
            
            _studyView = new StudyView();
			StudyViewContainer.Children.Add(_studyView);
            MouseHelper.SetBackgroundElement(LayoutRoot);

            if (startRequest == null)
            {
                //TODO: replace this with the custom dialog. For some reason, it doesn't work here.
                System.Windows.MessageBox.Show(ErrorMessages.MissingParameters);
            }
            else
            {

                ToolstripViewComponent.EventDispatcher = _context.ServerEventBroker;

                LayoutRoot.MouseLeftButtonDown += ToolstripViewComponent.OnLoseFocus;
                LayoutRoot.MouseRightButtonDown += ToolstripViewComponent.OnLoseFocus;

                _context.ServerEventBroker.StartApplication(startRequest);

                TileView.ApplicationRootVisual = _studyView.StudyViewCanvas;
                
                this.LayoutRoot.KeyUp += OnKeyUp;
            }
		}

        void ErrorHandler_OnCriticalError(object sender, EventArgs e)
        {
            Shutdown();
        }        

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        //TODO: close this on error/timeout
                        var panel = new StackPanel { Orientation = Orientation.Horizontal };
                        panel.Children.Add(new StatisticsPanel { Margin = new Thickness(10) });
                        panel.Children.Add(new ThrottlePanel { Margin = new Thickness(10)});

                        PopupHelper.PopupContent(DialogTitles.ThrottleSettings, panel);
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

                    ApplicationStoppedEvent @event = e.ServerEvent;

                    String title= @event.IsTimedOut? DialogTitles.Timeout: DialogTitles.Error;
                    String mesage = @event.Message;


                    var window = PopupHelper.PopupMessage(title, mesage);            
                }
            });
        }
        
        private void OnSessionUpdated(object sender, ServerEventArgs ev)
        {
            UIThread.Execute(() =>
            {
                var @event = ev.ServerEvent as SessionUpdatedEvent;
                ApplicationBridge.Current.OnViewerSessionUpdated(this, @event);
            });
        }

        private void OnMessageBox(object sender, ServerEventArgs ev)
        {
            var @event = ev.ServerEvent as MessageBoxShownEvent;

			//TODO (CR May 2010): can this be consolidate or split up?
            List<Button> buttonList = new List<Button>();

            if (@event.MessageBox.Actions == WebMessageBoxActions.Ok)
            {
                var okButton = new Button { Content = Labels.ButtonOK, Margin = new Thickness(5) };
                okButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                                                 TargetId = @event.MessageBox.Identifier,
                                                                                                                 Result = WebDialogBoxAction.Ok, 
                                                                                                                 Identifier = Guid.NewGuid()
                                                                                                             });
                buttonList.Add(okButton);
            }
            else if (@event.MessageBox.Actions == WebMessageBoxActions.OkCancel)
            {
                var okButton = new Button { Content = Labels.ButtonOK, Margin = new Thickness(5) };
                okButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                                                 TargetId = @event.MessageBox.Identifier,
                                                                                                                 Result = WebDialogBoxAction.Ok,
                                                                                                                 Identifier = Guid.NewGuid()
                                                                                                             });
                buttonList.Add(okButton);
                var cancelButton = new Button { Content = Labels.ButtonCancel, Margin = new Thickness(5) };
                cancelButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                               {
                                                                                                   TargetId = @event.MessageBox.Identifier,
                                                                                                   Result = WebDialogBoxAction.Cancel,
                                                                                                   Identifier = Guid.NewGuid()
                                                                                               });
                buttonList.Add(cancelButton);
            }
            else if (@event.MessageBox.Actions == WebMessageBoxActions.YesNo)
            {
                var yesButton = new Button { Content = Labels.ButtonYes, Margin = new Thickness(5) };
                yesButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                                              {
                                                                                                                  TargetId = @event.MessageBox.Identifier,
                                                                                                                  Result = WebDialogBoxAction.Yes,
                                                                                                                  Identifier = Guid.NewGuid()
                                                                                                              });
                buttonList.Add(yesButton);
                var noButton = new Button { Content = Labels.ButtonNo, Margin = new Thickness(5) };
                noButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                                                 TargetId = @event.MessageBox.Identifier,
                                                                                                                 Result = WebDialogBoxAction.No,
                                                                                                                 Identifier = Guid.NewGuid()
                                                                                                             });
                buttonList.Add(noButton);
            }
            else if (@event.MessageBox.Actions == WebMessageBoxActions.YesNoCancel)
            {
                var yesButton = new Button { Content = Labels.ButtonYes, Margin = new Thickness(5) };
                yesButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                                              {
                                                                                                                  TargetId = @event.MessageBox.Identifier,
                                                                                                                  Result = WebDialogBoxAction.Yes,
                                                                                                                  Identifier = Guid.NewGuid()
                                                                                                              });
                buttonList.Add(yesButton);
                var noButton = new Button { Content = Labels.ButtonNo, Margin = new Thickness(5) };
                noButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(new DismissMessageBoxMessage
                                                                                                             {
                                                                                         TargetId =
                                                                                             @event.MessageBox.
                                                                                             Identifier,
                                                                                         Result = WebDialogBoxAction.No,
                                                                                         Identifier = Guid.NewGuid()
                                                                                     });
                buttonList.Add(noButton);
                var cancelButton = new Button { Content = Labels.ButtonCancel, Margin = new Thickness(5) };
                cancelButton.Click += (s, e) => _context.ServerEventBroker.DispatchMessage(
                    new DismissMessageBoxMessage()
                        {
                            TargetId = @event.MessageBox.Identifier,
                            Result = WebDialogBoxAction.Cancel,
                            Identifier = Guid.NewGuid()
                        });
                buttonList.Add(cancelButton);
            }
                           
            PopupHelper.PopupContent(@event.MessageBox.Title, @event.MessageBox.Message, buttonList.ToArray());      
        }

		private void ApplicationStarted(object sender, ServerEventArgs e)
		{
            if (_context.ServerEventBroker == null)
            {
                return;
            }

			var ev = (ApplicationStartedEvent)e.ServerEvent;
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
                _context.ServerEventBroker.StopApplication(_serverApplication.Identifier);
            }

			//TODO (CR May 2010): we don't unregister these
            _context.ServerEventBroker.RegisterEventHandler(ev.StartRequestId, OnApplicationEvent);
            _context.ID = ev.SenderId;
        }

        private void OnApplicationEvent(object sender, ServerEventArgs e)
        {
            if (!(e.ServerEvent is PropertyChangedEvent))
                return;

            var ev = (PropertyChangedEvent)e.ServerEvent;

            if (ev.PropertyName == "Application")
            {
                _serverApplication = (ViewerApplication)ev.Value;
                _context.ID = _serverApplication.Identifier;

                ApplicationContext.Current.ViewerVersion = _serverApplication.VersionString;

				//TODO (CR May 2010): we don't unregister these
                _context.ServerEventBroker.RegisterEventHandler(_serverApplication.Viewer.Identifier, OnViewerEvent);
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

			var ev = (PropertyChangedEvent)e.ServerEvent;

			//TODO (CR May 2010): this is in the method above, too.  Which one works?
            if (ev.PropertyName == "Application")
            {
                _serverApplication = (ViewerApplication)ev.Value;
                _context.ID = _serverApplication.Identifier;
				//TODO (CR May 2010): we don't unregister these
                _context.ServerEventBroker.RegisterEventHandler(_serverApplication.Viewer.Identifier, OnViewerEvent);
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
                var actionModel = (Collection<WebActionNode>)ev.Value;
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

                Visibility = Visibility.Collapsed;

                if (_studyView != null)
                {
                    MouseHelper.SetBackgroundElement(null);
                    _studyView.Destroy();
                    _studyView = null;
                }

                if (Shuttingdown != null)
                {
                    Shuttingdown(this, EventArgs.Empty);
                }


                try
                {
                    if (_context.ServerEventBroker.Faulted)
                    {
                        _context.ServerEventBroker.StopApplication(_serverApplication.Identifier);

                        // Free the connection on the server
                        _context.ServerEventBroker.Disconnect("WebViewer is actively shutting down");
                    }
                }
                catch (Exception)
                {
                }  
            }
                      
        }

	    public void Dispose()
	    {
            if (_context.ServerEventBroker != null)
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

            Shutdown();
	    }
    }
}
