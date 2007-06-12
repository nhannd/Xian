using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="PriorReportComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PriorReportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PriorReportComponent class
    /// </summary>
    [AssociateView(typeof(PriorReportComponentViewExtensionPoint))]
    public class PriorReportComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PriorReportComponent()
        {
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
    }
}
