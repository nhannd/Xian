using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Workstation.Dashboard
{
    public interface IDashboardContent
    {
        string Name
        {
            get;
        }

        UserControl MasterView
        {
            get;
            set;
        }

        UserControl DetailView
        {
            get;
            set;
        }

		Control MasterViewHost
		{
			get;
			set;
		}

        Control DetailViewHost
        {
            get;
            set;
        }

        void OnSelected();
    }
}
