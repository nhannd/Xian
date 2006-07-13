require 'rexml/document'

class ElementDef
  def namespace
  end

  def get_binding
    binding
  end
end


# Represents a Hibernate model
class Model < ElementDef
  attr_reader :namespace, :entityDefs, :enumDefs

  # hbmFile - the *.hbm.xml mapping file from which the model is built
  def initialize(hbmFile = nil)
    @entityDefs = []
    @enumDefs = []
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
    mappings.root.each_element('class') do |c|
      className = c.attributes['name']
      
      # check if this class name already exists
      if(!@symbolSpace.include?(className))
        if(/Enum$/.match(className))
          processEnum(c)
        else
          processEntity(c, "Entity", @entityDefs)
        end
      end
    end
  end
  
  def shortName
    #return the last component of the namespace
    @namespace.split('.')[-1]
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
      processEntity(subclassNode, entityDef.className, entityDefs ) if(['joined-subclass', 'subclass'].include?(subclassNode.name))
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
    
    # remove "EnumHbm" - this is a hack to remove the Hbm from mapped enum classes
    name = name.sub("EnumHbm", "")
  end
end

# represents the definition of a class
class ClassDef < ElementDef
  attr_reader :className, :model
  
  def initialize(model, classNode)
    @model = model
    @className = classNode.attributes['name']
  end
  
  def namespace
    @model.namespace
  end
end

class EnumDef < ClassDef
  attr_reader :enumName
  def initialize(model, classNode)
    super(model, classNode)
    @enumName = @className.sub("Enum", "")
  end
end


class EntityDef < ClassDef
  attr_reader :fields
  
  def initialize(model, classNode, superClassName)
    super(model, classNode)
    @superClassName = superClassName
    @isSubClass = (superClassName != "Entity")
    @fields = []
    classNode.each_element do |fieldNode|
      processField(fieldNode) if(['property', 'map','set','many-to-one','component'].include?(fieldNode.name))
    end
  end
  
protected  
  # processes fields to create instances of FieldDef
  # a "field" is a node of type property, map, set, many-to-one, and others
  def processField(fieldNode)
      @fields << FieldDef.new(fieldNode)
  end
  
end

# represents the definition of a field within an entity class
class FieldDef
  @@initialValues = { 'IDictionary' => 'new Hashtable()', 'ISet' => 'new HybridSet()', 'DateTime' => 'DateTime.Now'}

  attr_reader :fieldName, :accessorName, :dataType, :readOnly, :isCollection, :initialValue

  def initialize(fieldNode)
      @accessorName = fieldNode.attributes['name']
      @fieldName = "_" + @accessorName[0..0].downcase + @accessorName[1..-1]
      
      #if 'not-null' attribute is omitted, the default value is false (eg. the column is nullable)
      nullable = (fieldNode.attributes['not-null'] == nil || fieldNode.attributes['not-null'] == 'false')
      
      if(fieldNode.name == "property")
        @dataType = Model.fixDataType(fieldNode.attributes['type'])
	@dataType << "?" if(@dataType == 'DateTime' && nullable)
      elsif(['many-to-one', 'component'].include?(fieldNode.name))
        @dataType = Model.fixDataType(fieldNode.attributes['class'])
      elsif(fieldNode.name == "map")
        @dataType = "IDictionary"
      elsif(fieldNode.name == "set")
        @dataType = "ISet"
      end
      
      @readOnly = @isCollection = ['map', 'set'].include?(fieldNode.name)
      @initialValue = @@initialValues[@dataType] || ("new #{@dataType}()" if fieldNode.name == 'component')
  end
end

