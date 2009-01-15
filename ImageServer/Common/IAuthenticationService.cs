using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
    //TODO: This authentication service definition is temporary 

    [ServiceContract()]
    public interface IAuthenticationService
    {
        /// <summary>
        /// Initiates a new session for the specified user, first verifying the password,
        /// and returns a new session token if successful.
        /// </summary>
        /// <remarks>
        /// Implementors should throw a <see cref="SecurityTokenException"/> if the username
        /// or password is invalid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">Invalid username or password.</exception>
        [OperationContract]
        SessionToken SignOn(string userName, string password);

        /// <summary>
        /// Validates an existing user session, returning a new session token
        /// that has the same identifier but an updated expiry time.
        /// </summary>
        /// <remarks>
        /// Implementors should throw a <see cref="SecurityTokenException"> if the session
        /// token has expired or is otherwise invalid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">Session token expired or otherwise invalid.</exception>
        [OperationContract]
        SessionToken ValidateUserSession(string userName, SessionToken sessionToken);

        /// <summary>
        /// Terminates an existing user session.
        /// </summary>
        /// <remarks>
        /// Implementors should throw a <see cref="SecurityTokenException"> if the session
        /// token has expired or is otherwise invalid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        [OperationContract]
        void SignOff(string userName, SessionToken sessionToken);

        /// <summary>
        /// Changes the password for the specified user account.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        [OperationContract]
        void ChangePassword(string userName, string currentPassword, string newPassword);

        /// <summary>
        /// Obtains the set of authority tokens that have been granted to the 
        /// specified user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        string[] ListAuthorityTokensForUser(string userName);

        [OperationContract]
        bool AssertTokenForUser(string userName, string token);
    }
}