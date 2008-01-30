function TabControl(tabControlId)
{
   this.currentTabPage = null;
   this.tabControlId = tabControlId;

   var labelElements = document.getElementById(this.tabControlId).getElementsByTagName("LABEL");
   for(var i=0; i<labelElements.length; i++)
   {
      if(labelElements[i].getAttribute("className") == "Tab")
      {
         pageElement = document.getElementById(labelElements[i].getAttribute("htmlFor"));
         page = new TabPage(this, labelElements[i], pageElement);
         if(this.currentTabPage == null)
         {
            page.Show();
         }
      }
   }
}

TabControl.prototype.HideCurrent = function ()
{
   if(this.currentTabPage != null)
   {
      this.currentTabPage.Hide();
   }
};

function TabPage(tabControl, tabElement, pageElement)
{
   this.tabControl = tabControl;
   this.tabElement = tabElement;
   this.pageElement = pageElement;
   
   var oThis = this;
   this.tabElement.onclick = function() { oThis.Show() };
}

TabPage.prototype.PageClassName = "TabPage";
TabPage.prototype.ActivePageClassName = "TabPage Active";
TabPage.prototype.TabClassName = "Tab";
TabPage.prototype.ActiveTabClassName = "Tab Active";

TabPage.prototype.Show = function()
{
   if(this.tabControl.currentTabPage == this) return;

   this.tabControl.HideCurrent();
   
   this.pageElement.className = this.ActivePageClassName;
   this.tabElement.className = this.ActiveTabClassName;

   this.tabControl.currentTabPage = this;
};

TabPage.prototype.Hide = function()
{
   this.pageElement.className = this.PageClassName;
   this.tabElement.className = this.TabClassName;
};

function initTabs()
{
   var divs = document.getElementsByTagName("DIV");
   for(var i=0; i < divs.length; i++)
   {
      if(divs[i].getAttribute("className") == "TabControl")
      {
         new TabControl(divs[i].getAttribute("ID"));
      }
   }
}         

if ( typeof window.addEventListener != "undefined" )
{
  window.addEventListener( "load", initTabs, false );
}
// IE 
else if ( typeof window.attachEvent != "undefined" ) 
{
  window.attachEvent( "onload", initTabs );
}
