require 'fielddef'

# Represents the definition of a field that is a C# enum type
class EnumFieldDef < FieldDef
  
  def initialize(model, fieldNode)
    super(model, fieldNode)
    @dataType = Model.fixDataType(fieldNode.attributes['type'])
  end

  def kind
    :enum
  end
  
  def dataType
    @dataType
  end
  
  # enum fields do not need to be initialized
  def initialValue
    nil
  end
  
  # a enum field is always mandatory
  def isMandatory
    true
  end

end
