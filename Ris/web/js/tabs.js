var tabsInitialized = false;

function TabControl(tabControlId)
{
	this.currentTabPage = null;
	this.tabControlId = tabControlId;

	var labelElements = document.getElementById(this.tabControlId).getElementsByTagName("LABEL");
	for(var i=0; i<labelElements.length; i++)
	{
		if(labelElements[i].getAttribute(classAttribute) == "Tab")
		{
			pageElement = document.getElementById(labelElements[i].getAttribute(forAttribute));
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
	var oldOnClick = this.tabElement.onclick || function() {};
	oThis.tabElement.onclick = function() { oldOnClick(); oThis.Show() };
}

TabPage.prototype.PageClassName = "TabPage";
TabPage.prototype.ActivePageClassName = "TabPage ActiveTabPage";
TabPage.prototype.TabClassName = "Tab";
TabPage.prototype.ActiveTabClassName = "Tab ActiveTab";

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
	if (tabsInitialized)
		return;

	var divs = document.getElementsByTagName("DIV");
	for(var i=0; i < divs.length; i++)
	{
		if(divs[i].getAttribute(classAttribute) == "TabControl")
		{
			new TabControl(divs[i].getAttribute("ID"));
		}
	}
	
	tabsInitialized = true;
}

var classAttribute = "class";
var forAttribute = "for";

if ( typeof window.addEventListener != "undefined" )
{
  window.addEventListener( "load", initTabs, false );
}
// IE 
else if ( typeof window.attachEvent != "undefined" ) 
{
  window.attachEvent( "onload", initTabs );
  classAttribute = "className";
  forAttribute = "htmlFor";
}
