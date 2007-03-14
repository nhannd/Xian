using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class TelephoneNumberAssembler
    {
        public TelephoneDetail CreateTelephoneDetail(TelephoneNumber telephoneNumber, IPersistenceContext context)
        {
            TelephoneDetail telephoneDetail = new TelephoneDetail();

            telephoneDetail.CountryCode = telephoneNumber.CountryCode;
            telephoneDetail.AreaCode = telephoneNumber.AreaCode;
            telephoneDetail.Number = telephoneNumber.Number;
            telephoneDetail.Extension = telephoneNumber.Extension;

            TelephoneUseEnumTable useEnumTable = context.GetBroker<ITelephoneUseEnumBroker>().Load();
            telephoneDetail.Use.Code = telephoneNumber.Use.ToString();
            telephoneDetail.Use.Value = useEnumTable[telephoneNumber.Use].Values;

            TelephoneEquipmentEnumTable equipmentEnumTable = context.GetBroker<ITelephoneEquipmentEnumBroker>().Load();
            telephoneDetail.Equipment.Code = telephoneNumber.Equipment.ToString();
            telephoneDetail.Equipment.Value = equipmentEnumTable[telephoneNumber.Equipment].Values;

            telephoneDetail.ValidRangeFrom = telephoneNumber.ValidRange.From;
            telephoneDetail.ValidRangeUntil = telephoneNumber.ValidRange.Until;

            return telephoneDetail;
        }

        public TelephoneNumber CreateTelephoneNumber(TelephoneDetail telephoneDetail)
        {
            TelephoneNumber telephoneNumber = new TelephoneNumber();

            telephoneNumber.CountryCode = telephoneDetail.CountryCode;
            telephoneNumber.AreaCode = telephoneDetail.AreaCode;
            telephoneNumber.Number = telephoneDetail.Number;
            telephoneNumber.Extension = telephoneDetail.Extension;
            telephoneNumber.Use = (TelephoneUse)Enum.Parse(typeof(TelephoneUse), telephoneDetail.Use.Code);
            telephoneNumber.Equipment = (TelephoneEquipment)Enum.Parse(typeof(TelephoneEquipment), telephoneDetail.Equipment.Code);
            telephoneNumber.ValidRange.From = telephoneDetail.ValidRangeFrom;
            telephoneNumber.ValidRange.Until = elephoneDetail.ValidRangeUntil;

            return telephoneNumber;
        }

        public void AddTelephoneNumber(TelephoneDetail phoneDetail, List<TelephoneNumber> phoneNumbers)
        {
            //TODO: Check automatic expiration of Telephone functionality
            TelephoneNumber newNumber = CreateTelephoneNumber(phoneDetail);
            
            foreach (TelephoneNumber phone in phoneNumbers)
            {
                if (newNumber.Equipment.Equals(phone.Equipment) &&
                    newNumber.Use.Equals(phone.Use))
                {
                    // Automatically expired any previous phone numbers
                    phone.ValidRange.Until = Platform.Time.Date;
                }
            }

            phoneNumbers.Add(newNumber);
        }
    }
}
