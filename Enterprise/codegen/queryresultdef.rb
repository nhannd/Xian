require 'queryclassdef'

class QueryResultDef < QueryClassDef
  def initialize(model, className, mappings)
    super(model, className, mappings)
  end
  
  def superClassName
    "QueryResult"
  end    
end
