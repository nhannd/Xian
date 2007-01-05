require 'elementdef'
require 'fielddef'
require 'fielddeffactory'

# represents the definition of a logical class (a class as defined by an NHibernate mapping).
# A number of C# classes that serve different purposes will typically be generated for each logical class
# There are several different categories of logical classes:
# see subclasses EnumDef, EntityDef, and ComponentDef
class ClassDef < ElementDef
  attr_reader :className, :model, :fields
  
  def initialize(model, className)
    @model = model
    @className = className
    @fields = []
  end
  
  def elementName
    @className
  end
  
  def namespace
    @model.namespace
  end
  
  # the kind of class (:entity, :enum, :component)
  def kind
    nil # defer to subclass
  end
  
  # returns the set of non-collection fields defined in this class, not including superclass fields
  def singleValuedFields
    @fields.select {|f| f.kind != :collection}
  end
  
  # returns the set of mandatory (non-nullable) fields defined in this class, not including superclass fields
  def mandatoryFields
    @fields.select {|f| f.isMandatory }
  end
  
  # true if this class is a subclass
  def isSubClass
    false # default to false
  end
  
  # returns the superclass as a ClassDef, or null if the superclass is "Entity"
  def superClass
    return nil if !isSubClass
    @model.entityDefs.find {|entity| entity.className == @superClassName}
  end
  
  # returns the set of inherited fields as an array of FieldDef
  def inheritedFields
    superClass ? (superClass.fields + superClass.inheritedFields) : []
  end
  
  # returns the set of inherited fields that are mandatory (non-nullable) as an array of FieldDef
  def inheritedMandatoryFields
    superClass ? (superClass.mandatoryFields + superClass.inheritedMandatoryFields) : []
  end
  
  # returns the set of fields that require initialization
  def initializedFields
    @fields.select {|f| f.initialValue }
  end
  
  # returns the set of fields that are searchable (a search criteria or condition can be created for the field)
  def searchableFields
    @fields.select {|f| f.isSearchable }
  end

  
protected  
  # processes fields to create instances of FieldDef
  # a "field" is a node of type property, map, set, many-to-one, and others
  def processField(fieldNode)
    field = FieldDefFactory.CreateFieldDef(@model, fieldNode)
    @fields << field
    
    # check for components/composite elements, and process them
    if(fieldNode.name == 'component')
      @model.processComponent(fieldNode) 
    elsif(field.kind == :collection && (compositeElementNode = fieldNode.elements['composite-element']))
      @model.processComponent(compositeElementNode)
    end
  end
end
