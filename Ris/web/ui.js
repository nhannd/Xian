/*
    Table
*/
function Table(htmlTable, columns, items)
{
	this.htmlTable = htmlTable;
	this.columns = columns || [];
	this.checkBoxes = [];
	this.items = items || [];
	
	this.getSelectedItems = function()
	{
		var result = [];
		for(var i=0; i < this.htmlTable.rows.length; i++)
		{
			if(this.checkBoxes[i] && this.checkBoxes[i].checked)
				result.add(this.items[i-1]);
		}
		return result;
	}
	this._addRow = function(item)
	{
		var index = this.htmlTable.rows.length;
		var tr = this.htmlTable.insertRow(index);
		var td = tr.insertCell(0);
		var checkBox = document.createElement("input");
		checkBox.type = "checkbox";
		td.appendChild(checkBox);
		this.checkBoxes[index] = checkBox;
		for(var i=0; i < this.columns.length; i++)
		{
			td = tr.insertCell(i+1);
			td.innerHTML = columns[i].getter(item) || "";
		}
	}
	this._removeRow = function(index)
	{
		this.htmlTable.deleteRow(index+1);
		this.checkBoxes.removeAt(index+1);
	}
	
	// bind to events on the items array
	var table = this;
	this.items.itemAdded = function(obj, index) { table._addRow(obj); }
	this.items.itemRemoved = function(obj, index) { table._removeRow(index); }
	
	// init table with items array
	this.items.each(function(item) { table._addRow(item); });
}
