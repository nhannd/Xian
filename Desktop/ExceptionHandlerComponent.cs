using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="ExceptionHandlerComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExceptionHandlerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExceptionHandlerComponent class
    /// </summary>
    [AssociateView(typeof(ExceptionHandlerComponentViewExtensionPoint))]
    public class ExceptionHandlerComponent : ApplicationComponent
    {
        private Exception _exception;
        private string _message;

        /// <summary>
        /// Constructor
        /// </summary>
        internal ExceptionHandlerComponent(Exception e, string message)
        {
            _exception = e;
            _message = message;
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public string Message
        {
            get { return _message; }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }
    }
}
