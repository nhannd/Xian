using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services
{
    /// <summary>
    /// This class is basically a big hack to present the client with a simplified set of choices for phone
    /// number type.  This class maps back and forth between <see cref="SimplifiedPhoneType"/> and the
    /// the <see cref="TelephoneUse"/> and <see cref="TelephoneEquipment"/> enums.
    /// </summary>
    class SimplifiedPhoneTypeAssembler
    {
        enum SimplifiedPhoneType
        {
            Unknown,    // need this to deal with combinations that don't map to anything

            Home,
            Work,
            Mobile,
            Fax,
            Pager,
        }

        public EnumValueInfo GetSimplifiedPhoneType(TelephoneNumber number)
        {
            SimplifiedPhoneType t = SimplifiedPhoneType.Unknown;
            if (number.Use == TelephoneUse.PRN)
            {
                if (number.Equipment == TelephoneEquipment.PH)
                    t = SimplifiedPhoneType.Home;
                else if (number.Equipment == TelephoneEquipment.CP)
                    t = SimplifiedPhoneType.Mobile;
            }
            else if (number.Use == TelephoneUse.WPN)
            {
                if (number.Equipment == TelephoneEquipment.PH)
                    t = SimplifiedPhoneType.Work;
                else if (number.Equipment == TelephoneEquipment.BP)
                    t = SimplifiedPhoneType.Pager;
                else if (number.Equipment == TelephoneEquipment.FX)
                    t = SimplifiedPhoneType.Fax;
            }
            return new EnumValueInfo(t.ToString(), t.ToString());
        }

        public void UpdatePhoneNumber(EnumValueInfo simplePhoneType, TelephoneNumber number)
        {
            SimplifiedPhoneType type = (SimplifiedPhoneType)Enum.Parse(typeof(SimplifiedPhoneType), simplePhoneType.Code);
            switch (type)
            {
                case SimplifiedPhoneType.Home:
                    number.Equipment = TelephoneEquipment.PH;
                    number.Use = TelephoneUse.PRN;
                    break;
                case SimplifiedPhoneType.Work:
                    number.Equipment = TelephoneEquipment.PH;
                    number.Use = TelephoneUse.WPN;
                    break;
                case SimplifiedPhoneType.Mobile:
                    number.Equipment = TelephoneEquipment.CP;
                    number.Use = TelephoneUse.PRN;
                case SimplifiedPhoneType.Fax:
                    number.Equipment = TelephoneEquipment.FX;
                    number.Use = TelephoneUse.WPN;
                    break;
                case SimplifiedPhoneType.Pager:
                    number.Equipment = TelephoneEquipment.BP;
                    number.Use = TelephoneUse.WPN;
                    break;
                case SimplifiedPhoneType.Unknown:
                    // do nothing
                    break;
            }
        }

        public List<EnumValueInfo> GetSimplifiedPhoneTypeChoices(bool includeFaxPager)
        {
            List<EnumValueInfo> choices = new List<EnumValueInfo>();
            choices.Add(new EnumValueInfo(SimplifiedPhoneType.Home.ToString(), SimplifiedPhoneType.Home.ToString()));
            choices.Add(new EnumValueInfo(SimplifiedPhoneType.Work.ToString(), SimplifiedPhoneType.Work.ToString()));
            choices.Add(new EnumValueInfo(SimplifiedPhoneType.Mobile.ToString(), SimplifiedPhoneType.Mobile.ToString()));

            if (includeFaxPager)
            {
                choices.Add(new EnumValueInfo(SimplifiedPhoneType.Fax.ToString(), SimplifiedPhoneType.Fax.ToString()));
                choices.Add(new EnumValueInfo(SimplifiedPhoneType.Pager.ToString(), SimplifiedPhoneType.Pager.ToString()));
            }
        }

    }
}
