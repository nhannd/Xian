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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitLocationsSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitLocationsSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitLocationsSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitLocationsSummaryComponentViewExtensionPoint))]
    public class VisitLocationsSummaryComponent : ApplicationComponent
    {
        private VisitDetail _visit;
        private readonly VisitLocationTable _locationsTable;
        private VisitLocationDetail _currentVisitLocationSelection;
        private readonly List<EnumValueInfo> _visitLocationRoleChoices;
        private readonly CrudActionModel _visitLocationActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitLocationsSummaryComponent(List<EnumValueInfo> visitLocationRoleChoices)
        {
            _locationsTable = new VisitLocationTable();
            _visitLocationRoleChoices = visitLocationRoleChoices;

            _visitLocationActionHandler = new CrudActionModel();
            _visitLocationActionHandler.Add.SetClickHandler(AddVisitLocation);
            _visitLocationActionHandler.Edit.SetClickHandler(UpdateSelectedVisitLocation);
            _visitLocationActionHandler.Delete.SetClickHandler(DeleteSelectedVisitLocation);

            _visitLocationActionHandler.Add.Enabled = true;
        }

        public override void Start()
        {
            LoadVisitLocations();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public VisitDetail Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public ITable Locations
        {
            get { return _locationsTable; }
        }

        public VisitLocationDetail CurrentVisitLocationSelection
        {
            get { return _currentVisitLocationSelection; }
            set
            {
                _currentVisitLocationSelection = value;
                VisitLocationSelectionChanged();
            }
        }

        public void SetSelectedVisitLocation(ISelection selection)
        {
            this.CurrentVisitLocationSelection = (VisitLocationDetail)selection.Item;
        }

        private void VisitLocationSelectionChanged()
        {
            if (_currentVisitLocationSelection != null)
            {
                _visitLocationActionHandler.Edit.Enabled = true;
                _visitLocationActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitLocationActionHandler.Edit.Enabled = false;
                _visitLocationActionHandler.Delete.Enabled = false;
            }
        }


        public ActionModelNode VisitLocationActionModel
        {
            get { return _visitLocationActionHandler; }
        }

        public void AddVisitLocation()
        {
            //DummyAddVisitLocation();
            
            LoadVisitLocations();
            this.Modified = true;
        }

        public void UpdateSelectedVisitLocation()
        {
            //DummyUpdateSelectedVisitLocation();

            LoadVisitLocations();
            this.Modified = true;
        }

        public void DeleteSelectedVisitLocation()
        {
            _visit.Locations.Remove(_currentVisitLocationSelection);

            LoadVisitLocations();
            this.Modified = true;
        }

        public void LoadVisitLocations()
        {
            _locationsTable.Items.Clear();
            _locationsTable.Items.AddRange(_visit.Locations);
        }
    }
}
