using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSNS.Helpers
{
    public class CSNSMemory
    {
        private static Dictionary<string, Assembly> _assemblyMemories;
        public static Dictionary<string, Assembly> AssemblyMemories
        {
            get
            {
                return _assemblyMemories ?? new Dictionary<string, Assembly>();
            }
            set
            {
                _assemblyMemories = value;
            }
        }

        private static Assembly GetAssembly(string assemblyName)
        {
            if (!IsAssemblyLoad(assemblyName))
                throw new Exception("Assembly not loaded.");

            return AssemblyMemories.GetValueOrDefault(assemblyName);
        }

        public static async Task<Assembly> LoadAssemblyAsync(string path, string assemblyName, bool forceReload = false)
        {
            if(IsAssemblyLoad(assemblyName) && forceReload == false)
            {
                return GetAssembly(assemblyName);
            }

            Loader loader = new Loader(path, assemblyName);
            await loader.LoadAsync();
            var assembly = loader.Assembly;

            AssemblyMemories.Add(assemblyName, assembly);

            return assembly;
        }

        public static bool IsAssemblyLoad(string assemblyName)
        {
            return AssemblyMemories.ContainsKey(assemblyName);
        }
            
    }
}
