require 'fielddef'

# Represents the definition of a field that is a reference to another entity
class EntityFieldDef < FieldDef

  def initialize(model, fieldNode)
    super(model, fieldNode)
    @dataType = Model.fixDataType(fieldNode.attributes['class'])
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
  
#  def searchCriteriaDataType
 #   entityDef.searchCriteriaClassName
 # end
  
#  def searchCriteriaReturnType
#    entityDef.searchCriteriaClassName
#  end

protected
  def entityDef
    model.entityDefs.find {|entity| entity.className == @dataType}
  end
  
end
