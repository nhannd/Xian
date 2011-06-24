using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.HangingProtocols
{
    [TypeConverter(typeof(HpSortComparerConverter))]
    internal class HpSortComparer : IHpSortComparer
    {
        public static readonly List<HpSortComparer> All;

        private readonly StackTool.ISortMenuItem _sortMenuItem;

        static HpSortComparer()
        {
            var items = new List<StackTool.ISortMenuItem>();
            var xp = new StackTool.SortMenuItemFactoryExtensionPoint();
            foreach (StackTool.ISortMenuItemFactory factory in xp.CreateExtensions())
                items.AddRange(factory.Create());
            All = CollectionUtils.Map<StackTool.ISortMenuItem, HpSortComparer>(items, (item)=> new HpSortComparer(item));
        }

        public HpSortComparer(StackTool.ISortMenuItem sortMenuItem)
        {
            _sortMenuItem = sortMenuItem;
        }
    
        #region IHpSortComparer Members

        public string  Name
        {
            get { return _sortMenuItem.Name; }
        }

        public string  Description
        {
            get { return _sortMenuItem.Description; }
        }

        public IComparer<IPresentationImage> GetComparer()
        {
            return _sortMenuItem.Comparer;
        }

        #endregion
    }
}