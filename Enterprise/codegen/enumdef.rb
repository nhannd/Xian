require 'classdef'

# Represents the definition of an enum class
class EnumDef < ClassDef
  attr_reader :enumName
  
  def initialize(model, classNode)
    super(model, classNode.attributes['name'])
    @enumName = @className.sub("Enum", "")
  end
  
  def kind
    :enum
  end
end
