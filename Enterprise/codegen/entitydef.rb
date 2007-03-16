require 'classdef'

# Represents the definition of an entity class
class EntityDef < ClassDef
  attr_reader :superClassName, :isSubClass
  
  def initialize(model, classNode, namespace, superClassName, suppressCodeGen)
    super(model, TypeNameUtils.getShortName(classNode.attributes['name']), namespace, suppressCodeGen)
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
    className + "Info"
  end
  
  def supportSuperClassName
    @superClassName + "Info"
  end
  
  def searchCriteriaClassName
    className + "SearchCriteria"
  end
end
