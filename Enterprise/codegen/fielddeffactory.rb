require 'constants'
require 'primitivefielddef'
require 'collectionfielddef'
require 'enumfielddef'
require 'componentfielddef'
require 'entityfielddef'
require 'usertypefielddef'

# Factory class to create FieldDef subclasses of the correct type, based upon the specified fieldNode
class FieldDefFactory
  
  # Creates a FieldDef subclass based on the specified fieldNode
  def FieldDefFactory.CreateFieldDef(model, fieldNode)
    # what kind of field is this?
    if(NHIBERNATE_COLLECTION_TYPES.include?(fieldNode.name))
      return CollectionFieldDef.new(model, fieldNode)
    elsif(fieldNode.name == 'many-to-one')
      return EntityFieldDef.new(model, fieldNode)
    elsif(fieldNode.name == 'component')
      return ComponentFieldDef.new(model, fieldNode)
    elsif(Model.removeAssemblyQualifier(fieldNode.attributes['type']) =~ /EnumHbm$/)
      return EnumFieldDef.new(model, fieldNode)
    elsif(Model.removeAssemblyQualifier(fieldNode.attributes['type']) =~ /Hbm$/)
      return UserTypeFieldDef.new(model, fieldNode)
    else
      return PrimitiveFieldDef.new(model, fieldNode)
    end
    
  end
end
