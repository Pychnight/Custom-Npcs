using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal static class Extensions
    {
        public static T CreateInstanceAndUnwrap<T>(this AppDomain domain)
        {
            Type type = typeof(T);

            return (T)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        public static T CreateInstanceAndUnwrap<T>(this AppDomain domain, params object[] args)
        {
            Type type = typeof(T);

            return (T)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true, BindingFlags.Default, null, args, null, null);
        }
    }
}
