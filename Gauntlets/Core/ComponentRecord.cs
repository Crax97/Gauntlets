using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;

using CraxAwesomeEngine.Core.Physics;

namespace CraxAwesomeEngine.Core
{
    /// <summary>
    /// Static class that holds informations regarding the various IComponents
    /// </summary>
    public class ComponentRecord
    {

        private struct ComponentTypeAndInitializer
        {
            public Type t { get; internal set; }
        }

        private static Dictionary<string, ComponentTypeAndInitializer> knownTypes = new Dictionary<string, ComponentTypeAndInitializer>();

        public static Type GetAssociatedType(string type)
        {
            return knownTypes[type].t;
        }

        public static void RegisterAttribute<T>(string name) where T: IComponent
        {
            ComponentTypeAndInitializer typeAndInitializer = new ComponentTypeAndInitializer();
            typeAndInitializer.t = typeof(T);
            knownTypes.Add(name, typeAndInitializer);
            UserData.RegisterType<T>();
        }
    }
}
