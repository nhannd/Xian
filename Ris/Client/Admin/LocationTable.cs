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

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    public class LocationTable : Table<LocationSummary>
    {
        public LocationTable()
        {
			this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnID,
				delegate(LocationSummary loc) { return loc.Id; },
				0.2f));

			this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnName,
				delegate(LocationSummary loc) { return loc.Name; },
				1.0f));

			this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnFacility,
                delegate(LocationSummary loc) { return loc.Facility.Name; },
                1.0f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnBuilding,
                delegate(LocationSummary loc) { return loc.Building; },
                0.5f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnFloor,
                delegate(LocationSummary loc) { return loc.Floor; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnPointOfCare,
                delegate(LocationSummary loc) { return loc.PointOfCare; },
                0.5f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnRoom,
                delegate(LocationSummary loc) { return loc.Room; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnBed,
                delegate(LocationSummary loc) { return loc.Bed; },
                0.2f));

        }
    }
}
