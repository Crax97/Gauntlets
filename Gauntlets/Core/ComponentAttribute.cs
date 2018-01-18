using System;
using System.Collections.Generic;
namespace Gauntlets.Core
{
    public class ComponentAttribute : System.Attribute
    {

        private static Dictionary<string, Type> knownTypes = new Dictionary<string, Type>();

        public static Type GetAssociatedType(string type)
        {
            return knownTypes[type];
        }

        //(IComponent) cast is always true because we only add IComponent types 
        //in the knowTypes dictionary.
        public static IComponent GetInstanceOfType(string type)
        {

            return (IComponent)Activator.CreateInstance(knownTypes[type]);

        }

        public static void RegisterAttribute<T>(string name) where T: IComponent
        {
            knownTypes.Add(name, typeof(T));
        }
    }
}
