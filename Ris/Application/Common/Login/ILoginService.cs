using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Login
{
    /// <summary>
    /// Provides application login operations
    /// </summary>
    [ServiceContract]
    public interface ILoginService
    {
        /// <summary>
        /// Allows a client application to validate user credentials and obtain a set of authority tokens
        /// specifying what privileges the user has.
        /// </summary>
        /// <remarks>
        /// User validation is performed like any other application service.  If the credentials are invalid
        /// a security exception will be thrown.
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoginResponse Login(LoginRequest request);
    }
}
