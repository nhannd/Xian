/*
    DynamicTable
    
    The DynamicTable.createTable method accepts an HTML DOM table element as input and augments it with properties and methods
    that allow it to function as a dynamically generated table.  It binds the table to an array of items, such that additions
    or removals from the items array automatically update the table.
    
    It is assumed that the HTML table contains exactly one header row and exactly one column (the leftmost column) reserved
    for checkboxes, and the table must conform to this layout or the Table class will not function correctly.
    The Table class does not touch the header row.
    
    DynamicTable.createTable(htmlTable, items, propNames)
            htmlTable - the DOM TABLE element to augment
            columns - an array of column objects that maps properties of the items to columns of the table. A column object
                has the following properties:
                    property - the name of the property on the item to which this column maps
            items (optional) - the array of items that this table will bind to
            
    DynamicTable.createTable returns the htmlTable element with the following properties, methods, and events added to it:
            
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
            To attach an event handler, use the following syntax: table.formatRow = function(sender, args) {...}
            The args object contains the properties:   htmlRow - the DOM TR element
                                                        rowIndex - the index of the row added
                                                        item - the item that this row represents
                                                        
        formatCell - fired when a new cell is added to the table.
            To attach an event handler, use the following syntax: table.formatCell = function(sender, args) {...}
            The args object contains the properties:   htmlCell - the DOM TD element                                        
                                                        rowIndex - the row index of the cell
                                                        colIndex - the col index of the cell
                                                        item - the item that this row represents
                                                        column - the column object to which this cell is mapped
    
    
*/

var DynamicTable = {
    createTable: function(htmlTable, columns, items)
    {
        htmlTable._columns = columns;
        htmlTable._checkBoxes = [];
        htmlTable.rowCycleClassNames = [];

        for(var prop in TableMixIn)
            htmlTable[prop] = TableMixIn[prop];
            
        if(items)
            htmlTable.bindItems(items);
            
        return htmlTable;
    }
};

// defines the methods that DynamicTable will mix-in to the DOM table object
var TableMixIn = {
	
	
	bindItems: function(items)
	{
	    this._removeAllRows();
	    
	    this.items = items;
	    
	    // bind to events on the items array
	    var table = this;
	    this.items.itemAdded = function(sender, args) { table._addRow(args.item); }
	    this.items.itemRemoved = function(sender, args) { table._removeRow(args.index+1); }
    	
	    // init table with items array
	    this.items.each(function(item) { table._addRow(item); });
	    
	},
	
	getCheckedItems: function()
	{
		var result = [];
		for(var i=0; i < this.rows.length; i++)
		{
			if(this._checkBoxes[i] && this._checkBoxes[i].checked)
				result.add(this.items[i-1]);
		}
		return result;
	},
	
	setItemCheckState: function(item, checked)
	{
	    var rowIndex = this.items.indexOf(item) + 1;
	    if(rowIndex > 0)
	        this._checkBoxes[rowIndex].checked = checked;
	},
	
	_addRow: function(obj)
	{
		var index = this.rows.length;
		var tr = this.insertRow(index);
		
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
		this._checkBoxes[index] = checkBox;
		
		// add other cells
		for(var i=0; i < this._columns.length; i++)
		{
			td = tr.insertCell(i+1);
			this._renderCell(index+1, i, td, obj);
		}
	},
	
	_renderCell: function(row, col, td, obj)
	{
		// by default, set cell content to the value of the specified property of the object
		td.innerHTML = ((obj[this._columns[col].property] || "")+"").escapeHTML();
		// fire custom formatting event, which may itself set the innerHTML property to override default cell content
		if(this.formatCell)
		    this.formatCell(this, { htmlCell: td, column: this._columns[col], item: obj, rowIndex: row, colIndex: col });
	},
	
	_removeRow: function(index)
	{
		this.deleteRow(index);
		this._checkBoxes.removeAt(index);
	},
	
	_removeAllRows: function()
	{
	    for(var i=this.rows.length-1; i > 0; i--)
	        this._removeRow(i);
	}
};

var EditableTableMixIn = {
	_renderCell: function(row, col, td, obj)
	{
		// by default, set cell content to the value of the specified property of the object
		td.innerHTML = ((obj[this._columns[col]] || "")+"").escapeHTML();
		// fire custom formatting event, which may itself set the innerHTML property to override default cell content
		if(this.formatCell)
		    this.formatCell(this, { htmlCell: td, propertyName: this._columns[col], item: obj, rowIndex: row, colIndex: col });
	}
};


function Validation()
{
    this._validators = [];
    
    this.show = function(htmlElement, message)
    {
        // see if there is already a validator for this element
        var validator = this._validators.find(function(v) { return v.element == htmlElement; });   
        
        // if not, create one
        if(!validator)
        {
            validator = { element: htmlElement, img: document.createElement("img") };
            validator.img.src = "warning.gif";
            validator.img.width = validator.img.height = 20;
            this._validators.add( validator );
            
            htmlElement.parentNode.insertBefore(validator.img, htmlElement.nextSibling);       
        }
        validator.img.alt = message;
        validator.img.style.visibility = (message && message.length) ? "visible" : "hidden";
    }
    
    this.hideAll = function(htmlElement, message)
    {
        this._validators.each(function(validator) {  validator.img.style.visibility = "hidden"; });
    }
}
