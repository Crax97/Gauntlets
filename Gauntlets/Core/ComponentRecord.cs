using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;

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
            public Func<XmlNode, Game, object> initializer { get; internal set; }
        }

        private static Dictionary<string, ComponentTypeAndInitializer> knownTypes = new Dictionary<string, ComponentTypeAndInitializer>();

        public static Type GetAssociatedType(string type)
        {
            return knownTypes[type].t;
        }

        public static IComponent CreateInstance(string which, XmlNode node, Game game) {
            return (IComponent)knownTypes[which].initializer(node, game);
        }

        public static void RegisterAttribute<T>(string name, Func<XmlNode, Game, object> initializerFunction) where T: IComponent
        {
            ComponentTypeAndInitializer typeAndInitializer = new ComponentTypeAndInitializer();
            typeAndInitializer.t = typeof(T);
            typeAndInitializer.initializer = initializerFunction;
            knownTypes.Add(name, typeAndInitializer);
            UserData.RegisterType<T>();
        }
    }
}
