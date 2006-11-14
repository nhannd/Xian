using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitLocationsSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitLocationsSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitLocationsSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitLocationsSummaryComponentViewExtensionPoint))]
    public class VisitLocationsSummaryComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VisitLocationsSummaryComponent()
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
