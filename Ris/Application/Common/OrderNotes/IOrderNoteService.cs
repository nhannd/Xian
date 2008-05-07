using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    /// <summary>
    /// Defines a service contract for working with order notes.
    /// </summary>
    [ServiceContract]
    [RisServiceProvider]
    public interface IOrderNoteService
    {
        /// <summary>
        /// Queries the contents of a specified notebox.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        QueryNoteboxResponse QueryNotebox(QueryNoteboxRequest request);

        /// <summary>
        /// Gets the entire conversation for a specified order.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetConversationResponse GetConversation(GetConversationRequest request);

        /// <summary>
        /// Get conversation editor form data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetConversationEditorFormDataResponse GetConversationEditorFormData(GetConversationEditorFormDataRequest request);

        /// <summary>
        /// Acknowledges and/or posts to the specified order conversation.  Can also be used to
        /// initiate a conversation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AcknowledgeAndPostResponse AcknowledgeAndPost(AcknowledgeAndPostRequest request);
    }
}
