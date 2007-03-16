require 'classdef'
require 'type_name_utils'

# Represents the definition of an enum class
class EnumDef < ClassDef
  attr_reader :enumName
  
  def initialize(model, classNode, namespace, suppressCodeGen)
    super(model, TypeNameUtils.getShortName(classNode.attributes['name']), namespace, suppressCodeGen)
    @enumName = @className.sub("Enum", "")
  end
  
  def kind
    :enum
  end
end
