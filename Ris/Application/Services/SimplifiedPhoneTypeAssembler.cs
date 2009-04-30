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
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
    /// <summary>
    /// This class is basically a big hack to present the client with a simplified set of choices for phone
    /// number type.  This class maps back and forth between <see cref="SimplifiedPhoneType"/> and the
    /// the <see cref="TelephoneUse"/> and <see cref="TelephoneEquipment"/> enums.
    /// </summary>
    class SimplifiedPhoneTypeAssembler
    {
        public enum SimplifiedPhoneType
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
                    break;
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

        public List<EnumValueInfo> GetPatientPhoneTypeChoices()
        {
            // order is important because it is the order that things will show up in the UI by default
            return GetPhoneTypeChoices(
                new SimplifiedPhoneType[]
                    {
                        SimplifiedPhoneType.Home,
                        SimplifiedPhoneType.Work,
                        SimplifiedPhoneType.Mobile
                    });
        }

        public List<EnumValueInfo> GetPractitionerPhoneTypeChoices()
        {
            // order is important because it is the order that things will show up in the UI by default
            return GetPhoneTypeChoices(
                new SimplifiedPhoneType[]
                    {
                        SimplifiedPhoneType.Fax,
                        SimplifiedPhoneType.Work,
                        SimplifiedPhoneType.Mobile,
                        SimplifiedPhoneType.Pager
                    });
        }

        public List<EnumValueInfo> GetPhoneTypeChoices(SimplifiedPhoneType[] list)
        {
            return CollectionUtils.Map<SimplifiedPhoneType, EnumValueInfo>(list,
                delegate(SimplifiedPhoneType t) { return new EnumValueInfo(t.ToString(), t.ToString()); });
        }



    }
}
