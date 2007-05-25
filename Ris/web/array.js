/*
    Augments the javascript Array prototype with a number of functional-style oonvenience methods.
*/

// adds an item to the end of the array
if(!Array.prototype.push){
    Array.prototype.push=function(x){
        this[this.length]=x;
        return true
    }
};

// removes the last item from the array
if (!Array.prototype.pop){
    Array.prototype.pop=function(){
        var response = this[this.length-1];
        this.length--;
        return response
    }
};

// iterates over the array, passing each element to the supplied function
if(!Array.prototype.each)
{
    Array.prototype.each = function(func)
    {
        for(var i = 0; i < this.length; i++)
            func(this[i]);
    }
};

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
};

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
};

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
};
