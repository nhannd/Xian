require 'fielddef'

# Represents the definition of a field that is a collection type,
# such as an IList or ISet
class CollectionFieldDef < FieldDef
  
  def initialize(model, fieldNode)
    super(model, fieldNode)
    @dataType = DATATYPE_MAPPINGS[fieldNode.name]
    @isLazy = (fieldNode.attributes['lazy'] == 'true')
    elementNode = fieldNode.elements['composite-element'] || fieldNode.elements['one-to-many'] || fieldNode.elements['many-to-many']
    @elementType = elementNode.attributes['class'] if elementNode
  end
  
  def kind
    :collection
  end

  def dataType
    @dataType
  end
  
  def elementType
    @elementType
  end
  
  def supportDataType
    "List<"+collectionElementClassDef.supportClassName+">"
  end
  
  def initialValue
    CSHARP_INITIALIZERS[@dataType]
  end
  
  def supportInitialValue
    "new #{supportDataType}()"
  end
  
  # a collection field is never mandatory
  def isMandatory
    false
  end
  
  # collection fields never have setters
  def hasSetter
    true
  end
  
  def setterAccess
	  "private"
  end
  
  # true if this field is a lazy collection
  def isLazy
    @isLazy
  end
  
  # searching on collection fields is not currently supported
  def isSearchable
    false
  end
  
protected
  def collectionElementClassDef
     classDef = model.entityDefs.find {|entity| entity.className == @elementType}
     if(classDef == nil)
	classDef = model.componentDefs.find {|component| component.className == @elementType}
     end
     return classDef
  end
 
end
