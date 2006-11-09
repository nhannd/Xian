require 'classdef'

# Represents the definition of an entity class
class EntityDef < ClassDef
  attr_reader :superClassName, :isSubClass
  
  def initialize(model, classNode, superClassName)
    super(model, classNode.attributes['name'])
    @superClassName = superClassName
    @isSubClass = (superClassName != "Entity")
    classNode.each_element do |fieldNode|
      processField(fieldNode) if(NHIBERNATE_FIELD_TYPES.include?(fieldNode.name))
    end
  end
  
  def kind
    :entity
  end
  
  def supportClassName
    className + "Support"
  end
  
  def searchCriteriaClassName
    className + "SearchCriteria"
  end
end
