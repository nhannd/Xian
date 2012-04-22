﻿using System;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    internal class ServiceProxyHelper
    {
        public static TResult Call<TInput, TResult>(Func<TInput, TResult> function, TInput input)
        {
            try
            {
                return function(input);
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
        }
    }
}