# NHibernate elements that result in C# classes
NHIBERNATE_CLASS_TYPES = ['class', 'joined-subclass', 'subclass']

# NHibernate elements that result in C# fields
NHIBERNATE_FIELD_TYPES = ['property', 'many-to-one','component','map','set','idbag', 'bag', 'list']

# NHibernate collection elements supported by this code generator
NHIBERNATE_COLLECTION_TYPES = ['map', 'set', 'idbag', 'bag', 'list']

# NHibernate to C# type mappings
DATATYPE_MAPPINGS =
{
	'map' => 'IDictionary',
	'set' => 'ISet',
	'idbag' => 'IList',
	'String' => 'string',
	'Boolean' => 'bool',
	'bag' => 'IList',
	'list' => 'IList',
	'StringClob' => 'string'
}

# C# primitive datatypes
CSHARP_PRIMITIVES = ['string', 'bool', 'int', 'DateTime', 'DateTime?']

# C# datatype initializers
CSHARP_INITIALIZERS = 
{
	'IDictionary' => 'new Hashtable()',
	'ISet' => 'new HybridSet()',
	'IList' => 'new ArrayList()',
	'DateTime' => 'Platform.Time'
}

