/*
    Augments the javascript Array prototype with a number of convenience and functional-style methods, and some events.
    The following methods are added:
        add
        each
        map
        reduce
        select
        removeAt
        remove
        isArray
        
    The following events are added:
        itemAdded (note: this event is only fired when the "add" method is called - assignment by [] does not invoke this event)
        itemRemoved
*/

// isArray always returns true - provides an easy way to test if an unknown object is an instance of an array or not
if(!Array.prototype.isArray)
{
    Array.prototype.isArray = true;
}

// adds an item to the end of the array
if(!Array.prototype.add){
	Array.prototype.add = function(obj)
	{
		var i = this.length;
		this[i] = obj;
		if(this.itemAdded)
			this.itemAdded(obj, i);
	};
}

// iterates over the array, passing each element to the supplied function
if(!Array.prototype.each)
{
    Array.prototype.each = function(func)
    {
        for(var i = 0; i < this.length; i++)
            func(this[i]);
    }
}

// maps this array onto a new array, using the the supplied mapping function
if(!Array.prototype.map)
{
    Array.prototype.map = function(func)
    {
        var result = [];
        for(var i = 0; i < this.length; i++)
            result[i] = func(this[i]);
        return result;
    }
}

// reduces this array to a scalar value by calling the specified function for each item in the array,
// an taking the return value of the function as the next value of the "accumlator"
//      initial - the initial value of the accumlator
//      func - a function of the form func(accumlator, element), that returns a new value for the accumlator
// e.g. if x is an array of ints, then sum(x) = x.reduce(0, function(sum, y) { return sum + y; });
if(!Array.prototype.reduce)
{
    Array.prototype.reduce = function(initial, func)
    {
        var memo = initial;
        for(var i = 0; i < this.length; i++)
            memo = func(memo, this[i]);
        return memo;
    }
}

// returns a new array containing only those elements of this array that satisfy the specified predicate function
if(!Array.prototype.select)
{
    Array.prototype.select = function(func)
    {
        var result = [];
        for(var i = 0; i < this.length; i++)
            if(func(this[i]))
                result.push(this[i]);
        return result;
    }
}

// returns the index of the specified object, or -1 if not found
if(!Array.prototype.indexOf)
{
	Array.prototype.indexOf = function(obj)
	{
        for(var i = 0; i < this.length; i++)
            if(this[i] == obj)
                return i;
        return -1;
	};
}


// removes the item at the specified index
if(!Array.prototype.removeAt)
{
	Array.prototype.removeAt = function(i)
	{
		var obj = this.splice(i, 1);
		if(this.itemRemoved)
			this.itemRemoved(obj, i);
		return obj;
	};
}

// removes the specified item from the array, or does nothing if the item is not contained in this array
if(!Array.prototype.remove)
{
	Array.prototype.remove = function(obj)
	{
		var i = this.indexOf(obj);
		return (i > -1) ? this.removeAt(i) : null;
	};
}
