require 'elementdef'
require 'queryresultdef'
require 'querycriteriadef'
require 'model'
require 'type_name_utils'

class QueryDef < ElementDef
  attr_reader :queryName, :root, :joins, :resultFieldMappings, :criteriaFieldMappings
  
  def initialize(model, queryNode, defaultNamespace)
    @model = model
    @joins = []
    @queryName = TypeNameUtils.getShortName(queryNode.attributes['name'])
    @namespace = TypeNameUtils.getNamespace(queryNode.attributes['name']) || defaultNamespace
    queryNode.each_element do |subNode|
      case subNode.name
        when 'root'
          @root = QueryRoot.new(self, subNode, defaultNamespace)
        when 'join'
          @joins << QueryJoin.new(self, subNode)
        when 'result'
          @resultFieldMappings = createMappings(subNode);
          @resultClassName = TypeNameUtils.getQualifiedName(subNode.attributes['class'] || (@queryName + "Result"), defaultNamespace)
          @model.addDef(@resultClassName, QueryResultDef.new(model, @resultClassName, defaultNamespace, @resultFieldMappings))
        when 'criteria'
          @criteriaFieldMappings = createMappings(subNode);
          @criteriaClassName = TypeNameUtils.getQualifiedName(subNode.attributes['class'] || (@queryName + "Criteria"), defaultNamespace)
          @model.addDef(@criteriaClassName, QueryCriteriaDef.new(model, @criteriaClassName, defaultNamespace, @criteriaFieldMappings))
      end
    end
  end
  
  def elementName
    @queryName
  end
  
  def qualifiedName
    @namespace + "." + @queryName
  end
  
  def namespace
    @namespace
  end
  
  def resultClass
    @model.findDef(@resultClassName)
  end
  
  def criteriaClass
    @model.findDef(@criteriaClassName)
  end
  
  def resolveHqlSource(source)
    sourcePath = source.split('.')
    sourcePart = (@joins + [@root]).find {|part| part.hqlAlias == sourcePath[0]}
    resolveDataType(sourcePart.dataType, sourcePath[1..-1])
  end
  
protected
  def createMappings(baseNode)
    fields = []
    baseNode.each_element do |fieldNode|
      fields << QueryFieldMapping.new(self, fieldNode, fields.length)
    end
    return fields
  end
  
  def resolveDataType(dataType, pathParts)
    return dataType if(pathParts.length == 0)
    
    classDef = @model.findDef(dataType)
    fieldDef = classDef.findField(pathParts[0])
    return resolveDataType((fieldDef.kind == :collection) ? fieldDef.elementType : fieldDef.dataType, pathParts[1..-1])
  end
end

class QueryRoot
  attr_reader :dataType, :hqlAlias
  def initialize(queryDef, rootNode, defaultNamespace)
    @queryDef = queryDef
    @dataType = TypeNameUtils.getQualifiedName(rootNode.attributes['class'], defaultNamespace)
    @hqlAlias = rootNode.attributes['alias']
  end
end

class QueryJoin
  attr_reader :hqlAlias, :source
  def initialize(queryDef, joinNode)
    @queryDef = queryDef
    @hqlAlias = joinNode.attributes['alias']
    @source = joinNode.attributes['source']
  end
  
  def dataType
    return @dataType if @dataType != nil
    @dataType = @queryDef.resolveHqlSource(@source)
  end
end

class QueryFieldMapping
  attr_reader :name, :source, :index
  def initialize(queryDef, baseNode, index)
    @queryDef = queryDef
    @name = baseNode.attributes['name']
    @source = baseNode.attributes['source']
    @index = index
  end
  
  def dataType
    return @dataType if @dataType != nil
    @dataType = @queryDef.resolveHqlSource(@source)
  end
end

