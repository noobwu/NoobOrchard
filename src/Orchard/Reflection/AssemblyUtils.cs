using Orchard.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class AssemblyUtils
    {
        private const string FileUri = "file:///";
        private const char UriSeperator = '/';

        private static Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();
#if !XBOX
        /// <summary>
        /// Find the type from the name supplied
        /// </summary>
        /// <param name="typeName">[typeName] or [typeName, assemblyName]</param>
        /// <returns></returns>
        public static Type FindType(string typeName)
        {
            Type type = null;
            if (TypeCache.TryGetValue(typeName, out type)) return type;

#if !SL5
            type = Type.GetType(typeName);
#endif
            if (type == null)
            {
                var typeDef = new AssemblyTypeDefinition(typeName);
                type = !string.IsNullOrEmpty(typeDef.AssemblyName)
                    ? FindType(typeDef.TypeName, typeDef.AssemblyName)
                    : FindTypeFromLoadedAssemblies(typeDef.TypeName);
            }

            Dictionary<string, Type> snapshot, newCache;
            do
            {
                snapshot = TypeCache;
                newCache = new Dictionary<string, Type>(TypeCache) { [typeName] = type };

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref TypeCache, newCache, snapshot), snapshot));

            return type;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyBinPath(Assembly assembly)
        {
            var codeBase = GetAssemblyCodeBase(assembly);
            var binPathPos = codeBase.LastIndexOf(UriSeperator);
            var assemblyPath = codeBase.Substring(0, binPathPos + 1);
            if (assemblyPath.StartsWith(FileUri, StringComparison.OrdinalIgnoreCase))
            {
                assemblyPath = assemblyPath.Remove(0, FileUri.Length);
            }
            return assemblyPath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyCodeBase(Assembly assembly)
        {
            return assembly.CodeBase;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Type FindType(string typeName, string assemblyName)
        {
            var binPath = GetAssemblyBinPath(Assembly.GetExecutingAssembly());
            Assembly assembly = null;
            var assemblyDllPath = binPath + $"{assemblyName}.dll";
            if (File.Exists(assemblyDllPath))
            {
                assembly =LoadAssembly(assemblyDllPath);
            }
            var assemblyExePath = binPath + $"{assemblyName}.exe";
            if (File.Exists(assemblyExePath))
            {
                assembly = LoadAssembly(assemblyExePath);
            }
            return assembly != null ? assembly.GetType(typeName) : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static Assembly LoadAssembly(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static  Assembly[] GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type FindTypeFromLoadedAssemblies(string typeName)
        {
            var assemblies = GetAllAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

    }
}
