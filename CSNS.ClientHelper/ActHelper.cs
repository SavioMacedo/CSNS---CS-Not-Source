using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSNS.ClientHelper
{
    public class ActHelper
    {
        public static Type Commands { get; set; }

        public static object Execute(string method, params object[] parameters)
        {
            if (Commands == null)
                throw new Exception("Personalized Commands arent defined.");

            var methodType = Commands.GetMethod(method);

            return methodType.Invoke(null, parameters);
        }
    }
}
