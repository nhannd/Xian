using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public class CrudActionModel : SimpleActionModel
    {
        public CrudActionModel()
            :this(true, true, true)
        {
        }

        public CrudActionModel(bool add, bool edit, bool delete)
            :base(new ResourceResolver(typeof(CrudActionModel).Assembly))
        {
            if (add)
            {
                this.AddAction("Add", SR.TitleAdd, "Icons.Add.png");
            }
            if (edit)
            {
                this.AddAction("Edit", SR.TitleEdit, "Icons.Edit.png");
            }
            if (delete)
            {
                this.AddAction("Delete", SR.TitleDelete, "Icons.Delete.png");
            }
        }

        public ClickAction Add
        {
            get { return this["Add"]; }
        }

        public ClickAction Edit
        {
            get { return this["Edit"]; }
        }

        public ClickAction Delete
        {
            get { return this["Delete"]; }
        }
    }
}
