using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using System.IO;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/JScript")]
    [ClickHandler("apply", "Apply")]

    [MenuAction("specs", "global-menus/MenuTools/Specs")]
    [ClickHandler("specs", "TestSpec")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class JscriptTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private JscriptComponent _component;

        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public JscriptTool()
        {
        }

        /// <summary>
        /// Called by the framework to initialize this tool.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // TODO: add any significant initialization code here rather than in the constructor
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            if (_component == null)
            {
                _component = new JscriptComponent();
                ApplicationComponent.LaunchAsShelf(this.Context.DesktopWindow,
                    _component,
                    SR.TitleJScriptWindow,
                    ShelfDisplayHint.DockFloat,
                    delegate(IApplicationComponent c) { _component = null; });
            }
        }

        public void TestSpec()
        {
            ResourceResolver rr = new ResourceResolver(this.GetType().Assembly);
            Stream s = rr.OpenResource("validation.xml");

            SpecificationFactory specFactory = new SpecificationFactory(s);
            ISpecification rule1 = specFactory.GetSpecification("rule1");

            PatientProfile p = new PatientProfile();
            p.Name.FamilyName = "Bill";

            bool a = rule1.Test(p.Name).Success;
        }
    }
}
