using CraxAwesomeEngine;
using System;
using MoonSharp.Interpreter;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CraxAwesomeEngine.Core.GUI;
using Microsoft.Xna.Framework.Input;
using CraxAwesomeEngine.Core.Debugging;
using Microsoft.Xna.Framework.Graphics;

namespace CraxAwesomeEngine.Core.Scripting
{

    /// <summary>
    /// TODO: Implement InputManager
    /// Implement Vector2 static functions
    /// Write documentation
    /// Implement XML reader for this class
    /// </summary>
    class GameScript : IComponent
    {
        private static List<GameScript> scriptList = null;

        private Script script;

        private Closure scriptUpdateFunc = null;
        private Closure scriptInitFunc = null;
        private Closure scriptDestroyFunc = null;

        private Entity owner = null;

        public string ScriptFileName { get; private set; } = null;

        public static void InitGameScript()
        {

            //Registering the most used MonoGame types.
            //IComponents are registered in ComponentRecord.RegisterComponent<T>()
            UserData.RegisterType<Vector2>();
            UserData.RegisterType<Point>();
            UserData.RegisterType<Keys>();
            UserData.RegisterType<MouseKeys>(); //This should have a [MoonSharpUserData] attribute, but for consistency i register it there
            UserData.RegisterType<SpriteEffects>();

            UserData.RegisterAssembly();

            Script.WarmUp();
            scriptList = new List<GameScript>();
        }

        public GameScript(string scriptName)
        {
            ScriptFileName = scriptName;
            InitScript(scriptName);
            scriptList.Add(this);
        }

        private void InitScript(string scriptName)
        {
            script = new Script();
            script.Globals["Vector2"] = typeof(Vector2);
            script.Globals["Transform"] = typeof(Transform);
            script.Globals["Input"] = typeof(InputManager);
            script.Globals["Keys"] = typeof(Keys);
            script.Globals["MouseKeys"] = typeof(MouseKeys);
            script.Globals["Globals"] = Globals.Instance;
            script.Globals["RenderingEffect"] = typeof(SpriteEffects);

            try
            {
                script.DoFile(GetScriptFile(scriptName));
                if (script.Globals["update"] != null)
                    scriptUpdateFunc = script.Globals.Get("update").Function;
                if (script.Globals["init"] != null)
                    scriptInitFunc = script.Globals.Get("init").Function;
                if (script.Globals["destroy"] != null)
                    scriptDestroyFunc = script.Globals.Get("destroy").Function;

            }
            catch (InterpreterException ex)
            {
                script = null;
                Debug.Error(ex.DecoratedMessage);
            }
        }

        public static void ResetScripts()
        {
            foreach (GameScript gameScript in scriptList)
            {
                Debug.Log("Resetting script {0}", gameScript.ScriptFileName);
                gameScript.InitScript(gameScript.ScriptFileName);
                gameScript.Initialize(gameScript.owner);
            }
        }

        public object Clone()
        {
            return new GameScript(ScriptFileName);
        }

        public void Destroy()
        {
            if (scriptDestroyFunc != null)
                try
                {
                    scriptDestroyFunc.Call();
                }
                catch (InterpreterException ex)
                {
                    Debug.Log(ex.DecoratedMessage);
                }
        }

        public void Initialize(Entity owner)
        {
            this.owner = owner;

            if (scriptInitFunc != null) try
                {
                    scriptInitFunc.Call(owner);
                }
                catch (InterpreterException ex)
                {
                    Debug.Log(ex.DecoratedMessage);
                }
        }

        public void Update(float deltaTime, Entity parent)
        {
            if (scriptUpdateFunc != null)
            try
            {
                scriptUpdateFunc.Call(deltaTime, parent);
            }
                catch (InterpreterException ex)
            {
                Debug.Log(ex.DecoratedMessage);
            }
        }

        private static string GetScriptFile(string scriptName)
        {
            return Path.Combine("Content", "Scripts", scriptName);
        }
    }
}
