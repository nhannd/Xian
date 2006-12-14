using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public class CrudActionModel : ActionModelRoot
    {

        public class CrudAction : ClickAction
        {
            internal CrudAction(string name, string icon, IResourceResolver resolver)
                : base(name, new ActionPath(string.Format("root/{0}", name), resolver), ClickActionFlags.None, resolver)
            {
                this.Tooltip = name;
                this.Label = name;
                this.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            }
        }


        private CrudAction _add;
        private CrudAction _edit;
        private CrudAction _delete;

        public CrudActionModel()
            :this(true, true, true)
        {
        }

        public CrudActionModel(bool add, bool edit, bool delete)
        {
            IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
            if (add)
            {
                this.InsertAction(_add = new CrudAction(SR.TitleAdd, "Icons.Add.png", resolver));
            }
            if (edit)
            {
                this.InsertAction(_edit = new CrudAction(SR.TitleEdit, "Icons.Edit.png", resolver));
            }
            if (delete)
            {
                this.InsertAction(_delete = new CrudAction(SR.TitleDelete, "Icons.Delete.png", resolver));
            }
        }

        public CrudAction Add
        {
            get { return _add; }
        }

        public CrudAction Edit
        {
            get { return _edit; }
        }

        public CrudAction Delete
        {
            get { return _delete; }
        }
    }
}
