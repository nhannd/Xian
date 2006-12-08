using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    class AddressTable : Table<Address>
    {
        public AddressTable()
        {
            IAdtService _adtService = ApplicationContext.GetService<IAdtService>();
            AddressTypeEnumTable _addressTypes = _adtService.GetAddressTypeEnumTable();

            this.Columns.Add(new TableColumn<Address, string>("Type", 
                delegate(Address a) { return _addressTypes[a.Type].Value; }, 
                0.5f));
            this.Columns.Add(new TableColumn<Address, string>("Address", 
                delegate(Address a) { return a.Format(); }, 
                2.8f));
            this.Columns.Add(new TableColumn<Address, string>("Valid From", 
                delegate(Address a) { return a.ValidRange == null ? null : Format.Date(a.ValidRange.From); }, 
                0.9f));
            this.Columns.Add(new TableColumn<Address, string>("Valid Until", delegate(Address a) { return a.ValidRange == null ? null : Format.Date(a.ValidRange.Until); }, 
                0.9f));

        }
    }
}
