require 'elementdef'
require 'entitydef'
require 'enumdef'
require 'componentdef'
require 'querydef'

# Represents a domain model defined by a set of NHibernate XML mappings
class Model < ElementDef
  attr_reader :namespace, :entityDefs, :enumDefs, :componentDefs, :queryDefs, :queryCriteriaDefs, :queryResultDefs
  
  def initialize()
    @entityDefs = []
    @enumDefs = []
    @componentDefs = []
    @queryDefs = []
    @queryCriteriaDefs = []
    @queryResultDefs = []
    
    @symbolSpace = []
  end
  
  def elementName
    @namespace + " Model"
  end
  
  # add the specified file to the model
  # suppressCodeGen is a boolean that will suppress code generation for the classes defined in the specified file (hbm only)
  def add(fileName, suppressCodeGen)
    case
      when fileName.include?('.hbm.xml') : addHbmFile(fileName, suppressCodeGen)
      when fileName.include?('.hrq.xml') : addHrqFile(fileName)
    end
  end
  
  # returns the last component of the namespace
  def shortName
    @namespace.split('.')[-1]
  end
  
  # searches entityDefs, componentDefs and enumDefs for the specified class, and returns the ClassDef or nil if not found
  def findClass(className)
    (@entityDefs + @componentDefs + @enumDefs).find {|c| c.className == className}
  end

  # processes componentNode to create instances of ComponentDef
  # this method is public because it must be called from the ClassDef class - it should not be called otherwise
  def processComponent(componentNode)
    # only create a ComponentDef if the component is in the same namespace as this model
    # (otherwise the component is defined in another namespace, and is simply being referenced by this model)
    if(extractNamespace(componentNode.attributes['class']) == @namespace)
	    componentDef = ComponentDef.new(self, componentNode)
	    @componentDefs << componentDef if(!@symbolSpace.include?(componentDef.className))
    end
  end
  
protected
  def addHbmFile(hbmFile, suppressCodeGen)
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
            processEntity(c, c.attributes['extends'] || "Entity", @entityDefs, suppressCodeGen)
          end
        end
      end
    end
  end
  
  def addHrqFile(hrqFile)
    # read the hrq xml file
    mappings = REXML::Document.new(File.new(hrqFile))
    # extract the namespace - TODO throw exception if model already defined and doesn't match
    @namespace = mappings.root.attributes['namespace'] if @namespace == nil
    
    # process each query in the hrq file
    mappings.root.each_element do |queryNode|
      if(queryNode.name == 'query')
        queryName = queryNode.attributes['name']
        
        # check if this class name already exists
        if(!@symbolSpace.include?(queryName))
          queryDef = QueryDef.new(self, queryNode)
          @queryDefs << queryDef
          @symbolSpace << queryDef.queryName
        end
      end
    end
  end
  
  # processes classNode to create instances of EntityDef
  def processEntity(classNode, superClassName, entityDefs, suppressCodeGen)
    #create EntityDef for classNode
    entityDef = EntityDef.new(self, classNode, superClassName)
    entityDef.suppressCodeGen = suppressCodeGen
    entityDefs << entityDef
    @symbolSpace << entityDef.className
    
    #process any contained subclassses recursively
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
  
  def extractNamespace(name)
	  #find the last .
	  p =name.rindex('.')
	  
	  # if no dot was found, then assume the default namespace (the namespace of this model)
	  return p ? name[0..p] : @namespace
  end
 
  def Model.fixDataType(name)
    # remove the assembly name if present
    name = Model.removeAssemblyQualifier(name)
    
    # remove the namespace
    name = name.split('.')[-1]
    
    # remove "EnumHbm" or "Hbm" - this is a hack to extract the underlying datatype from "mapper" types
    name = name.sub(/EnumHbm$/, "").sub(/Hbm$/, "")
  end
  
  def Model.removeAssemblyQualifier(name)
    commaPos = name.index(",")
    commaPos ? name[0, commaPos] : name
  end
end
