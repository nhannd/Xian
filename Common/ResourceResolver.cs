using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Common
{
    public class ResourceResolver
    {
        private static Dictionary<Assembly, List<Type>> _mapAsmToSRclass = new Dictionary<Assembly,List<Type>>();

        private Assembly[] _assemblies;

        public ResourceResolver(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public string Resolve(string key)
        {
            // search the assemblies in order
            for (int a = 0; a < _assemblies.Length; a++)
            {
                try
                {
                    string localized = Resolve(key, _assemblies[a]);
                    if (localized != null)
                    {
                        return localized;
                    }
                }
                catch (Exception)
                {
                    // failed to resolve in the specified assembly
                }
            }
            return key;     // return the unresolved string if not resolved
        }

        private static string Resolve(string str, Assembly asm)
        {
            List<Type> SRtypes = GetSRTypes(asm);
            foreach (Type SRclass in SRtypes)
            {
                PropertyInfo propertyInfo = SRclass.GetProperty(str, BindingFlags.Public | BindingFlags.Static);
                if (propertyInfo != null)
                {
                    MethodInfo getter = propertyInfo.GetGetMethod();
                    return (string)getter.Invoke(null, null);
                }
            }
            return null;
        }

        private static List<Type> GetSRTypes(Assembly asm)
        {
            if(!_mapAsmToSRclass.ContainsKey(asm))
            {
                _mapAsmToSRclass.Add(asm, new List<Type>());
                ProcessAssembly(asm);
            }
            return _mapAsmToSRclass[asm];
        }

        private static void ProcessAssembly(Assembly asm)
        {
            Type[] types = asm.GetTypes();
            foreach (Type t in types)
            {
                if (t.Name.IndexOf(".SR") == t.Name.Length-3)
                {
                    _mapAsmToSRclass[asm].Add(t);
                }
            }
        }
   }
}
