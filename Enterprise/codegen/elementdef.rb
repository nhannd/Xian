require 'rexml/document'
require 'constants'

# Base class for all definitions in the model
# the Model itself is a subclass of ElementDef
# see other subclasses ClassDef, FieldDef
class ElementDef
  def namespace
  end
  
  # Used by the Template class to determine the binding context for all code
  # contained within the template.  The symbols in the template code are
  # resolved with respect to the binding.
  def get_binding
    binding
  end
end
