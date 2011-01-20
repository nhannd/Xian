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
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestOrderAttachmentFactory
    {
        internal static IList<OrderAttachment> CreateOrderAttachments()
        {
            IList<OrderAttachment> attachments = new List<OrderAttachment>();

            attachments.Add(CreateOrderAttachment());

            return attachments;
        }

        internal static OrderAttachment CreateOrderAttachment()
        {
            return new OrderAttachment(
                new OrderAttachmentCategoryEnum("PD", "Pending", null),
                TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)),
                DateTime.Now,
                TestAttachedDocumentFactory.CreateAttachedDocument());
        }
    }
}
