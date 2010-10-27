#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Web
{
    [ExceptionPolicyFor(typeof(LoadPriorStudiesException))]
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class PriorStudyLoaderExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
        {
            if (e is LoadPriorStudiesException)
            {
                exceptionHandlingContext.Log(LogLevel.Error, e);

                Handle(e as LoadPriorStudiesException, exceptionHandlingContext);
            }
        }

        private static void Handle(LoadPriorStudiesException exception, IExceptionHandlingContext context)
        {
            if (exception.FindFailed)
            {
                context.ShowMessageBox(SR.MessageLoadPriorsFindErrors);
            }
            else if (ShouldShowErrorMessage(exception))
            {
                context.ShowMessageBox(ClearCanvas.Web.Services.ExceptionTranslator.Translate(exception));
            }
        }

        private static bool ShouldShowErrorMessage(LoadPriorStudiesException exception)
        {
            if (exception.IncompleteCount > 0)
                return true;

            if (exception.NotFoundCount > 0)
                return true;

            if (exception.UnknownFailureCount > 0)
                return true;

            return false;
        }
    }
}
