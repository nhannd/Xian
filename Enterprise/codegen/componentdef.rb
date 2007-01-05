require 'classdef'

# Represents the definition of a component class
class ComponentDef < ClassDef
  
  def initialize(model, componentNode)
    super(model, Model.fixDataType(componentNode.attributes['class']))
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
    className + "Support"
  end

  def searchCriteriaClassName
    className + "SearchCriteria"
  end
end
