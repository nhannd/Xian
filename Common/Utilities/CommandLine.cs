using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Parses command line arguments.  May be used directly, or may be subclassed and used in conjunction with
    /// <see cref="CommandLineParameterAttribute"/> to have strongly-typed resolution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Parses an array of command line arguments. Each argument is treated as either a named parameter,
    /// a positional parameter, or a switch, depending on its format.  Arguments may be quoted to allow spaces -
    /// the .NET runtime removes the quotes prior to providing the arguments to application code.
    /// </para>
    /// <para>
    /// A named parameter has the form 
    ///     /param:value
    ///     -param:value
    ///     /param=value
    ///     -param=value 
    /// 
    /// There must not be a space between the : or = and the value.  Named parameters are stored in the
    /// <see cref="Named"/> property.
    /// </para>
    /// <para>
    /// A switch has the form
    ///     /switch[+-]
    ///     -switch[+-]
    /// 
    /// The trailing + or - is optional, and if omitted, the boolean state of the switch is simply toggled.
    /// Switch states are stored in the <see cref="Switches"/> property.
    /// </para>
    /// <para>
    /// A parameter that does not match either the named or switch form is interpreted as a positional parameter.
    /// Positional parameters are stored in the <see cref="Positional"/> property, in the order in which they occur.
    /// </para>
    /// </remarks>
    public class CommandLine
    {
        private readonly Dictionary<string, string> _namedArgs = new Dictionary<string, string>();
        private readonly List<string> _positionalArgs = new List<string>();
        private readonly Dictionary<string, bool> _switches = new Dictionary<string, bool>();

        private readonly Regex _namedArgRegex = new Regex(@"^[\/\-](\w+)[:=](.*)$", RegexOptions.Compiled);
        private readonly Regex _boolSwitchRegex = new Regex(@"^[\/\-](\w+)([\+\-]?)$", RegexOptions.Compiled);

        /// <summary>
        /// Constructs an instance of this class for the specified set of arguments.
        /// </summary>
        /// <param name="args"></param>
        public CommandLine(string[] args)
        {
            ProcessArgs(args);
            ProcessAttributes();
        }

        /// <summary>
        /// Gets the set of named arguments.
        /// </summary>
        public IDictionary<string, string> Named
        {
            get { return _namedArgs; }
        }

        /// <summary>
        /// Gets the set of positional arguments.
        /// </summary>
        public IList<string> Positional
        {
            get { return _positionalArgs; }
        }

        /// <summary>
        /// Gets the set of switches.
        /// </summary>
        public IDictionary<string, bool> Switches
        {
            get { return _switches; }
        }

        #region Private Helpers

        private void ProcessArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (TryParseNamedArg(arg))
                {
                }
                else if (TryParseBoolSwitch(arg))
                {
                }
                else
                {
                    _positionalArgs.Add(arg);
                }
            }
        }

        private void ProcessAttributes()
        {
            ObjectWalker walker = new ObjectWalker(
                delegate (IObjectMemberContext context)
                {
                    CommandLineParameterAttribute a = AttributeUtils.GetAttribute<CommandLineParameterAttribute>(context.Member);
                    if(a != null)
                    {
                        if(a.Position > -1)
                        {
                            // treat as positional
                            ValidateType(context.Member.Name, new Type[]{typeof(string)}, context.MemberType);
                            if (_positionalArgs.Count > a.Position)
                                context.MemberValue = _positionalArgs[a.Position];
                            else
                            {
                                if(a.Required)
                                    throw new CommandLineException(string.Format("Missing required command line argument <{0}>", a.DisplayName));
                            }
                        }
                        else
                        {
                            // treat as named/switch
                            if (context.MemberType == typeof(bool))
                            {
                                ValidateType(context.Member.Name, new Type[] { typeof(bool) }, context.MemberType);
                                SetMemberValue(a, context, _switches);
                            }
                            else
                            {
                                ValidateType(context.Member.Name, new Type[] { typeof(string), typeof(int), typeof(Enum) }, context.MemberType);
                                SetMemberValue(a, context, _namedArgs);
                            }
                        }
                    }
                });

            walker.IncludeNonPublicFields = true;
            walker.IncludeNonPublicProperties = true;
            walker.Walk(this);
        }

        /// <summary>
        /// Sets the value of a member property/field from the specified source dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        private void SetMemberValue<T>(CommandLineParameterAttribute a, IObjectMemberContext context, IDictionary<string, T> source)
        {
            // T will be either bool or string, depending on whether source is "Switches" or "Named"
            foreach (string key in a.Keys)
            {
                if(source.ContainsKey(key))
                {
                    T value = source[key];
                    if(context.MemberType == typeof(T))
                        context.MemberValue = value;
                    else if(context.MemberType.IsEnum)
                    {
                        context.MemberValue = Enum.Parse(context.MemberType, value.ToString());
                    }
                    else if(context.MemberType == typeof(int))
                    {
                        context.MemberValue = int.Parse(value.ToString());
                    }
                    return;
                }
            }
            if (a.Required)
                throw new CommandLineException(
                    string.Format("Missing required command line argument <{0}> ({1})",
                    a.DisplayName,
                    StringUtilities.Combine(a.Keys, ", ")));
        }

        private void ValidateType(string memberName, Type[] allowable, Type actual)
        {
            if(!CollectionUtils.Contains(allowable, delegate (Type t) { return t.IsAssignableFrom(actual);}))
                throw new InvalidOperationException(
                    string.Format("Property/field {0} cannot be of type {1} - must be of type {2}",
                        memberName,
                        actual,
                        StringUtilities.Combine(allowable, ", ")));
        }

        private bool TryParseNamedArg(string arg)
        {
            Match match = _namedArgRegex.Match(arg);
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups.Count > 2 ? match.Groups[2].Value : "";
                _namedArgs[key] = value;
                return true;
            }
            return false;
        }

        private bool TryParseBoolSwitch(string arg)
        {
            Match match = _boolSwitchRegex.Match(arg);
            if (match.Success)
            {
                string key = match.Groups[1].Value;
                if (!_switches.ContainsKey(key))
                    _switches.Add(key, false);

                // check for a + or -, indicating true or false
                string value = match.Groups.Count > 2 ? match.Groups[2].Value : "";
                if (value == "+")
                    _switches[key] = true;
                else if (value == "-")
                    _switches[key] = false;
                else
                    // if no value passed explicitly, then toggle the state of the switch
                    _switches[key] = !_switches[key];

                return true;
            }
            return false;
        }

        #endregion
    }
}
