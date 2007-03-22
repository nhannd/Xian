using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class EmailAddressAssembler
    {
        public EmailAddressDetail CreateEmailAddressDetail(EmailAddress emailAddress, IPersistenceContext context)
        {
            EmailAddressDetail detail = new EmailAddressDetail();

            detail.Address = emailAddress.Address;
            detail.ValidRangeFrom = emailAddress.ValidRange.From;
            detail.ValidRangeUntil = emailAddress.ValidRange.Until;

            return detail;
        }

        public EmailAddress CreateEmailAddress(EmailAddressDetail detail)
        {
            EmailAddress emailAddress = new EmailAddress();

            emailAddress.Address = detail.Address;
            emailAddress.ValidRange = new DateTimeRange(
                detail.ValidRangeFrom,
                detail.ValidRangeUntil);

            return emailAddress;
        }

    }
}
