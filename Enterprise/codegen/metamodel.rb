require 'rexml/document'

# NHibernate elements that result in C# classes
NHIBERNATE_CLASS_TYPES = ['class', 'joined-subclass']

# NHibernate elements that result in C# fields
NHIBERNATE_FIELD_TYPES = ['property', 'many-to-one','component','map','set','idbag', 'bag', 'list']

# NHibernate collection elements supported by this code generator
NHIBERNATE_COLLECTION_TYPES = ['map', 'set', 'idbag', 'bag', 'list']

# NHibernate to C# type mappings
DATATYPE_MAPPINGS =
{
	'map' => 'IDictionary',
	'set' => 'ISet',
	'idbag' => 'IList',
	'String' => 'string',
	'Boolean' => 'bool',
	'bag' => 'IList',
	'list' => 'IList'
}

# C# datatype initializers
DATATYPE_INITIALIZERS = 
{
	'IDictionary' => 'new Hashtable()',
	'ISet' => 'new HybridSet()',
	'IList' => 'new ArrayList()',
	'DateTime' => 'DateTime.Now',
	'DateTimeRange' => 'new DateTimeRange()'
}

class ElementDef
  def namespace
  end

  def get_binding
    binding
  end
end


# Represents a Hibernate model
class Model < ElementDef
  attr_reader :namespace, :entityDefs, :enumDefs, :componentDefs

  # hbmFile - the *.hbm.xml mapping file from which the model is built
  def initialize(hbmFile = nil)
    @entityDefs = []
    @enumDefs = []
    @componentDefs = []
    @symbolSpace = []
    add(hbmFile) if hbmFile
  end
  
  # add the specified hbm file to the model  
  def add(hbmFile)
    # read the hbm xml file
    mappings = REXML::Document.new(File.new(hbmFile))
    
    # extract the namespace - TODO throw exception if model already defined and doesn't match
    @namespace = mappings.root.attributes['namespace'] if @namespace == nil
    
    # process each class in the hbm file
    mappings.root.each_element do |c|
      if(NHIBERNATE_CLASS_TYPES.include?(c.name))
	      className = c.attributes['name']
	      
	      # check if this class name already exists
	      if(!@symbolSpace.include?(className))
		if(/Enum$/.match(className))	#does the class name end with "Enum"?
		  processEnum(c)
		else
		  processEntity(c, c.attributes['extends'] || "Entity", @entityDefs)
		end
	      end
      end
    end
    
  end
  
  # returns the last component of the namespace
  def shortName
    @namespace.split('.')[-1]
  end
  
  # processes componentNode to create instances of ComponentDef
  # this method is public because it must be called from the ClassDef class - it should not be called otherwise
  def processComponent(componentNode)
    componentDef = ComponentDef.new(self, componentNode)
    @componentDefs << componentDef if(!@symbolSpace.include?(componentDef.className))
  end

protected

  # processes classNode to create instances of EntityDef
  def processEntity(classNode, superClassName, entityDefs)
    #create EntityDef for classNode
    entityDef = EntityDef.new(self, classNode, superClassName)
    entityDefs << entityDef
    @symbolSpace << entityDef.className
    
    #process subclassses recursively
    classNode.each_element do |subclassNode|
      processEntity(subclassNode, entityDef.className, entityDefs ) if(NHIBERNATE_CLASS_TYPES.include?(subclassNode.name))
    end
  end
  
  # processes classNode to create instances of EnumDef
  def processEnum(classNode)
    enumDef = EnumDef.new(self, classNode)
    @enumDefs << enumDef
    @symbolSpace << enumDef.className
  end
  
  def Model.fixDataType(name)
    # remove the assembly name if present
    if(commaPos = name.index(","))
      name = name[0, commaPos]
    end
    
    # remove the namespace
    name = name.split('.')[-1]
    
    # remove "EnumHbm" or "Hbm" - this is a hack to extract the underlying datatype from "mapper" types
    name = name.sub(/EnumHbm$/, "").sub(/Hbm$/, "")
  end
end

# represents the definition of a class - see subclasses EnumDef, EntityDef, and ComponentDef
class ClassDef < ElementDef
  attr_reader :className, :model, :fields
  
  def initialize(model, className)
    @model = model
    @className = className
    @fields = []
  end
  
  def namespace
    @model.namespace
  end

protected  
  # processes fields to create instances of FieldDef
  # a "field" is a node of type property, map, set, many-to-one, and others
  def processField(fieldNode)
    field = FieldDef.new(fieldNode)
    @fields << field
    
    # check for components/composite elements, and process them
    if(fieldNode.name == 'component')
      @model.processComponent(fieldNode) 
    elsif(field.isCollection && (compositeElementNode = fieldNode.elements['composite-element']))
      @model.processComponent(compositeElementNode)
    end
  end
end

class EnumDef < ClassDef
  attr_reader :enumName
  def initialize(model, classNode)
    super(model, classNode.attributes['name'])
    @enumName = @className.sub("Enum", "")
  end
end


class EntityDef < ClassDef
  def initialize(model, classNode, superClassName)
    super(model, classNode.attributes['name'])
    @superClassName = superClassName
    @isSubClass = (superClassName != "Entity")
    classNode.each_element do |fieldNode|
      processField(fieldNode) if(NHIBERNATE_FIELD_TYPES.include?(fieldNode.name))
    end
  end
end

class ComponentDef < ClassDef
  def initialize(model, componentNode)
    super(model, Model.fixDataType(componentNode.attributes['class']))
    componentNode.each_element do |fieldNode|
      processField(fieldNode) if(NHIBERNATE_FIELD_TYPES.include?(fieldNode.name))
    end
  end
end

# represents the definition of a field within a ClassDef
class FieldDef
  attr_reader :fieldName, :accessorName, :dataType, :hasGetter, :hasSetter, :isCollection, :isComponent, :initialValue, :isLazy, :isEntityRef

  def initialize(fieldNode)
	# establish some basic things about the field
      @isCollection = NHIBERNATE_COLLECTION_TYPES.include?(fieldNode.name)
      @isLazy = (fieldNode.attributes['lazy'] == 'true')
      @isEntityRef = (fieldNode.name == 'many-to-one')
      @isComponent = (fieldNode.name == 'component')

	# determine the access strategy - if the strategy contains multiple parts separated by . then only take the first part
	# use "property" as the default strategy, as hibernate does
	access = (fieldNode.attributes['access'] || "property").split('.')[0];
	
	@hasGetter = ['property', 'nosetter'].include?(access)
	@hasSetter = (access == 'property' && !@isCollection)
  
      @accessorName = fieldNode.attributes['name']
      @fieldName = "_" + @accessorName[0..0].downcase + @accessorName[1..-1]
      
	#if 'not-null' attribute is omitted, the default value is false (eg. the column is nullable)
      nullable = (fieldNode.attributes['not-null'] == nil || fieldNode.attributes['not-null'] == 'false')

      
      
	# get the raw datatype,  from either the node name itself, or the 'class' or 'type' attribute
      # the call to fixDataType will simply remove assembly name qualifiers, etc
	@dataType = @isCollection ? fieldNode.name :
				Model.fixDataType(['many-to-one', 'component'].include?(fieldNode.name) ? fieldNode.attributes['class'] :
					fieldNode.attributes['type'])
 
      # map the datatype if possible, otherwise assume that the raw datatype is a valid C# type
      @dataType = DATATYPE_MAPPINGS[@dataType] || @dataType
      
      # special handling of DateTime, because we need to support nullable
	@dataType << "?" if(@dataType == 'DateTime' && nullable)
      
       
      # determine the C# initializer for the field
      @initialValue = DATATYPE_INITIALIZERS[@dataType] || ("#{@dataType}.New()" if @isComponent)
  end
end

