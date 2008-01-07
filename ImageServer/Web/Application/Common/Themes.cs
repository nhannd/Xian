using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.ImageServer.Web.Application.Common
{

    public class Theme
    {
        #region Private members
        private string _name;

        #endregion Private members

        #region public properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion public properties

        #region constructors
        protected Theme(string name)
        {
            Name = name;
        }

        #endregion constructors

        #region Public Static Methods
        public static bool Contains(string theme)
        {
            List<Theme> list = AllThemes;
            foreach (Theme t in list)
            {
                if (t.Name == theme)
                    return true;
            }

            return false;
        }

        #endregion Public Static Methods

        #region Public Static Properties
        public static List<Theme> AllThemes
        {
            get
            {
                DirectoryInfo dInfo = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("~/App_Themes"));
                DirectoryInfo[] dArrInfo = dInfo.GetDirectories();
                List<Theme> list = new List<Theme>();
                foreach (DirectoryInfo sDirectory in dArrInfo)
                {
                    Theme temp = new Theme(sDirectory.Name);
                    list.Add(temp);
                }
                return list;
            }
        }
        #endregion Public Static Properties
    }
}
