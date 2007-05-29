/*
    Table
    
    The Table class wraps an HTML DOM table element so that it functions as a dynamically generated table.  It binds the
    table to an array of items, such that additions or removals from the items array automatically update the table.
    
    It is assumed that the HTML table contains exactly one header row and exactly one column (the leftmost column) reserved
    for checkboxes, and the table must conform to this layout or the Table class will not function correctly.
    The Table class does not touch the header row.
    
    Constructor:
        Table(htmlTable, items, propNames)
            htmlTable - the DOM TABLE element that this Table will bind to
            propNames - an array of property names which maps properties of the items to columns of the table
            items (optional) - the array of items that this table will bind to
            
    Properties:
        items - the array of items to which the table is bound. Do not set this property - use bindItems() method instead.
        rowCycleClassNames - an array of CSS class names that will be applied cyclically to rows as they are added
           
    Methods:      
        bindItems(items)
            items - an array of items that this table will bind to
              
        getCheckedItems()
            returns an array containing the items that are currently checked
            
        setItemCheckState(item, checked)
            item - the item whose check state to set
            checked - boolean check state
        
    Events:
        formatRow - fired when a new row is added to the table.
            The event object contains the properties:   htmlRow - the DOM TR element
                                                        rowIndex - the index of the row added
                                                        item - the item that this row represents
                                                        
        formatCell - fired when a new cell is added to the table.
            The event object contains the properties:   htmlCell - the DOM TD element                                        
                                                        rowIndex - the row index of the cell
                                                        colIndex - the col index of the cell
                                                        item - the item that this row represents
                                                        propertyName - the property of the item that the column holds
    
    
*/
function Table(htmlTable, propNames, items)
{
	this.htmlTable = htmlTable;
	this.propNames = propNames || [];
	this.checkBoxes = [];
	this.rowCycleClassNames = [];
	
	this.bindItems = function(items)
	{
	    this._removeAllRows();
	    
	    this.items = items;
	    
	    // bind to events on the items array
	    var table = this;
	    this.items.itemAdded = function(sender, args) { table._addRow(args.item); }
	    this.items.itemRemoved = function(sender, args) { table._removeRow(args.index+1); }
    	
	    // init table with items array
	    this.items.each(function(item) { table._addRow(item); });
	    
	}
	this.getCheckedItems = function()
	{
		var result = [];
		for(var i=0; i < this.htmlTable.rows.length; i++)
		{
			if(this.checkBoxes[i] && this.checkBoxes[i].checked)
				result.add(this.items[i-1]);
		}
		return result;
	}
	this.setItemCheckState = function(item, checked)
	{
	    var rowIndex = this.items.indexOf(item) + 1;
	    if(rowIndex > 0)
	        this.checkBoxes[rowIndex].checked = checked;
	}
	this._addRow = function(obj)
	{
		var index = this.htmlTable.rows.length;
		var tr = this.htmlTable.insertRow(index);
		
		// apply row cyclic css class to row
		if(this.rowCycleClassNames && this.rowCycleClassNames.length > 0)
		    tr.className = this.rowCycleClassNames[(index-1)%(this.rowCycleClassNames.length)];
		    
		// fire custom formatting event    
		if(this.formatRow)
		    this.formatRow(this, { htmlRow: tr, rowIndex: index-1, item: obj });
		
		// add checkbox cell at start of row
		var td = tr.insertCell(0);
		var checkBox = document.createElement("input");
		checkBox.type = "checkbox";
		td.appendChild(checkBox);
		this.checkBoxes[index] = checkBox;
		
		// add other cells
		for(var i=0; i < this.propNames.length; i++)
		{
			td = tr.insertCell(i+1);
			// by default, set cell content to the value of the specified property of the object
			td.innerHTML = ((obj[this.propNames[i]] || "")+"").escapeHTML();
			// fire custom formatting event, which may itself set the innerHTML property to override default cell content
			if(this.formatCell)
			    this.formatCell(this, { htmlCell: td, propertyName: this.propNames[i], item: obj, rowIndex: index-1, colIndex: i });
		}
	}
	this._removeRow = function(index)
	{
		this.htmlTable.deleteRow(index);
		this.checkBoxes.removeAt(index);
	}
	this._removeAllRows = function()
	{
	    for(var i=this.htmlTable.rows.length-1; i > 0; i--)
	        this._removeRow(i);
	}
	
	// bind this table to items array
	this.bindItems(items || []);
}

function Validation()
{
    this._validators = [];
    
    this.addValidator = function(htmlElement, errorFunc)
    {
        var validator = { img: document.createElement("img"), div: document.createElement("div"), errorCallback: errorFunc };
        validator.img.src = "error.gif";
        validator.img.alt = "Error";
        validator.div.style.visibility = "hidden";
        
        validator.div.appendChild(validator.img);
        htmlElement.parentNode.insertBefore(validator.div, htmlElement);
        
        this._validators.add( validator );
    }
    
    this.hasErrors = function()
    {
        return this._validators.select(
            function(validator) {
                var message = validator.errorCallback();
                return message && message.length > 0;
            }).length > 0;
    }
    
    this.showErrors = function(show)
    {
        this._validators.each(
            function(validator)
            {
                if(show)
                {
                    var message = validator.errorCallback();
                    if(message && message.length > 0)
                    {
                        validator.img.alt = message;
                        validator.div.style.visibility = "visible";
                    }
                    else
                        validator.div.style.visibility = "hidden";
                }
                else
                    validator.div.style.visibility = "hidden";
            });
    }
}