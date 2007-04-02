using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [ServiceContract]
    public interface ILoginService
    {
        [OperationContract]
        LoginResponse Login(LoginRequest request);
    }
}
