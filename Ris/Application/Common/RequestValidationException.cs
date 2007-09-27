using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// Used by a service to indicate to the client that a request was rejected because it was invalid.  The client will likely
    /// display the contained message to the end user.  Therefore, the message should be appropriate for the end-user.
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        public static RequestValidationException FromTestResultReasons(string message, TestResultReason[] reasons)
        {
            List<string> messages = new List<string>();
            foreach (TestResultReason reason in reasons)
                messages.AddRange(BuildMessageStrings(reason));

            if (messages.Count > 0)
            {
                message += "\n" + StringUtilities.Combine<string>(messages, "\n");
            }
            return new RequestValidationException(message);
        }

        private static List<string> BuildMessageStrings(TestResultReason reason)
        {
            List<string> messages = new List<string>();
            if (reason.Reasons.Length == 0)
                messages.Add(reason.Message);
            else
            {
                foreach (TestResultReason subReason in reason.Reasons)
                {
                    List<string> subMessages = BuildMessageStrings(subReason);
                    foreach (string subMessage in subMessages)
                    {
                        messages.Add(string.Format("{0} {1}", reason.Message, subMessage));
                    }
                }
            }
            return messages;
        }


        public RequestValidationException(string message)
            :base(message)
        {
        }

        public RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
