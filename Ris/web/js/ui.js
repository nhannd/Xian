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
					autoSelectFirstElement (bool)  - indicates whether the first row of the table should automatically be selected.  Only applicable when onRowClick is implemented
					
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
					cellType: a string indicating the type of cell (e.g. "text", "choice", "checkbox", "lookup", "datetime", "bool", "link" and others...)
						*note: this may also be populated with a custom value (e.g. "myCellType"), in which case you must handle
						 the renderCell event and do custom rendering.
					choices: an array of strings - used only by the "choice" cell type. In addition to strings, the array
						may contain objects with the properties 'group' and 'choices'.  In this case, the object represents
						a group of choices (HTML optgroup) - 'group' is the name of the group, and 'choices' is an array
						specifying the choices within that group.  The 'choices' array is processed recursively, allowing
						for a hierarchical structure of choices of abitrary depth.
					clickLink: a function that is excute if the cellType is a "link"
					lookup: function(query) - used only the the "lookup" cell type - returns the result of the query, or null if not found
					size: the size of the column, in characters
						 
			items (optional) - the array of items that this table will bind to. See also the bindItems() method.
			
		createTable returns the htmlTable element with the following properties, methods, and events added to it:
				
		Properties:
			items - the array of items to which the table is bound. Do not set this property - use bindItems() method instead.
			rowCycleClassNames - an array of CSS class names that will be applied cyclically to rows as they are added
			mouseOverClassName - a CSS class name that will be applied when mouse over a clickable row
			highlightClassName - a CSS class name that will be applied when a clickable row is selected
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
			onRowClick - fired when a row is clicked
				To attach an event handler, use the following syntax: table.onRowClick = function(sender, args) {...}
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

		if(options.addColumnHeadings && !options.flow)
		{
			var tr = htmlTable.insertRow(0);
			tr.className = "tableheading";

			var checkboxCellOffset = options.checkBoxes ? 1 : 0;
			if(options.checkBoxes)
			{
				cell = tr.insertCell(0);
			}

			var cell = null;
			for(var i=0; i < columns.length; i++)
			{
				cell = tr.insertCell(i + checkboxCellOffset);
				cell.innerHTML = columns[i].label;
			}
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

			if (this.onRowClick)
			{
				if (this._options.autoSelectFirstElement)
					this._selectRow(this.rows[1]); // skip header row
			}
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

		_indexOfRow: function(r)
		{
			for(var i=1; i < this.rows.length; i++)
			{
				if(this.rows[i] === r)
					return i;
			}
			
			return -1;
		},

		_resetRowClassName: function(r)
		{
			var index = this._indexOfRow(r);
			if (this.rowCycleClassNames && this.rowCycleClassNames.length > 0)
				r.className = this.rowCycleClassNames[(index-1)%(this.rowCycleClassNames.length)];
		},

		_selectRow: function(r)
		{
			// reset the row class name of the previously selected row
			if (this._selectedRow)
				this._resetRowClassName(this._selectedRow);

			var index = this._indexOfRow(r);
			var obj = this.items[index-1];
			this.onRowClick(this, { htmlRow: r, rowIndex: index-1, item: obj }); 
			this._selectedRow = r;
			this._selectedRow.className = this.highlightClassName;
		},

		_mouseOverRow: function(r)
		{ 
			r.style.cursor="hand"; 
			r.className = this.mouseOverClassName;
		},

		_mouseOutRow: function(r) 
		{ 
			r.style.cursor=''; 
			this._resetRowClassName(r);
			if (this._selectedRow && r == this._selectedRow)
				this._selectedRow.className = this.highlightClassName;
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

			if (this.onRowClick)
			{
				var htmlTable = this;
				tr.onmouseover = function() { htmlTable._mouseOverRow(tr); };
				tr.onmouseout = function() { htmlTable._mouseOutRow(tr); };
				tr.onclick = function() { htmlTable._selectRow(tr); };
			}

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
			else if (column.cellType == "link" && column.clickLink)
			{
				Field.setLink(td, value, function() { column.clickLink(obj); });
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
	
		_renderer :
		{
			"readonly": function(row, col, td, obj, column, table)
			{
				var field = document.createElement("div");
				field.className = "readonlyField";
				td.appendChild(field);
				td._setCellDisplayValue = function(value) { Field.setPreFormattedValue(field, value); }
			},

			"newline": function(row, col, td, obj, column, table) 
			{
				var parent = td.parentNode;
				parent.removeChild(td);

				var right  = document.createElement("div");
				right.style.clear = 'both';
				right._setCellDisplayValue = function() {};
				parent.appendChild(right);

				td = right;
				return td;
			},

			"text": function(row, col, td, obj, column, table) 
			{
				var input = document.createElement("input");
				td.appendChild(input);
				td._setCellDisplayValue = function(value) { input.value = (value === undefined || value === null) ? "" : value; }
				if(column.size) input.size = column.size;

				// respond to every keystroke
				input.onkeyup = input.onchange = function() { column.setValue(obj, this.value); table._onCellUpdate(row, col); }

				// consider the edit complete when focus is lost
				input.onblur = function() { table._onEditComplete(row, col); }

				// Overwrite IE's default styling for text boxes so that all cell-types have the same height and flow properly.
				// This cannot be done in CSS since IE ignores the "type" pseudo selector.
				input.style.padding = '2px';
				input.style.border = '1px solid #969696';
			},

			"textarea": function(row, col, td, obj, column, table)
			{
				var input = document.createElement("textarea");
				td.appendChild(input);
				td.style.height = "100%";  // overwrite default styles heights so whole text area is visible
				td._setCellDisplayValue = function(value) { input.value = (value === undefined || value === null) ? "" : value; }
				if(column.cols) input.cols = column.cols;
				if(column.rows) input.rows = column.rows;
				if(column.readOnly) input.readOnly = column.readOnly;

				// respond to every keystroke
				input.onkeyup = input.onchange = function() { column.setValue(obj, this.value); table._onCellUpdate(row, col); }

				// consider the edit complete when focus is lost
				input.onblur = function() { table._onEditComplete(row, col); }
			},

			_dateHelper: function(row, col, td, obj, column, table) 
			{
				var inputDate = document.createElement("input");
				inputDate.id = table.id + "_" + column.label + "_" + "dateinput" + row;
				td.appendChild(inputDate);
				if(column.size) inputDate.size = column.size;
				
				// Overwrite IE's default styling for text boxes so that all cell-types have the same height and flow properly.
				// This cannot be done in CSS since IE ignores the "type" pseudo selector.
				inputDate.style.padding = '2px';
				inputDate.style.border = '1px solid #969696';
				
				// consider the edit complete when focus is lost
				inputDate.onblur = function() 
				{ 
					// Just tabbing through an empty field, so don't assume a value
					if (!inputDate.value && !column.getValue(obj))
						return;
					
					var date;

					try
					{
						date = Date.parseDate(inputDate.value, Ris.getDateFormat()) || new Date();
					}
					catch(e)
					{
						date = new Date();
					}
					
					var extendedDate = column.getValue(obj) || new Date();
					extendedDate.setDate(date.getDate());
					extendedDate.setMonth(date.getMonth());
					extendedDate.setYear(date.getYear());
					column.setValue(obj, extendedDate);
					table._onEditComplete(row, col); 
				}


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

				return inputDate;
			},

			"date": function(row, col, td, obj, column, table) 
			{
				var inputDate = this._dateHelper(row, col, td, obj, column, table);
				td._setCellDisplayValue = function(value) { inputDate.value = value ? Ris.formatDate(value) : ""; };
			},

			_timeHelper: function(row, col, td, obj, column, table) 
			{
				var inputTime = document.createElement("input");
				inputTime.id = table.id + "_" + column.label + "_" + "timeinput" + row;
				td.appendChild(inputTime);
				if(column.size) inputTime.size = column.size;

				// Overwrite IE's default styling for text boxes so that all cell-types have the same height and flow properly.
				// This cannot be done in CSS since IE ignores the "type" pseudo selector.
				inputTime.style.padding = '2px';
				inputTime.style.border = '1px solid #969696';
				
				// consider the edit complete when focus is lost
				inputTime.onblur = function() 
				{ 
					// Just tabbing through an empty field, so don't assume a value
					if (!inputTime.value && !column.getValue(obj))
						return;
					
					var date;

					try
					{
						date = sstp_validateTimePicker(inputTime) || new Date();
					}
					catch(e)
					{
						date = new Date();
					}

					var extendedDate = column.getValue(obj) || new Date();
					extendedDate.setHours(date.getHours());  
					extendedDate.setMinutes(date.getMinutes()); 
					column.setValue(obj, extendedDate);
					table._onEditComplete(row, col); 
				}

				// launch calendar on click
				var findButtonTime = document.createElement("span");
				findButtonTime.innerText = "     ";
				findButtonTime.className = "timePickerButton";
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
				
				return inputTime;
			},

			"time": function(row, col, td, obj, column, table) 
			{
				var inputTime = this._timeHelper(row, col, td, obj, column, table);
				td._setCellDisplayValue = function(value) { inputTime.value = value ? Ris.formatTime(value) : ""; };
			},

			"datetime": function(row, col, td, obj, column, table)
			{
				var inputDate = this._dateHelper(row, col, td, obj, column, table);
				var inputTime = this._timeHelper(row, col, td, obj, column, table);
				td._setCellDisplayValue = function(value) 
				{ 
					inputDate.value = value ? Ris.formatDate(value) : "";
					inputTime.value = value ? Ris.formatTime(value) : ""; 
				};
			},

			"choice": function(row, col, td, obj, column, table)
			{
				// define a function to populate the dropdown
				function addOptions(parent, items)
				{
					if(!items)
						return;
						
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
				td._setCellDisplayValue = function(value) { input.value = (value === undefined || value === null) ? "" : value; }
				if(column.size) input.style.width = column.size + "pc"; // set width in chars

				input.style.marginTop = '1px';
				input.style.marginBottom = '1px';

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
			},

			"combobox": function(row, col, td, obj, column, table) { this["choice"](row, col, td, obj, column, table); },
			"dropdown": function(row, col, td, obj, column, table) { this["choice"](row, col, td, obj, column, table); },
			"enum": function(row, col, td, obj, column, table) { this["choice"](row, col, td, obj, column, table); },
			"list": function(row, col, td, obj, column, table) { this["choice"](row, col, td, obj, column, table); },
			"listbox": function(row, col, td, obj, column, table) { this["choice"](row, col, td, obj, column, table); },

			"check": function(row, col, td, obj, column, table)
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
			},

			"checkbox": function(row, col, td, obj, column, table) { this["check"](row, col, td, obj, column, table); },
			"bool": function(row, col, td, obj, column, table) { this["check"](row, col, td, obj, column, table); },
			"boolean": function(row, col, td, obj, column, table) { this["check"](row, col, td, obj, column, table); },
			
			"lookup": function(row, col, td, obj, column, table) 
			{
				// define a helper to do the lookup
				function doLookup()
				{
					var result = column.lookup(input.value || "");
					if(result)
					{
						column.setValue(obj, result);
						table._onCellUpdate(row, col);
						table._onEditComplete(row, col);
						input.className = "";	// revert field background color since the query is resolved
					}
				}

				var input = document.createElement("input");
				td.appendChild(input);
				td._setCellDisplayValue = function(value)
				{
					input.value = (value === undefined || value === null) ? "" : value;
					input.className = "";
				}
				if(column.size) input.size = column.size;

				// Overwrite IE's default styling for text boxes so that all cell-types have the same height and flow properly.
				// This cannot be done in CSS since IE ignores the "type" pseudo selector.
				input.style.padding = '2px';
				input.style.border = '1px solid #969696';
				
				input.onkeyup = function()
				{
					switch(event.keyCode)
					{
						case 13:
							doLookup();
							break;
						case 9:	// tab into the control
						case 16: // backtab into the control
							break;
						default:
							// any manual edit clears the underlying item
							column.setValue(obj, null);
							table._onCellUpdate(row, col);
							
							// if there are any characters in the input field, change color to indicate to user that the query is unresolved
							input.className = (input.value && column.getValue(obj) === null) ? "unresolved" : "";
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
			},
			
			"html": function(row, col, td, obj, column, table) {}
		},
		
		// override the _renderCell method from _tableMixIn
		_renderCell: function(row, col, td, obj)
		{
			var column = this._columns[col];
			var value = this._getColumnValue(column, obj);
			var table = this;

			if(this._renderer[column.cellType])
			{
				td = this._renderer[column.cellType](row, col, td, obj, column, table) || td;
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

function NewLineField() {
	this.label = "";
	this.cellType = "newline";
	this.getValue = function(item) { return; };
	this.setValue = function(item, value) { return; };
}

var Field = 
{
	setValue: function(element, value)
	{
		element.innerHTML = (value === undefined || value === null) ? "" : (value + "").escapeHTML();
	},

	setLink: function(element, value, clickLink)
	{
		var anchorElement= document.createElement("a");
		anchorElement.appendChild(document.createTextNode(value));
		anchorElement.setAttribute("href", "#");
		anchorElement.onclick = function() { clickLink(); };
		element.appendChild(anchorElement);
	},
	
	setPreFormattedValue: function(element, value)
	{
		element.innerHTML = (value === undefined || value === null) ? "" : (value + "").replaceLineBreak();
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

/*
 *  Date object patches adapted from source cited below.
 */

/*  Copyright Mihai Bazon, 2002-2005  |  www.bazon.net/mishoo
 * -----------------------------------------------------------
 *
 * The DHTML Calendar, version 1.0 "It is happening again"
 *
 * Details and latest version at:
 * www.dynarch.com/projects/calendar
 *
 * This script is developed by Dynarch.com.  Visit us at www.dynarch.com.
 *
 * This script is distributed under the GNU Lesser General Public License.
 * Read the entire license text here: http://www.gnu.org/licenses/lgpl.html
 */

// $Id: calendar.js,v 1.51 2005/03/07 16:44:31 mishoo Exp $

// BEGIN: DATE OBJECT PATCHES

/** Adds the number of days array to the Date object. */
Date._MD = new Array(31,28,31,30,31,30,31,31,30,31,30,31);

// full month names
Date._MN = new Array
("January",
 "February",
 "March",
 "April",
 "May",
 "June",
 "July",
 "August",
 "September",
 "October",
 "November",
 "December");

/** Constants used for time computations */
Date.SECOND = 1000 /* milliseconds */;
Date.MINUTE = 60 * Date.SECOND;
Date.HOUR   = 60 * Date.MINUTE;
Date.DAY    = 24 * Date.HOUR;
Date.WEEK   =  7 * Date.DAY;

Date.parseDate = function(str, fmt) {

	fmt = fmt.replace(/dddd/g, "%a")
			.replace(/ddd/g, "%A")
			.replace(/dd/g, "%z")   // use an intermediate substitution here
			.replace(/d/g, "%e")
			.replace(/MMMM/g, "%B")
			.replace(/MMM/g, "%b")
			.replace(/MM/g, "%m")
			.replace(/HH/g, "%w")   // use an intermediate substitution here
			.replace(/H/g, "%k")
			.replace(/hh/g, "%I")
			.replace(/h/g, "%l")
			.replace(/mm/g, "%M")
			.replace(/tt/g, "%p")
			.replace(/ss/g, "%S")
			.replace(/yyyy/g, "%Y")
			.replace(/yy/g, "%y")
			.replace(/%z/g, "%d")   // replace intermediate substitutions
			.replace(/%w/g, "%H");  // replace intermediate substitutions

	var today = new Date();
	var y = 0;
	var m = -1;
	var d = 0;
	var a = str.split(/\W+/);
	var b = fmt.match(/%./g);
	var i = 0, j = 0;
	var hr = 0;
	var min = 0;
	for (i = 0; i < a.length; ++i) {
		if (!a[i])
			continue;
		switch (b[i]) {
		    case "%d":
		    case "%e":
			d = parseInt(a[i], 10);
			break;

		    case "%m":
			m = parseInt(a[i], 10) - 1;
			break;

		    case "%Y":
		    case "%y":
			y = parseInt(a[i], 10);
			(y < 100) && (y += (y > 29) ? 1900 : 2000);
			break;

		    case "%b":
		    case "%B":
			for (j = 0; j < 12; ++j) {
				if (Date._MN[j].substr(0, a[i].length).toLowerCase() == a[i].toLowerCase()) { m = j; break; }
			}
			break;

		    case "%H":
		    case "%I":
		    case "%k":
		    case "%l":
			hr = parseInt(a[i], 10);
			break;

		    case "%P":
		    case "%p":
			if (/pm/i.test(a[i]) && hr < 12)
				hr += 12;
			else if (/am/i.test(a[i]) && hr >= 12)
				hr -= 12;
			break;

		    case "%M":
			min = parseInt(a[i], 10);
			break;
		}
	}
	if (isNaN(y)) y = today.getFullYear();
	if (isNaN(m)) m = today.getMonth();
	if (isNaN(d)) d = today.getDate();
	if (isNaN(hr)) hr = today.getHours();
	if (isNaN(min)) min = today.getMinutes();
	if (y != 0 && m != -1 && d != 0)
		return new Date(y, m, d, hr, min, 0);
	y = 0; m = -1; d = 0;
	for (i = 0; i < a.length; ++i) {
		if (a[i].search(/[a-zA-Z]+/) != -1) {
			var t = -1;
			for (j = 0; j < 12; ++j) {
				if (Date._MN[j].substr(0, a[i].length).toLowerCase() == a[i].toLowerCase()) { t = j; break; }
			}
			if (t != -1) {
				if (m != -1) {
					d = m+1;
				}
				m = t;
			}
		} else if (parseInt(a[i], 10) <= 12 && m == -1) {
			m = a[i]-1;
		} else if (parseInt(a[i], 10) > 31 && y == 0) {
			y = parseInt(a[i], 10);
			(y < 100) && (y += (y > 29) ? 1900 : 2000);
		} else if (d == 0) {
			d = a[i];
		}
	}
	if (y == 0)
		y = today.getFullYear();
	if (m != -1 && d != 0)
		return new Date(y, m, d, hr, min, 0);
	if (hr != 0 && min != 0)
		return new Date(y, m, d, hr, min, 0);
	return today;
};

/** Returns the number of days in the current month */
Date.prototype.getMonthDays = function(month) {
	var year = this.getFullYear();
	if (typeof month == "undefined") {
		month = this.getMonth();
	}
	if (((0 == (year%4)) && ( (0 != (year%100)) || (0 == (year%400)))) && month == 1) {
		return 29;
	} else {
		return Date._MD[month];
	}
};

/** Returns the number of day in the year. */
Date.prototype.getDayOfYear = function() {
	var now = new Date(this.getFullYear(), this.getMonth(), this.getDate(), 0, 0, 0);
	var then = new Date(this.getFullYear(), 0, 0, 0, 0, 0);
	var time = now - then;
	return Math.floor(time / Date.DAY);
};

/** Returns the number of the week in year, as defined in ISO 8601. */
Date.prototype.getWeekNumber = function() {
	var d = new Date(this.getFullYear(), this.getMonth(), this.getDate(), 0, 0, 0);
	var DoW = d.getDay();
	d.setDate(d.getDate() - (DoW + 6) % 7 + 3); // Nearest Thu
	var ms = d.valueOf(); // GMT
	d.setMonth(0);
	d.setDate(4); // Thu in Week 1
	return Math.round((ms - d.valueOf()) / (7 * 864e5)) + 1;
};

/** Checks date and time equality */
Date.prototype.equalsTo = function(date) {
	return ((this.getFullYear() == date.getFullYear()) &&
		(this.getMonth() == date.getMonth()) &&
		(this.getDate() == date.getDate()) &&
		(this.getHours() == date.getHours()) &&
		(this.getMinutes() == date.getMinutes()));
};

Date.prototype.__msh_oldSetFullYear = Date.prototype.setFullYear;
Date.prototype.setFullYear = function(y) {
	var d = new Date(this);
	d.__msh_oldSetFullYear(y);
	if (d.getMonth() != this.getMonth())
		this.setDate(28);
	this.__msh_oldSetFullYear(y);
};

