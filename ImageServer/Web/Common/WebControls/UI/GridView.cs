using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:GridView runat=server></{0}:GridView>")]
    public class GridView : System.Web.UI.WebControls.GridView , IScriptControl
    {
        private string onClientRowClick;
        private string onClientRowDblClick;

        public string OnClientRowClick
        {
            get { return onClientRowClick; }
            set { onClientRowClick = value; }
        }

        public string OnClientRowDblClick
        {
            get { return onClientRowDblClick; }
            set { onClientRowDblClick = value; }
        }

        private Hashtable _prevSelectedRows = new Hashtable();

        #region IScriptControl Members

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor(this.GetType().FullName, ClientID);
            desc.AddProperty("SelectedRowIndicesField", ClientID + "SelectedRowIndices");
            string style = StyleToString(SelectedRowStyle);
            desc.AddProperty("SelectedRowStyle", style);
            desc.AddProperty("SelectedRowCSS", SelectedRowStyle.CssClass);

            style = StyleToString(RowStyle);
            desc.AddProperty("UnSelectedRowStyle", style);
            desc.AddProperty("UnSelectedRowCSS", RowStyle.CssClass);

            if (this.OnClientRowClick!=null)
                desc.AddEvent("onClientRowClick", this.OnClientRowClick);

            if (OnClientRowDblClick!=null)
                desc.AddEvent("onClientRowDblClick", this.OnClientRowDblClick);
            
            return new ScriptDescriptor[] { desc };
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();

            reference.Path = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView.js");
            //reference.Path = ResolveClientUrl("SampleTextBox.js"); //ResolveClientUrl("SampleTextBox.js");
            return new ScriptReference[] { reference };
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptControl(this);    
            }
            
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptDescriptors(this);
            }
            base.Render(writer);
        }

        protected override void OnSelectedIndexChanging(GridViewSelectEventArgs e)
        {

            base.OnSelectedIndexChanging(e);

            int rowIndex = e.NewSelectedIndex;

            Rows[rowIndex].RowState = DataControlRowState.Selected;
            Rows[rowIndex].Attributes["selected"] = "true";
            _prevSelectedRows[rowIndex] = true;
        
        }

        private int firstSelectedIndex = -1;
        public override int SelectedIndex
        {
            get
            {
                return firstSelectedIndex;
            }
            set
            {
                firstSelectedIndex = value;

            }
        }

        public int[] SelectedIndices
        {
            get
            {
                if (_prevSelectedRows.Keys.Count==0)
                    return null;

                int[] rows = new int[_prevSelectedRows.Keys.Count];
                int i = 0;
                foreach (int r in _prevSelectedRows.Keys)
                    rows[i++] = r;

                return rows;
            }
            set
            {
                ClearSelections();
                for(int i=0; i<value.Length; i++)
                {
                    int rowIndex = value[i];
                    SelectRow(rowIndex);
                }
            }
        }

        protected void SelectRow(int rowIndex)
        {
            Rows[rowIndex].RowState = DataControlRowState.Selected;
            Rows[rowIndex].Attributes["selected"] = "true";
            _prevSelectedRows[rowIndex] = true;
        }

        protected void ClearSelections()
        {
            int[] rows = SelectedIndices;
            if (rows!=null)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    int rowIndex = rows[i];
                    Rows[rowIndex].RowState = DataControlRowState.Normal;
                    Rows[rowIndex].Attributes["selected"] = "false";
                }
            }

            _prevSelectedRows = new Hashtable();
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int x = SelectedIndex;
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
           
            
            
            if (!DesignMode)
            {
                if (Page.IsPostBack)
                {
                    string statesValue  = Page.Request[ClientID + "SelectedRowIndices"];
                    if (statesValue != null && statesValue!="")
                    {
                        string[] states = statesValue.Split(',');
                        int firstSelectedIndex = -1;
                        for(int i=0; i<states.Length; i++)
                        {
                            int r = int.Parse(states[i]);
                            _prevSelectedRows[r] = true;
                            if (firstSelectedIndex < 0)
                                firstSelectedIndex = r;
                            
                        }

                        SelectedIndex = firstSelectedIndex;
                    }
                    else
                    {
                        SelectedIndex = -1;
                    }
                }
                else if (SelectedIndex>0)
                {
                    _prevSelectedRows[SelectedIndex] = SelectedIndex>=0;
                        
                }
            }
                    
        }

        private static string Serialize(ICollection array)
        {
            string value = "";
            IEnumerator enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (value == "")
                    value = enumerator.Current.ToString();
                else
                    value += "," + enumerator.Current.ToString();

            }
            return value;
        }

        private static string Serialize(int[] array)
        {
            string value = "";
            for (int i = 0; i < array.Length; i++)
                if (value == "")
                    value = array[i].ToString();
                else
                    value += "," + array[i].ToString();

            return value;
        }

       
        

        public override void RenderControl(HtmlTextWriter writer)
        {
            string value = Serialize(_prevSelectedRows.Keys);


            Page.ClientScript.RegisterHiddenField(ClientID + "SelectedRowIndices", value);
            base.RenderControl(writer);
            
        }

        

        protected override void OnRowDataBound(GridViewRowEventArgs e)
        {
            base.OnRowDataBound(e);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["isdatarow"] = "true";
                e.Row.Attributes["rowIndex"] = (e.Row.RowIndex ).ToString();
                //e.Row.Style.Add("cursor", "hand");
                //e.Row.Style.Add("cursor", "pointer");

                
                if (_prevSelectedRows.ContainsKey(e.Row.RowIndex))
                {
                    // SelectedIndex = e.Row.RowIndex; // can't use SelectedIndex, Rows doesn't include the new row yet
                    e.Row.RowState = DataControlRowState.Selected;
                    e.Row.Attributes["selected"] = "true";

                }
                else
                {
                    //e.Row.RowState = e.Row.RowIndex % 2 == 0 ? DataControlRowState.Normal : DataControlRowState.Alternate;
                    e.Row.Attributes["selected"] = "false";

                }
                
            }

           

        }

        public string StyleToString(Style s)
        {
            StringBuilder sb = new StringBuilder();
            
            if (s.BackColor != Color.Empty)
                sb.Append("background-color:"
                   + ColorTranslator.ToHtml(s.BackColor) + ";");
            if (s.ForeColor != Color.Empty)
                sb.Append("color:"
                   + ColorTranslator.ToHtml(s.ForeColor) + ";");

            #region -- Border --
            string color = "black";
            string width = "";
            string style = "solid";
            if (s.BorderColor != Color.Empty)
                color = ColorTranslator.ToHtml(s.BorderColor);
            if (s.BorderStyle != BorderStyle.NotSet)
                style = s.BorderStyle.ToString();
            if (s.BorderWidth != Unit.Empty)
                width = s.BorderWidth.ToString();
            if (color != "" && width != "" && style != "")
                sb.Append("border:" + color + " " + width + " " + style + ";");
            #endregion

            #region -- Font --

            #region -- Font General --
            if (s.Font.Size != FontUnit.Empty)
                sb.Append("font-size:" + s.Font.Size.ToString() + ";");
            if (s.Font.Bold == true)
                sb.Append("font-weight:Bold;");
            if (s.Font.Italic == true)
                sb.Append("font-style:Italic;");
            #endregion

            #region -- Font Names --
            ArrayList fontList = new ArrayList();
            if (s.Font.Name.Length != 0)
                fontList.Add(s.Font.Name);
            foreach (string f in s.Font.Names)
                fontList.Add(f);
            if (fontList.Count > 0)
            {
                string fontString = "";
                for (int i = 0; i < fontList.Count; i++)
                {
                    if (i == 0)
                        fontString = (string)fontList[i];
                    else
                        fontString += ", " + fontList[i];
                }
                sb.Append("font-family:" + fontString + ";");
            }
            #endregion

            #region - Text Decoration --
            ArrayList decorList = new ArrayList();
            if (s.Font.Underline == true)
                decorList.Add("underline");
            if (s.Font.Overline == true)
                decorList.Add("overline");
            if (s.Font.Strikeout == true)
                decorList.Add("line-through");
            if (decorList.Count > 0)
            {
                string strDecor = "";
                for (int i = 0; i < decorList.Count; i++)
                {
                    if (i == 0)
                        strDecor = (string)decorList[i];
                    else
                        strDecor += ", " + decorList[i];
                }
                sb.Append("text-decoration:" + strDecor + ";");
            }
            #endregion

            #endregion

            #region -- Height and Width --
            if (!s.Height.IsEmpty)
                sb.Append("height:" + s.Height.ToString() + ";");
            if (!s.Width.IsEmpty)
                sb.Append("width:" + s.Width.ToString() + ";");
            #endregion

            return sb.ToString();

        }

        #endregion
    }
}
