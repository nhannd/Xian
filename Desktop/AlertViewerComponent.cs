#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	[MenuAction("show", "global-menus/Test/Alert Viewer", "Show")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class AlertViewerTool : Tool<IDesktopToolContext>
	{
		private Shelf _shelf;

		public void Show()
		{
			if(_shelf == null)
			{
				_shelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					new AlertViewerComponent(),
					"Alerts",
					ShelfDisplayHint.DockRight);
				_shelf.Closed += (sender, args) => _shelf = null;
			}
			else
			{
				_shelf.Activate();
			}
		}
	}
	
	/// <summary>
    /// Extension point for views onto <see cref="AlertViewerComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class AlertViewerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// A test component not intended for production use.
	/// </summary>
	[AssociateView(typeof (AlertViewerComponentViewExtensionPoint))]
	public class AlertViewerComponent : ApplicationComponent
	{
		private readonly Table<Alert> _alertTable;
		private readonly AlertLog _log = AlertLog.Instance;

		private readonly CrudActionModel _alertActions;

		public AlertViewerComponent()
		{
			_alertTable = new Table<Alert>();
			_alertActions = new CrudActionModel(true, false, false);
		}

		public override void Start()
		{
			base.Start();

			_alertTable.Columns.Add(new TableColumn<Alert, IconSet>("Level", GetAlertIcon, 0.1f));
			_alertTable.Columns.Add(new TableColumn<Alert, string>("Message", a => a.Message, 0.9f));
			_alertTable.Columns.Add(new TableColumn<Alert, string>("Time", a => Format.DateTime(a.Time), 0.2f));
//			_alertTable.Columns.Add(new TableColumn<Alert, string>("Link", a => a.LinkText, 1) { ClickLinkDelegate = AlertLinkClicked });

			_alertTable.Items.AddRange(_log.Entries);

			_log.AlertLogged += AlertLoggedEventHandler;

			_alertActions.Add.SetClickHandler(AddAlert);
		}

		private void AddAlert()
		{
			var r = new System.Random();
			var alert = new AlertNotificationArgs((AlertLevel)r.Next(3), "power corrupts, and absolute power corrupts absolutely.",
			                      "Link", window => window.ShowMessageBox("test", MessageBoxActions.Ok));
			this.Host.DesktopWindow.ShowAlert(alert);
		}

		public override void Stop()
		{
			_log.AlertLogged -= AlertLoggedEventHandler;

			base.Stop();
		}

		#region Presentation Model

		public ActionModelNode AlertActions
		{
			get { return _alertActions; }
		}

		public ITable Alerts
		{
			get { return _alertTable; }
		}

		#endregion

		private void AlertLoggedEventHandler(object sender, ItemEventArgs<Alert> e)
		{
			_alertTable.Items.Insert(0, e.Item);
		}

		private static IconSet GetAlertIcon(Alert alert)
		{
			switch (alert.Level)
			{
				case AlertLevel.Info:
					return new IconSet("InfoSmall.png");
				case AlertLevel.Warning:
					return new IconSet("WarningSmall.png");
				case AlertLevel.Error:
					return new IconSet("ErrorSmall.png");
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
