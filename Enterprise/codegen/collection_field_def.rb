require 'field_def'
require 'type_name_utils'

# Represents the definition of a field that is a collection type,
# such as an IList or ISet
class CollectionFieldDef < FieldDef
  
  def initialize(model, fieldNode, defaultNamespace)
    super(model, fieldNode)
    @dataType = DATATYPE_MAPPINGS[fieldNode.name]
    @isLazy = (fieldNode.attributes['lazy'] == 'true')
    @elementNode = fieldNode.elements['composite-element'] || fieldNode.elements['one-to-many'] || fieldNode.elements['many-to-many']
    @elementType = TypeNameUtils.getQualifiedName(@elementNode.attributes['class'], defaultNamespace) if @elementNode
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
  
  # collection setters should be private  
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
 
# JR: commenting this out for now since we dont' really need - if we need it in future, just add it back in 
#  def attributes
#    attrs = super
#    case
#      when @elementNode.name == 'composite-element' : attrs << "ValueCollection(typeof(#{elementType}))"
#      when @elementNode.name == 'one-to-many' : attrs << "OneToMany(typeof(#{elementType}))"
#      when @elementNode.name == 'many-to-many' : attrs << "ManyToMany(typeof(#{elementType}))"
#    end
#    attrs
#  end
  
protected
  def collectionElementClassDef
     model.findClass(@elementType)
  end
 
end
