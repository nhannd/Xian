#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class RegistrationWorklistItemKey
    {
        private readonly EntityRef _orderRef;
        private readonly EntityRef _profileRef;

        public RegistrationWorklistItemKey(EntityRef orderRef, EntityRef profileRef)
        {
            _orderRef = orderRef;
            _profileRef = profileRef;
        }

        public EntityRef OrderRef
        {
            get { return _orderRef; }
        }

        public EntityRef ProfileRef
        {
            get { return _profileRef; }
        }
    }
}
