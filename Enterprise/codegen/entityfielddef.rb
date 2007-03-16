require 'fielddef'
require 'type_name_utils'

# Represents the definition of a field that is a reference to another entity
class EntityFieldDef < FieldDef

  def initialize(model, fieldNode, defaultNamespace)
    super(model, fieldNode)
    @dataType = TypeNameUtils.getQualifiedName(fieldNode.attributes['class'], defaultNamespace)
  end

  def kind
    :entity
  end
  
  def dataType
    @dataType
  end
  
  def initialValue
    nil
  end
  
  def supportDataType
    "EntityRef"
  end
end
