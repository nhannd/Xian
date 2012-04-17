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
using System.Collections;
using System.Linq;
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
		enum Filters
		{
			All,
			ErrorsAndWarnings,
			Errors
		}


		private readonly Table<Alert> _alertTable;
		private readonly AlertLog _log = AlertLog.Instance;
		private Filters _filter;

		private readonly CrudActionModel _alertActions;

		public AlertViewerComponent()
		{
			_alertTable = new Table<Alert>();
			_alertActions = new CrudActionModel(true, true, true);
		}

		public override void Start()
		{
			base.Start();

			_alertTable.Columns.Add(new TableColumn<Alert, IconSet>("Level", GetAlertIcon, 0.1f));
			_alertTable.Columns.Add(new TableColumn<Alert, string>("Message", a => a.Message, 0.9f));
			_alertTable.Columns.Add(new TableColumn<Alert, string>("Time", a => Format.DateTime(a.Time), 0.4f));

			Refresh();

			_log.AlertLogged += AlertLoggedEventHandler;

			_alertActions.Add.SetClickHandler(() => AddAlert(AlertLevel.Info));
			_alertActions.Edit.SetClickHandler(() => AddAlert(AlertLevel.Warning));
			_alertActions.Delete.SetClickHandler(() => AddAlert(AlertLevel.Error));
		}

		private void AddAlert(AlertLevel level)
		{
			var msg = "";
			switch (level)
			{
				case AlertLevel.Info:
					msg = "It's a quiet day in the office.";
					break;
				case AlertLevel.Warning:
					msg = "Power corrupts, and absolute power corrupts absolutely.";
					break;
				case AlertLevel.Error:
					msg = "You know, I thought everything was alright, but now that I'm looking into it, it's clear there are some issues here, and I'm not sure what we can do about it.";
					break;
				default:
					throw new ArgumentOutOfRangeException("level");
			}

			var alert = new AlertNotificationArgs(level, msg,
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

		public IList FilterChoices
		{
			get { return Enum.GetValues(typeof(Filters)); }
		}

		public string FormatFilter(object item)
		{
			var filter = (Filters) item;
			switch (filter)
			{
				case Filters.All:
					return SR.AlertFilterAll;
				case Filters.ErrorsAndWarnings:
					return SR.AlertFilterErrorsAndWarnings;
				case Filters.Errors:
					return SR.AlertFilterErrorsOnly;
			}
			throw new ArgumentOutOfRangeException();
		}

		public object Filter
		{
			get { return _filter; }
			set
			{
				var f = (Filters) value;
				if(f != _filter)
				{
					_filter = f;
					NotifyPropertyChanged("Filter");
					Refresh();
				}
			}
		}

		#endregion

		private void Refresh()
		{
			_alertTable.Items.Clear();
			_alertTable.Items.AddRange(_log.Entries.Where(Include).Reverse());
		}

		private bool Include(Alert alert)
		{
			return _filter == Filters.All
			       || (_filter == Filters.ErrorsAndWarnings && alert.Level != AlertLevel.Info)
			       || (_filter == Filters.Errors && alert.Level == AlertLevel.Error);
		}

		private void AlertLoggedEventHandler(object sender, ItemEventArgs<Alert> e)
		{
			_alertTable.Items.Insert(0, e.Item);
		}

		private static IconSet GetAlertIcon(Alert alert)
		{
			return alert.Level.GetIcon();
		}
	}
}
