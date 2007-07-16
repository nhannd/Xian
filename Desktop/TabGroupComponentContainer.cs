using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="TabComponent"/>
    /// </summary>
    public class TabbedGroupsComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public enum LayoutDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    [AssociateView(typeof(TabbedGroupsComponentContainerViewExtensionPoint))]
    public class TabGroupComponentContainer : ApplicationComponentContainer
    {
        public class TabGroupHost : ApplicationComponentHost
        {
            private TabGroupComponentContainer _owner;
            private TabGroup _tabGroup;

            internal TabGroupHost(
                TabGroupComponentContainer owner,
                TabGroup tabGroup)
                : base(tabGroup.Component)
            {
                Platform.CheckForNullReference(owner, "owner");
                Platform.CheckForNullReference(tabGroup, "pane");

                _owner = owner;
                _tabGroup = tabGroup;
            }

            public TabGroupComponentContainer Owner
            {
                get { return _owner; }
            }

            #region ApplicationComponentHost overrides

            public override void SetTitle(string title)
            {
                _owner.Host.SetTitle(title);
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

            #endregion
        }

        private List<TabGroup> _tabGroups;
        private LayoutDirection _layoutDirection;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TabGroupComponentContainer(LayoutDirection layoutDirection)
        {
            _tabGroups = new List<TabGroup>();
            _layoutDirection = layoutDirection;
        }

        public void AddTabGroup(TabGroup tg)
        {
            //if (tg != null && tg.ComponentHost != null && tg.ComponentHost.IsStarted)
            //    throw new InvalidOperationException(SR.ExceptionCannotSetTabGroupAfterContainerStarted);

            tg.ComponentHost = new TabGroupHost(this, tg);
            _tabGroups.Add(tg);
        }

        public IList<TabGroup> TabGroups
        {
            get { return _tabGroups.AsReadOnly(); ; }
        }

        public LayoutDirection LayoutDirection
        {
            get { return _layoutDirection; }
        }

        public TabGroup GetTabGroup(TabPage page)
        {
            foreach (TabGroup tg in _tabGroups)
            {
                if (CollectionUtils.Contains<TabPage>(tg.Component.Pages,
                    delegate(TabPage tp) { return tp == page; }))
                {
                    return tg;
                }
            }

            return null;
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            foreach (TabGroup tabGroup in _tabGroups)
            {
                tabGroup.ComponentHost.StartComponent();
            }
        }

        public override void Stop()
        {
            foreach (TabGroup tabGroup in _tabGroups)
            {
                tabGroup.ComponentHost.StopComponent();
            }

            base.Stop();
        }

        public override IActionSet ExportedActions
        {
            get
            {
                IActionSet exportedActionSet = new ActionSet(); ;

                // export the actions from all subcomponents
                foreach (TabGroup tabGroup in _tabGroups)
                {
                    exportedActionSet.Union(tabGroup.Component.ExportedActions);
                }

                return exportedActionSet;
            }
        }

        #endregion

        #region ApplicationComponentContainer overrides

        public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get 
            {
                List<IApplicationComponent> components = new List<IApplicationComponent>();
                foreach (TabGroup tabGroup in _tabGroups)
                {
                    components.AddRange(tabGroup.Component.ContainedComponents);
                }
                return components;
            }
        }

        public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get 
            {
                List<IApplicationComponent> components = new List<IApplicationComponent>();
                foreach (TabGroup tabGroup in _tabGroups)
                {
                    components.AddRange(tabGroup.Component.VisibleComponents);
                }
                return components;
            }
        }

        public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are started by default
        }

        public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are visible by default
        }

        #endregion

    }
}
