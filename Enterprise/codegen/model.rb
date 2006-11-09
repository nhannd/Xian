require 'elementdef'
require 'entitydef'
require 'enumdef'
require 'componentdef'

# Represents a domain model defined by a set of NHibernate XML mappings
class Model < ElementDef
  attr_reader :namespace, :entityDefs, :enumDefs, :componentDefs
  
  def initialize()
    @entityDefs = []
    @enumDefs = []
    @componentDefs = []
    @symbolSpace = []
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
