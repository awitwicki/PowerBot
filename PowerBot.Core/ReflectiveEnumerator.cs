using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PowerBot.Core
{
    public static class ReflectiveEnumerator
    {
        public static IEnumerable<Type> GetEnumerableOfType<T>() where T : class
        {
            List<Type> classes = AppDomain.CurrentDomain
                .GetAssemblies()
                .ToList()
                .SelectMany(x =>
                    x.GetTypes()
                        .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                        .ToList()
                ).ToList();

            return classes;
        }
    }
}
