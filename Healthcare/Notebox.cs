using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    public abstract class Notebox : INotebox
    {
        public abstract IList GetItems(INoteboxQueryContext wqc);
        public abstract int GetItemCount(INoteboxQueryContext wqc);

        public abstract NoteboxItemSearchCriteria[] GetInvariantCriteria(INoteboxQueryContext wqc);
    }
}
