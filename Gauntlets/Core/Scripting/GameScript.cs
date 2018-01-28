using CraxAwesomeEngine;
using System;
using MoonSharp.Interpreter;
using System.IO;
using System.Collections.Generic;

namespace CraxAwesomeEngine.Core.Scripting
{

    /// <summary>
    /// TODO: Implement InputManager
    /// Implement Vector2Proxy static functions
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
            UserData.RegisterAssembly();
            Script.WarmUp();
            scriptList = new List<GameScript>();
        }

        public GameScript(string scriptName)
        {
            ScriptFileName = scriptName;
            script = new Script();
            script.DoFile(GetScriptFile(scriptName));

            if (script.Globals["update"] != null)
                scriptUpdateFunc = script.Globals.Get("update").Function;
            if (script.Globals["init"] != null)
                scriptInitFunc = script.Globals.Get("init").Function;
            if (script.Globals["destroy"] != null)
                scriptDestroyFunc = script.Globals.Get("destroy").Function;

            scriptList.Add(this);

        }

        public static void ResetScripts()
        {
            foreach (GameScript gameScript in scriptList)
            {
                Debug.Log("Resetting script {0}", gameScript.ScriptFileName);
                gameScript.script.DoFile(GetScriptFile(gameScript.ScriptFileName));
                gameScript.Initialize(gameScript.owner);
            }
        }

        public object Clone()
        {
            return null;
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
