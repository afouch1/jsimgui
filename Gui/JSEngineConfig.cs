using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Gui;

public static class JSEngineConfig
{

    public static void RunApp(string title, ScriptObject app)
    {
        if (!(app.GetProperty("display") is ScriptObject display))
        {
            throw new ScriptEngineException("Missing 'display' function from application object");
        }
    
        var context = new JSImGuiContext();
    
        display.Invoke(false, context);
    }
    
    public static void Configure(V8ScriptEngine engine)
    {
        engine.AddHostObject("runApp", new Action<string, ScriptObject>(RunApp));
    }
}