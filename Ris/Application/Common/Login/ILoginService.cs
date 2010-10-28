#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Login
{
    /// <summary>
    /// Provides application login operations
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    [Authentication(false)]
    public interface ILoginService
    {
        /// <summary>
        /// Allows a client application to validate user credentials and obtain a set of authority tokens
        /// specifying what privileges the user has.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserAccessDeniedException))]
        [FaultContract(typeof(PasswordExpiredException))]
        LoginResponse Login(LoginRequest request);

        /// <summary>
        /// Allows a client application to inform the server explicitly that the user has logged out
        /// of the application.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InvalidUserSessionException))]
        LogoutResponse Logout(LogoutRequest request);

        /// <summary>
        /// Allows the user of a client application to change his password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(UserAccessDeniedException))]
        ChangePasswordResponse ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Gets the list of facilities so that the user can choose their current working facility.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetWorkingFacilityChoicesResponse GetWorkingFacilityChoices(GetWorkingFacilityChoicesRequest request);
    }
}
