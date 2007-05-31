/*
    Table
    
    The Table object augments a standard HTML DOM table with dynamic functionality. 
    
    createTable(htmlTable, options, columns, items)
        accepts an HTML DOM table element as input and augments it with properties and methods
        that allow it to function as a dynamically generated table.  It binds the table to an array of items, such that additions
        or removals from the items array automatically update the table.
    
        It is assumed that the HTML table initially contains exactly one header row and exactly one column (the leftmost column)
        reserved for checkboxes, and the table must conform to this layout or the Table class will not function correctly.
        The Table class does not modify the header row.
    
            htmlTable - the DOM TABLE element to augment.
            
            options - an object specifying options for the table.
                The options object has the following properties:
                    editInPlace (bool) - indicates whether to create an edit-in-place style table, vs a read-only table.
                    
            columns - an array of column objects that maps properties of an item to columns of the table. 
                For an non edit-in-place table, this may simply be an array of strings. Each string will be treated as a property
                of an item, and the cell will be populated with the value of that property.
                
                For an edit-in-place table, the column object must be a complex object with the following properties:
                    getValue: function(item) - a function that returns the value of the cell for the specified item
                    setValue: function(item, value) - a function that sets the value of the item from the cell value
                    cellType: a string indicating the type of cell (e.g. "text", "combo", "checkbox", "lookup" and others...)
                        *note: this may also be populated with a custom value (e.g. "myCellType"), in which case you must handle
                         the renderCell event and do custom rendering.
                    choices: an array of strings - used only by the "combo" cell type
                    lookup: function(query) - used only the the "lookup" cell type - returns the result of the query, or null if not found
                         
            items (optional) - the array of items that this table will bind to. See also the bindItems() method.
            
        createTable returns the htmlTable element with the following properties, methods, and events added to it:
                
        Properties:
            items - the array of items to which the table is bound. Do not set this property - use bindItems() method instead.
            rowCycleClassNames - an array of CSS class names that will be applied cyclically to rows as they are added
            errorProvider - the ErrorProvider object used by the table. You may set this property to your own ErrorProvider instance
                (e.g. so that the table shares the same ErrorProvider object as the rest of the page)
                  
        Methods:      
            bindItems(items)
                items - an array of items that this table will bind to
                  
            getCheckedItems()
                returns an array containing the items that are currently checked
                
            setItemCheckState(item, checked)
                item - the item whose check state to set
                checked - boolean check state
                
            updateValidation()
                refreshes the validation state of the table - typically this only needs to be called from a custom
                rendering event handler
            
        Events:
            renderRow - fired when a new row is added to the table.
                To attach an event handler, use the following syntax: table.renderRow = function(sender, args) {...}
                The args object contains the properties:   htmlRow - the DOM TR element
                                                            rowIndex - the index of the row added
                                                            item - the item that this row represents
                                                            
            renderCell - fired when a new cell is added to the table.
                To attach an event handler, use the following syntax: table.renderCell = function(sender, args) {...}
                The args object contains the properties:   htmlCell - the DOM TD element                                        
                                                            rowIndex - the row index of the cell
                                                            colIndex - the col index of the cell
                                                            item - the item that this row represents
                                                            column - the column object to which this cell is mapped
                                                            
            validateItem - fired to validate an item in the table.
                To attach an event handler, use the following syntax: table.validateItem = function(sender, args) {...}
                The args object contains the properties:   item - the item to be validated
                                                           error - the handler should set this to a string error message
                                                            if there is an error to report
*/

var Table = {


    createTable: function(htmlTable, options, columns, items)
    {
        htmlTable._columns = columns;
        htmlTable._checkBoxes = [];
        htmlTable.errorProvider = new ErrorProvider();
        htmlTable.rowCycleClassNames = [];

        // mix in methods
        for(var prop in this._tableMixIn)
            htmlTable[prop] = this._tableMixIn[prop];
        if(options.editInPlace)
        {
            for(var prop in this._editableTableMixIn)
                htmlTable[prop] = this._editableTableMixIn[prop];
        }
        
        // do initial binding if supplied   
        if(items)
            htmlTable.bindItems(items);
            
        return htmlTable;
    },
    
    // defines the methods that DHTML will mix-in to the DOM table object
    _tableMixIn : {
    	
    	
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
    	    
	        // validate items
	        this.updateValidation();
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
    	
	    updateValidation: function()
	    {
            for(var i=1; i < this.rows.length; i++) // skip header row
            {
                if(this.validateItem)
                {
                    var args = {item: this.items[i-1], error: ""};
                    this.validateItem(this, args);
                    this.errorProvider.setError(this._checkBoxes[i], args.error);
                }
            }
	    },
    	
	    _addRow: function(obj)
	    {
		    var index = this.rows.length;
		    var tr = this.insertRow(index);
    		
		    // apply row cyclic css class to row
		    if(this.rowCycleClassNames && this.rowCycleClassNames.length > 0)
		        tr.className = this.rowCycleClassNames[(index-1)%(this.rowCycleClassNames.length)];
    		    
		    // fire custom formatting event    
		    if(this.renderRow)
		        this.renderRow(this, { htmlRow: tr, rowIndex: index-1, item: obj });
    		
		    // add checkbox cell at start of row
		    var td = tr.insertCell(0);
		    var checkBox = document.createElement("input");
		    checkBox.type = "checkbox";
		    td.appendChild(checkBox);
		    this._checkBoxes[index] = checkBox;
    		
		    // add errorProvider image next to checkbox
		    this.errorProvider.setError(checkBox, "");
    		
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
            var column = this._columns[col];
            var value = this._getCellValue(column, obj);
		    td.innerHTML = ((value || "")+"").escapeHTML();
    		
		    // fire custom formatting event, which may itself set the innerHTML property to override default cell content
		    if(this.renderCell)
		        this.renderCell(this, { htmlCell: td, column: this._columns[col], item: obj, rowIndex: row, colIndex: col });
	    },
    	
	    _getCellValue: function(column, obj)
	    {
		    // if column is a string, treat it as an immediate property of the object
		    // otherwise, assume column is a complex object, and look for a getValue function
            return (typeof(column) == "string") ? obj[column] : ((column.getValue) ? column.getValue(obj) : null);
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
    },
    
    // defines the methods that will be mixed to an "edit-in-place" style table
    _editableTableMixIn: {
        // override the _renderCell method from _tableMixIn
	    _renderCell: function(row, col, td, obj)
	    {
	        var column = this._columns[col];
		    var value = this._getCellValue(column, obj);
		    var table = this;
		    if(column.cellType == "text")
		    {
		        var input = document.createElement("input");
		        td.appendChild(input);
		        input.value = value || "";
		        input.onkeyup = function() { column.setValue(obj, this.value); table.updateValidation(); }
		    }
		    else
		    if(["choice", "combo", "dropdown", "enum", "list", "listbox"].indexOf(column.cellType) > -1)
		    {
		        var input = document.createElement("select");
		        td.appendChild(input);
		        column.choices.each(
		            function(choice)
		            {
		                var option = document.createElement("option");
		                option.value = choice;
		                option.innerHTML = choice.escapeHTML();
		                input.appendChild(option);
		            });
		        input.value = value || "";
		        input.onchange = function() { column.setValue(obj, this.value); table.updateValidation(); }
		    }
		    else
		    if(["check","checkbox","bool","boolean"].indexOf(column.cellType) > -1)
		    {
		        var input = document.createElement("input");
		        input.type = "checkbox";
		        td.appendChild(input);
		        input.checked = value ? true : false;
		        input.onclick = input.onchange = function() { column.setValue(obj, this.checked ? true : false); table.updateValidation(); }
		    }
		    else
		    if(column.cellType == "lookup")
		    {
		        // define a helper to do the lookup
                function doLookup()
                {
                    var result = column.lookup(input.value || "");
                    if(result)
                    {
                        column.setValue(obj, result);
 		                input.value = column.getValue(obj) || "";
                        table.updateValidation();
                    }
                }
                
                var input = document.createElement("input");
                td.appendChild(input);
		        input.value = value || "";
                input.onkeyup = function()
                {
                    if(event.keyCode == 13) // pressing ENTER key executes lookup
                        doLookup();
                    else
                    {
                        // any manual edit clears the underlying item
                        column.setValue(obj, null);
                        table.updateValidation();
                    }
                }
                
                var findButton = document.createElement("input");
                findButton.type = "button";
                findButton.value = "Find";
                td.appendChild(findButton);
                findButton.onclick = doLookup;
                 
		    }
    		
		    // fire custom formatting event, which may itself set the innerHTML property to override default cell content
		    if(this.renderCell)
		        this.renderCell(this, { htmlCell: td, column: this._columns[col], item: obj, rowIndex: row, colIndex: col });
	    }
    }
};

/*
    ErrorProvider
    
    The ErrorProvider class provides the ability to show an error icon and error message next to any HTML DOM element.
    It is analogous to the WinForms ErrorProvider class.
    
    Constructor
    
    ErrorProvider(visible) - constructs an instance of a ErrorProvider object, specifying whether errors
        are initially visible on the page.
    
    
    Methods
    
    setError(htmlElement, message)
        htmlElement - the element to set the error for
        message - the error message
        
    hasErrors()
        returns true if any htmlElement has a non-null error message
        
    setVisible(visible)
        visible - specifies whether errors should be visible or hidden
*/
function ErrorProvider(visible)
{
    this._providers = [];
    this._visible = visible ? true : false;
    
    this.setError = function(htmlElement, message)
    {
        // see if there is already a provider for this element
        var provider = this._providers.find(function(v) { return v.element == htmlElement; });   
        
        // if not, create one
        if(!provider)
        {
            provider = { 
                element: htmlElement,
                img: document.createElement("img"),
                hasError: function() { return (this.img.alt && this.img.alt.length); }
                };
            provider.img.src = "errorprovider.gif";
            this._providers.add( provider );
            
            htmlElement.parentNode.insertBefore(provider.img, htmlElement.nextSibling);       
        }
        provider.img.alt = message;
        provider.img.style.visibility = (provider.hasError() && this._visible) ? "visible" : "hidden";
    }
    
    this.hasErrors = function()
    {
        return this._providers.find(function(provider) { return provider.hasError(); }) ? true : false;
    }
    
    this.setVisible = function(visible)
    {
        this._visible = visible;
        this._providers.each(
            function(provider)
            {
                provider.img.style.visibility = (visible && provider.hasError()) ? "visible" : "hidden";
            });
    }
}
