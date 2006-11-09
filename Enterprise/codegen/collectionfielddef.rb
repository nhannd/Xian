require 'fielddef'

# Represents the definition of a field that is a collection type,
# such as an IList or ISet
class CollectionFieldDef < FieldDef
  
  def initialize(model, fieldNode)
    super(model, fieldNode)
    @dataType = DATATYPE_MAPPINGS[fieldNode.name]
    @isLazy = (fieldNode.attributes['lazy'] == 'true')
  end
  
  def kind
    :collection
  end

  def dataType
    @dataType
  end
  
  def initialValue
    CSHARP_INITIALIZERS[@dataType]
  end
  
  # a collection field is never mandatory
  def isMandatory
    false
  end
  
  # collection fields never have setters
  def hasSetter
    false
  end
  
  # true if this field is a lazy collection
  def isLazy
    @isLazy
  end
  
  # searching on collection fields is not currently supported
  def isSearchable
    false
  end
end
