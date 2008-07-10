var _IE = document.all;

/*
	Table
	
	The Table object augments a standard HTML DOM table with dynamic functionality. 
	
	createTable(htmlTable, options, columns, items)
		accepts an HTML DOM table element as input and augments it with properties and methods
		that allow it to function as a dynamically generated table.  It binds the table to an array of items, such that additions
		or removals from the items array automatically update the table.
	
		It is assumed that the HTML table initially contains exactly one header row and, if "checkBoxes" option is specified (see options
		below), then exactly one column (the leftmost column) reserved for checkboxes.
		The table must conform to this layout or the Table class will not function correctly.
		The Table class does not modify the header row.
	
			htmlTable - the DOM TABLE element to augment.
			
			options - an object specifying options for the table.
				The options object has the following properties:
					editInPlace (bool) - indicates whether to create an edit-in-place style table, vs a read-only table.
					flow (bool) - true: the logical columns will be flowed inside of a single TD. false: each logical column given its own TD.
					checkBoxes (bool) - inidicates whether the first column of the table should be a checkbox column.
					
			columns - an array of column objects that maps properties of an item to columns of the table. 
				For an non edit-in-place table, this may simply be an array of strings. Each string will be treated as a property
				of an item, and the cell will be populated with the value of that property.
				
				For an edit-in-place table, the column object must be a complex object with the following properties:
					getValue: function(item) - a function that returns the value of the cell for the specified item
					setValue: function(item, value) - a function that sets the value of the item from the cell value
					getError: function(item) - a function that returns a validation error message to display in the cell,
						or null if the item is valid
					getVisible: function(item) - a function that returns a boolean indicating whether the cell contents
						should be visible or not. This function is called whenever an update occurs in the row, allowing for
						visibility of adjacent cells to be controlled dynamically.
					cellType: a string indicating the type of cell (e.g. "text", "choice", "checkbox", "lookup", "datetime", "bool" and others...)
						*note: this may also be populated with a custom value (e.g. "myCellType"), in which case you must handle
						 the renderCell event and do custom rendering.
					choices: an array of strings - used only by the "choice" cell type. In addition to strings, the array
						may contain objects with the properties 'group' and 'choices'.  In this case, the object represents
						a group of choices (HTML optgroup) - 'group' is the name of the group, and 'choices' is an array
						specifying the choices within that group.  The 'choices' array is processed recursively, allowing
						for a hierarchical structure of choices of abitrary depth.
					lookup: function(query) - used only the the "lookup" cell type - returns the result of the query, or null if not found
					size: the size of the column, in characters
						 
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
		htmlTable._options = options;

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
	
	// defines the methods that will mix-in to the DOM table object
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
			{
				this._checkBoxes[rowIndex].checked = checked;
			}
		},
		
		updateValidation: function()
		{
			for(var i=1; i < this.rows.length; i++) // skip header row
				this._validateRow(i);
		},
		
		_validateRow: function(rowIndex)
		{
			// validate each column
			var obj = this.items[rowIndex-1];
			for(var c=0; c < this._columns.length; c++)
			{
				var column = this._columns[c];
				if(column.getError)
				{
					// first see if the column is visible - don't validate invisible columns
					var visible = column.getVisible ? column.getVisible(obj) : true;
					var error = visible ? column.getError(obj) : null;
					this.errorProvider.setError(this.rows[rowIndex]._errorElements[c], error);
				}
			}

			// fire the validateItem event, which validates the item as a whole
			if(this.validateItem)
			{
				var args = {item: obj, error: ""};
				this.validateItem(this, args);
				this.errorProvider.setError(this._checkBoxes[rowIndex], args.error);
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
			
			if(this._options.checkBoxes)
			{
				// add checkbox cell at start of row
				var td = tr.insertCell(0);
				var checkBox = document.createElement("input");
				checkBox.type = "checkbox";
			  td.className = "rowCheckCell";
				td.appendChild(checkBox);
				this._checkBoxes[index] = checkBox;

				// add errorProvider image next to checkbox
				this.errorProvider.setError(checkBox, "");
			}
			
			var containerCell;  // used by "flow" style
			for(var i=0; i < this._columns.length; i++)
			{
				var cell = null;

			   if(this._options.flow)
			   {
					// add one containerCell to the table, and flow each of the "columns" inside of it
				   containerCell = containerCell || tr.insertCell(this._getBaseColumnIndex());
				   containerCell.className = "containerCell";
				   
				   // the cell is not technically a cell in this case, but rather a div
				   cell = document.createElement("div");
				   containerCell.appendChild(cell);
				   cell.className = "divCell";
				   cell.innerHTML = this._columns[i].label + "<br>";
			   }
			   else
			   {
				  // add one cell for each column, offset by 1 if there is a checkbox column
					cell = tr.insertCell(i + this._getBaseColumnIndex());
			   }
				 
			  this._renderCell(index, i, cell, obj);

			  // set cell error provider if the column has an error function
			  if(this._columns[i].getError)
			  {
				  var errorElement = cell.lastChild;  // the HTML element where the error will be shown
				  this.errorProvider.setError(errorElement, "");
				  
				  // cache the errorElement in an array in the TR, so we can reference it later
				  tr._errorElements = tr._errorElements || [];
				  tr._errorElements[i] = errorElement;
			  }
			}
			
			this._validateRow(index);
		   
		},
	
		_renderCell: function(row, col, td, obj)
		{
			// by default, set cell content to the value of the specified property of the object
			var column = this._columns[col];
			var value = this._getColumnValue(column, obj);
			if (column.cellType == "html")
			{
				td.innerHTML = value;
			}
			else if (column.cellType == "readonly")
			{
				Field.setPreFormattedValue(td, value);
			}
			else
			{
				Field.setValue(td, value);
			}

			// fire custom formatting event, which may itself set the innerHTML property to override default cell content
			if(this.renderCell)
				this.renderCell(this, { htmlCell: td, column: this._columns[col], item: obj, itemIndex: row-1, colIndex: col });
		},
		
		// returns the HTML DOM element that represents the "cell" of the table
		_getCell : function(rowIndex, colIndex)
		{
			var tr = this.rows[rowIndex];
			var cell;
			if(this._options.flow)
			{
				// get the container cell, and navigate through the children to the correct element
				var containerCell = tr.cells[this._getBaseColumnIndex()];
				for(var i=0, cell = containerCell.firstChild; i < colIndex; i++, cell = cell.nextSibling);
			}
			else
			{
				cell = tr.cells[colIndex + this._getBaseColumnIndex()];
			}
			return cell;
		},
		
		// returns either 0, if this table does not have a check-box column, or 1 if it does have a check-box column
		_getBaseColumnIndex: function()
		{
			return this._options.checkBoxes ? 1 : 0;
		},
		
		_getColumnValue: function(column, obj)
		{
			// if column is a string, treat it as an immediate property of the object
			// otherwise, assume column is a complex object, and look for a getValue function
			return (typeof(column) == "string") ? obj[column] : ((column.getValue) ? column.getValue(obj) : null);
		},
		
		_removeRow: function(index)
		{
			// remove any error providers for this row
			var row = this.rows[index];
			var errorProvider = this.errorProvider;
			row._errorElements.each(function(element) { errorProvider.remove(element); });
			
			// remove the row and row checkbox
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
			var value = this._getColumnValue(column, obj);
			var table = this;

			if(["readonly"].indexOf(column.cellType) > -1)
			{
				var field = document.createElement("div");
				field.className = "readonlyField";
				td.appendChild(field);
				td._setCellDisplayValue = function(value) { Field.setPreFormattedValue(field, value); }
			}
			else
			if(["text"].indexOf(column.cellType) > -1)
			{
				var input = document.createElement("input");
				td.appendChild(input);
				td._setCellDisplayValue = function(value) { input.value = (value || ""); }
				if(column.size) input.size = column.size;
				
				// respond to every keystroke
				input.onkeyup = input.onchange = function() { column.setValue(obj, this.value); table._onCellUpdate(row, col); }
				
				// consider the edit complete when focus is lost
				input.onblur = function() { table._onEditComplete(row, col); }
			}
			else
			if(["textarea"].indexOf(column.cellType) > -1)
			{
				var input = document.createElement("textarea");
				td.appendChild(input);
				  td.style.height = "100%";  // overwrite default styles heights so whole text area is visible
				td._setCellDisplayValue = function(value) { input.value = (value || ""); }
				if(column.cols) input.cols = column.cols;
				  if(column.rows) input.rows = column.rows;
				  if(column.readOnly) input.readOnly = column.readOnly;
				
				// respond to every keystroke
				input.onkeyup = input.onchange = function() { column.setValue(obj, this.value); table._onCellUpdate(row, col); }
				
				// consider the edit complete when focus is lost
				input.onblur = function() { table._onEditComplete(row, col); }
			}
			else
			if(["date", "time", "datetime"].indexOf(column.cellType) > -1)
			{
				if(["date", "datetime"].indexOf(column.cellType) > -1)
				{
					var inputDate = document.createElement("input");
					inputDate.id = this.id + "_" + column.label + "_" + "dateinput" + row;
					td.appendChild(inputDate);
					if(column.size) inputDate.size = column.size;
					inputDate.readOnly = "readonly";

					// launch calendar on click
					var findButtonDate = document.createElement("span");
					findButtonDate.innerText = "     ";
					findButtonDate.className = "datePickerButton";
					td.appendChild(findButtonDate);
					findButtonDate.onclick = function() 
					{
						showDatePicker(findButtonDate, inputDate, 
							function(date) 
							{
								var extendedDate = column.getValue(obj) || new Date();
								extendedDate.setDate(date.getDate());
								extendedDate.setMonth(date.getMonth());
								extendedDate.setYear(date.getYear());
								column.setValue(obj, extendedDate);
								table._onCellUpdate(row, col);
								table._onEditComplete(row, col);
							});
					}
				}

				if(["time", "datetime"].indexOf(column.cellType) > -1)
				{
					var inputTime = document.createElement("input");
					inputTime.id = this.id + "_" + column.label + "_" + "timeinput" + row;
					td.appendChild(inputTime);
					if(column.size) inputTime.size = column.size;

					inputTime.onkeyup = function()
					{
						// if user blanked out the field, reset time
						if(!this.value || this.value.length == 0)
						{
							var extendedDate = column.getValue(obj) || new Date();
							extendedDate.setHours(0);  
							extendedDate.setMinutes(0);  
							column.setValue(obj, extendedDate);
						}
						table._onCellUpdate(row, col);
					}
					
					// consider the edit complete when focus is lost
					inputTime.onblur = function() 
					{ 
						var date = sstp_validateTimePicker(inputTime);
						if(date)
						{
							var extendedDate = column.getValue(obj) || new Date();
							extendedDate.setHours(date.getHours());  
							extendedDate.setMinutes(date.getMinutes());  
							column.setValue(obj, extendedDate);
						}
						table._onEditComplete(row, col); 
					}

					// launch calendar on click
					var findButtonTime = document.createElement("input");
					findButtonTime.type = "button";
					findButtonTime.value = "...";
					td.appendChild(findButtonTime);
					findButtonTime.onclick = function() 
					{ 
						showTimePicker(findButtonTime, inputTime,
							function(time)
							{
								var extendedDate = column.getValue(obj) || new Date();
								extendedDate.setHours(time.getHours());  // Ensure Date object has extensions defined in jsx.js
								extendedDate.setMinutes(time.getMinutes());  // Ensure Date object has extensions defined in jsx.js
								column.setValue(obj, extendedDate);
								table._onCellUpdate(row, col);
								table._onEditComplete(row, col);
							}); 
					}
				}
				
				if(["date"].indexOf(column.cellType) > -1)
				{
					td._setCellDisplayValue = function(value) { inputDate.value = value ? Ris.formatDate(value) : ""; };
				}
				if(["time"].indexOf(column.cellType) > -1)
				{
					td._setCellDisplayValue = function(value) { inputTime.value = value ? Ris.formatTime(value) : ""; };
				}
				if(["datetime"].indexOf(column.cellType) > -1)
				{
					td._setCellDisplayValue = function(value) 
					{ 
						inputDate.value = value ? Ris.formatDate(value) : "";
						inputTime.value = value ? Ris.formatTime(value) : ""; 
					};
				}
			}
			else
			if(["choice", "combobox", "dropdown", "enum", "list", "listbox"].indexOf(column.cellType) > -1)
			{
				// define a function to populate the dropdown
				function addOptions(parent, items)
				{
					items.each(
						function(item)
						{
							if(typeof(item) == "string")
							{
								var option = document.createElement("option");
								option.value = item;
								option.innerHTML = item.escapeHTML();
								parent.appendChild(option);
							}
							else if(item.group) 
							{
								var group = document.createElement("optgroup");
								group.label = item.group;
								parent.appendChild(group);
								addOptions(group, item.choices);
							}
						});
				}
			
				var input = document.createElement("select");
				td.appendChild(input);
				td._setCellDisplayValue = function(value) { input.value = (value || ""); }
				if(column.size) input.style.width = column.size + "pc"; // set width in chars
				
				// choices may be an array, or a function that returns an array
				var choices = (typeof(column.choices) == "function") ? column.choices(obj) : column.choices;
				addOptions(input, choices);
				
				input.onchange = function()
				{
					column.setValue(obj, (this.value && this.value.length)? this.value : null);
					table._onCellUpdate(row, col);
					// for a combo box, the edit is completed as soon as the selection changes
					table._onEditComplete(row, col); 
				}
				
			}
			else
			if(["check","checkbox","bool","boolean"].indexOf(column.cellType) > -1)
			{
				  td.className = "checkedDivCell";

				  // Replace top-level label text with an actual label element
				  // This allows the check-box to be activated while clicking on the text in addition to the box
				  // <div>label text<br></div> will ultimately become <div><label><input type="checkbox">label text</label></div>
				  
				  var label = document.createElement("label");
				  td.appendChild(label);

				  var input = document.createElement("input");
				input.type = "checkbox";
				  label.appendChild(input);
				  
				  if(column.readonly)
				  {
					input.disabled = "disabled";
					}

				  // move the "label text" from the div/cell to the label element
				  var text = td.removeChild(td.firstChild);
				  label.appendChild(text);

				  // get rid of useless <br>
				  if(td.firstChild) td.removeChild(td.firstChild);  // get rid of <br>

				td._setCellDisplayValue = function(value) { input.checked = value ? true : false; }
				if(column.size) input.size = column.size;
				
				input.onclick = input.onchange = function()
				{
					column.setValue(obj, this.checked ? true : false);
					table._onCellUpdate(row, col);
					// for a check box, the edit is completed as soon as the click happens
					table._onEditComplete(row, col); 
				}
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
 						//input.value = column.getValue(obj) || "";
						table._onCellUpdate(row, col);
						table._onEditComplete(row, col);
					}
				}
				
				var input = document.createElement("input");
				td.appendChild(input);
				td._setCellDisplayValue = function(value) { input.value = (value || ""); }
				if(column.size) input.size = column.size;
				
				input.onkeyup = function()
				{
					if(event.keyCode == 13) // pressing ENTER key executes lookup
						doLookup();
					else
					{
						// any manual edit clears the underlying item
						column.setValue(obj, null);
						table._onCellUpdate(row, col);
					}
				}				
				
				var findButton = document.createElement("span");
				findButton.className = "lookupButton";
				findButton.innerText = "    ";
				findButton.visibility = "hidden";
				findButton.onclick = doLookup;
				td.appendChild(findButton);

				// consider the edit complete when focus is lost
				// JR: actually this doesn't work because it blanks the text box when focus moves to the find button
				//input.onblur = function() { table._onEditComplete(row, col); }
			}
			else
			if (column.cellType == "html")
			{
			}
			
			
			// initialize the cell value
			td._setCellDisplayValue(value);
			
			// initialize the cell visibility
			//td.style.visibility = (column.getVisible ? column.getVisible(obj) : true) ? "visible" : "hidden";
		  td.style.display = (column.getVisible ? column.getVisible(obj) : true) ? "block" : "none";
			
			// fire custom formatting event, which may itself set the innerHTML property to override default cell content
			if(this.renderCell)
				this.renderCell(this, { htmlCell: td, column: this._columns[col], item: obj, itemIndex: row-1, colIndex: col });
		},
		
		// called when 
		_onCellUpdate: function(row, col)
		{
			// update validation on the fly as the user types, rather than wait for the edit to complete
			this._validateRow(row);
		},
		
		_onEditComplete: function(rowIndex, colIndex)
		{
			var item = this.items[rowIndex-1];
		   for(var c=0; c < this._columns.length; c++)
			{
				var column = this._columns[c];
				var cell = this._getCell(rowIndex, c);
				
				// update the cell's visibility
				if(column.getVisible)
				{
					visible = column.getVisible(item);
					if(visible)
						cell.style.display = "block";
					else
					{
						// when a cell is hidden, set its value to null
						// this is because we don't want any information to exist on the form
						// and be hidden from the user
						cell.style.display = "none";
						column.setValue(item, null);
					}
				}
				
				// update the cell's display value from the item
				cell._setCellDisplayValue(column.getValue(item));
			}
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
				img:document.createElement("span"),
				hasError: function() { return (this.img.title && this.img.title.length) ? true : false; },
				showError: function(visible) 
				{ 
					this.img.style.display = (this.hasError() && visible) ? "inline" : "none"; 
					this.img.innerText = "    ";
					this.img.visiblity = "hidden";
				} 
			};
			this._providers.add( provider );
			
			insertAfter(provider.img, htmlElement);
		}
		provider.img.title = message || "";
		provider.img.className = "errorProvider";
		provider.showError(this._visible);
	}
	
	this.remove = function(htmlElement)
	{
		// see if there is a provider for this element
		var provider = this._providers.find(function(v) { return v.element == htmlElement; });   
		if(provider)
			this._providers.remove(provider);
	}
	
	this.hasErrors = function()
	{
		return this._providers.find(function(provider) { return provider.hasError(); }) ? true : false;
	}
	
	this.setVisible = function(visible)
	{
		this._visible = visible ? true : false;
		this._providers.each(
			function(provider)
			{
			   provider.showError(visible);
			});
	}
}

function insertAfter(newElement,targetElement)
{
	//target is what you want it to go after. Look for this elements parent.
	var parent = targetElement.parentNode;

	//if the parents lastchild is the targetElement...
	if(parent.lastchild == targetElement) {
		//add the newElement after the target element.
		parent.appendChild(newElement);
	} else {
		// else the target has siblings, insert the new element between the target and it's next sibling.
		parent.insertBefore(newElement, targetElement.nextSibling);
	}
}



var Field = 
{
	setValue: function(element, value)
	{
		element.innerHTML = value ? (value + "").escapeHTML() : "";
	},

	setPreFormattedValue: function(element, value)
	{
		element.innerHTML = value ? (value + "").replaceLineBreak() : "";
	},

	getValue: function(element)
	{
		return element.innerHTML;
	},
	
	show: function(element, state)
	{
		element.style.display = state ? "" : "none";
	},

	disabled: function(element, state)
	{
		element.disabled = state ? "disabled" : "";
	},
	
	readOnly: function(element, state)
	{
		element.readOnly = state;
	},
	
	setHeight: function(element, height)
	{
		element.style.height = height;
	},
	
	setWidth: function(element, width)
	{
		element.style.width = width;
	}
};

