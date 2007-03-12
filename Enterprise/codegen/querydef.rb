require 'elementdef'
require 'queryresultdef'
require 'querycriteriadef'
require 'model'

class QueryDef < ElementDef
  attr_reader :queryName, :root, :joins, :resultFieldMappings, :criteriaFieldMappings
  
  def initialize(model, queryNode)
    @model = model
    @joins = []
    @queryName = queryNode.attributes['name']
    queryNode.each_element do |subNode|
      case subNode.name
        when 'root'
          @root = QueryRoot.new(self, subNode)
        when 'join'
          @joins << QueryJoin.new(self, subNode)
        when 'result'
          @resultFieldMappings = createMappings(subNode);
          @resultClassName = subNode.attributes['class'] || (@queryName + "Result")
          model.queryResultDefs << QueryResultDef.new(model, @resultClassName, @resultFieldMappings)
        when 'criteria'
          @criteriaFieldMappings = createMappings(subNode);
          @criteriaClassName = subNode.attributes['class'] || (@queryName + "Criteria")
          model.queryCriteriaDefs << QueryCriteriaDef.new(model, @criteriaClassName, @criteriaFieldMappings)
      end
    end
  end
  
  def elementName
    @queryName
  end
  
  def namespace
    @model.namespace
  end
  
  def resultClass
    @model.queryResultDefs.find { |r| r.className == @resultClassName }
  end
  
  def criteriaClass
    @model.queryCriteriaDefs.find { |c| c.className == @criteriaClassName }
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
    
    classDef = @model.findClass(dataType)
    fieldDef = classDef.findField(pathParts[0])
    return resolveDataType((fieldDef.kind == :collection) ? fieldDef.elementType : fieldDef.dataType, pathParts[1..-1])
  end
end

class QueryRoot
  attr_reader :dataType, :hqlAlias
  def initialize(queryDef, rootNode)
    @queryDef = queryDef
    @dataType = Model.fixDataType(rootNode.attributes['class'])
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

