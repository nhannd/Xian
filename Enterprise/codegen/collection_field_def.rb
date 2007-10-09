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
 
  def attributes
    attrs = super
    if(@elementNode)
	    case
	      when @elementNode.name == 'composite-element' : attrs << "EmbeddedValueCollection(typeof(#{elementType}))"
	      #when @elementNode.name == 'one-to-many' : attrs << "OneToMany(typeof(#{elementType}))"
	      #when @elementNode.name == 'many-to-many' : attrs << "ManyToMany(typeof(#{elementType}))"
	    end
    end
    attrs
  end
  
protected
  def collectionElementClassDef
     model.findClass(@elementType)
  end
 
end
