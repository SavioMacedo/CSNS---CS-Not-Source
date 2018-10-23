using CSNS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CSNS.Static
{
    public static class CSNSStaticLoader
    {
        public delegate object DelegateMethod(params object[] parameters);
        
        public static void LoadCode(Assembly assembly)
        {
            var types = assembly.GetTypes();

            //Verify if a class has a runType.
            var runTypes = types.Where(obj => obj.GetMethod("Run") != null);
            
            //Execute run methods, the initializer of a class.
            foreach(var type in runTypes)
            {
                RunClass(type);
            }
        }

        public static void RunClass(Type type)
        {
            var runType = type.GetMethod("Run");
            runType.Invoke(null, new object[] { });
        }

        public static DelegateMethod LoadMethod(Assembly assembly, string metodoNamespace)
        {
            var methodName = metodoNamespace.Split('.').LastOrDefault();
            metodoNamespace = string.Join('.', metodoNamespace.Split('.').TakeWhile(obj => obj != methodName));

            var method = assembly.GetType(metodoNamespace).GetMethod(methodName);
            var methodParameters = method.GetParameters();
            var returnType = method.ReturnType;

            object methodDe(object[] parameters)
            {
                return assembly.GetType(metodoNamespace).GetMethod(methodName).Invoke(null, parameters);
            }

            return methodDe;
        }

        public static dynamic InstanceClass(Assembly assembly, string classNamespace, params object[] classParameters)
        {
            var classAssembly = assembly.GetType(classNamespace);
            var constructors = classAssembly.GetConstructors();

            if (!constructors.Any())
                throw new Exception("A classe citada não possui construtores");

            dynamic obj = Activator.CreateInstance(classAssembly, classParameters);

            return obj;
        }
    }
}
