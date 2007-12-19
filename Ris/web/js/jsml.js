/*
    This module defines JSML (Javascript Markup-language)
    
    JSML is largely based on the ideas contained in JSON (www.json.org) and some of the code has be
    adapted from the reference json implementation (www.json.org/json.js).
    
    JSML (Javascript Markup-language) is intended to provide a simple means of serializing a Javascript
    object graph to XML format.  The graph must not contain any cyclical references.
    
    This file adds these methods to JavaScript:

    JSML.create(obj, rootTag)
        Creates a JSML representation of the specified obj, using
        rootTag as the tag name of the root XML element.
        

    JSML.parse(jsml)
        This method parses JSML text to produce an object or
        array.
        
    array.toJsml()
    boolean.toJsml()
    date.toJsml()
    number.toJsml()
    object.toJsml()
    string.toJsml()
        These methods produce JSML text from a JavaScript value.
        It must not contain any cyclical references. Illegal values
        will be excluded.

        The default conversion for dates is to an ISO string. You can
        add a toJsml method to any date object to get a different
        representation.

*/

// augment the basic Javascript classes with Jsml
if(!Object.prototype.toJsml)
{

    Object.prototype.toJsml = function()
    {
        var xml = "";
        for(var prop in this)
        {
            // check that the prop belongs to this object (not its prototype) and that the value is non-null and is not a function
            if(this.hasOwnProperty(prop) && this[prop] && !(this[prop] instanceof Function))
                xml += JSML.create(this[prop], prop);
        }
        return xml;
    }
    
    Array.prototype.toJsml = function()
    {
        return this.reduce("", function(jsml, item) { return jsml + JSML.create(item, "item"); });
    }
    
    Boolean.prototype.toJsml = function () {
        return String(this);
    };


    Date.prototype.toJsml = function () {
        return this.toISOString();
    };

    Number.prototype.toJsml = function () {
    // JSON numbers must be finite. Encode non-finite numbers as null.
        return isFinite(this) ? String(this) : "null";
    };
    
    String.prototype.toJsml = function () {
/*
        // If the string contains no control characters, no quote characters, and no
        // backslash characters, then we can simply return it.
        // Otherwise we must also replace the offending characters with safe
        // sequences.

        if (/["\\\x00-\x1f]/.test(this)) {
            return '"' + this.replace(/([\x00-\x1f\\"])/g, function (a, b) {
                var c = m[b];
                if (c) {
                    return c;
                }
                c = b.charCodeAt();
                return '\\u00' +
                    Math.floor(c / 16).toString(16) +
                    (c % 16).toString(16);
            }) + '"';
        }
*/        
        return this.escapeHTML();   // works for XML too
    };
};

// definition of the JSML object		    
var JSML = {
    // default no-op parse filter
    _parseFilter: function(key, value)
    {
        return value;
    },
    
    /* Sets a global parse filter callback.
        The filter callback is a function which can filter and
        transform the results. It receives each of the keys and values, and
        its return value is used instead of the original value. If it
        returns what it received, then structure is not modified. If it
        returns undefined then the member is deleted.

        Example:

        // Parse the text. If a key contains the string 'date' then
        // convert the value to a date.

        function myFilter(key, value) {
            return key.indexOf('date') >= 0 ? new Date(value) : value;
        });
    */
    setParseFilter: function(filterFunc)
    {
        this._parseFilter = filterFunc;
    },
    
    parse: function(jsml)
    {
        function parseXml(xml) {
           var dom = null;
           if (window.DOMParser) {
                 return (new DOMParser()).parseFromString(xml, "text/xml"); 
           }
           else if (window.ActiveXObject) {
                 dom = new ActiveXObject('Microsoft.XMLDOM');
                 dom.async = false;
                 if (!dom.loadXML(xml)) // parse error ..
                    throw (dom.parseError.reason + dom.parseError.srcText);
                 return dom;
           }
        }
        
        // put child nodes in an array for convenience
        function getChildNodes(xmlNode)
        {
            var a = [];
            for (var n=xmlNode.firstChild; n; n=n.nextSibling)
                a.push(n);
            return a;
        }
        
        // convert a JSML fragment to a Javascript object
        function toObj(xmlNode)
        {
            var subElements = getChildNodes(xmlNode).select(function(n) { return n.nodeType == 1; });   // select element nodes
            var arrayAttr = xmlNode.attributes.getNamedItem("array");
            
            if(arrayAttr && arrayAttr.text == "true")
            {
                // collect sub-elements in an array
                return subElements.reduce([], function(a, node) { a.push(toObj(node)); return a; });
            }
            else
            {
                if(subElements.length > 0)  // node contains elements
                {
                    // treat sub-elements as independent properties
                    return subElements.reduce({},
                        function(o, n)
                        {
                            o[n.nodeName] = JSML._parseFilter(n.nodeName, toObj(n));
                            return o;
                        });
                }
                else    // node contains text
                {
                    // find the first non-empty text node
                    var textNodes = getChildNodes(xmlNode).select(function(n) { return n.nodeType==3 && n.nodeValue.match(/[^ \f\n\r\t\v]/); });
                    var value = textNodes.length > 0 ? textNodes[0].nodeValue : null;
                    return value ? parseValue(value) : null;
                }
            }
        }
        
        // deserialize value to an object
        function parseValue(str)
        {
            if(str.match(/^[+-]?[1-9]\d*$/))
                return parseInt(str);
            if(str.match(/^[+-]?\d*\.\d+$/))
                return parseFloat(str);
            if(str.match(/^(true|false)$/i))
                return str.toLowerCase() == "true";
            if(str.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$/))
                return Date.parseISOString(str);
            
            // treat it as a string
            return str;
        }
        
        if(jsml && jsml.length)
        {
            var dom = parseXml(jsml);
            return dom.documentElement ? toObj(dom.documentElement) : null;
        }
        else
        {
            return null;
        }
    },
    
    create: function(obj, tagName)
    {
        return '<'+tagName+(obj.isArray?' array="true"':'')+'>'+obj.toJsml()+'</'+tagName+'>';
    }    
};
