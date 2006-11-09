require 'fielddef'

# Represents the definition of a field that is a component
class ComponentFieldDef < FieldDef
  
  def initialize(model, fieldNode)
    super(model, fieldNode)
    @dataType = Model.fixDataType(fieldNode.attributes['class'])
  end

  def kind
    :component
  end
  
  def dataType
    @dataType
  end
  
  def initialValue
    "new #{dataType}()"
  end
  
  # a component field is mandatory if the component contains mandatory fields
  def isMandatory
    componentDef.mandatoryFields.length > 0
  end
  
  def supportDataType
    componentDef.supportClassName
  end
  
  def supportInitialValue
    "new #{componentDef.supportClassName}()"
  end

  def searchCriteriaDataType
    componentDef.searchCriteriaClassName
  end
  
  def searchCriteriaReturnType
    componentDef.searchCriteriaClassName
  end

protected
  def componentDef
    model.componentDefs.find {|component| component.className == @dataType}
  end

end
