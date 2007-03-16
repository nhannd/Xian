require 'queryclassdef'

class QueryResultDef < QueryClassDef
  def initialize(model, className, defaultNamespace, mappings)
    super(model, className, defaultNamespace, mappings)
  end
  
  def superClassName
    "QueryResult"
  end    
end
