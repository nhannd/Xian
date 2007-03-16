require 'class_def'
require 'type_name_utils'

# Represents the definition of a component class
class ComponentDef < ClassDef
  
  def initialize(model, componentNode, namespace, suppressCodeGen)
    super(model, TypeNameUtils.getShortName(componentNode.attributes['class']), namespace, suppressCodeGen)
    componentNode.each_element do |fieldNode|
      processField(fieldNode) if(NHIBERNATE_FIELD_TYPES.include?(fieldNode.name))
    end
  end
  
  def kind
    :component
  end
  
  def superClassName
    "ValueObject"
  end
  
  def supportClassName
    className + "Info"
  end

  def searchCriteriaClassName
    className + "SearchCriteria"
  end
end
