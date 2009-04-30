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
