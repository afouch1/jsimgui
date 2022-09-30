using ImGuiNET;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Gui;

public static class JSEngineConfig
{

    public static void RunApp(string title, ScriptObject app)
    {
        if (app.GetProperty("display") is not ScriptObject display)
        {
            throw new ScriptEngineException("Missing 'display' function from application object");
        }
    
        var context = new JSImGuiContext();

        var menu = app.GetProperty("menu") as ScriptObject;
        var hasMenu = menu is not null;
        
        ImGuiPresenter.Present(() =>
        {
            if (hasMenu && ImGui.BeginMenuBar())
            {
                menu?.Invoke(false, new JsImGuiMenuBar(context));
                ImGui.EndMenuBar();
            }
            display.Invoke(false, context);
        }, hasMenu);
    }
    
    public static void Configure(V8ScriptEngine engine)
    {
        engine.AddHostObject("runApp", new Action<string, ScriptObject>(RunApp));
        engine.AddHostObject("log", new Action<object>(text => Console.WriteLine(text)));
        engine.Script.isStandAlone = false;
    }
}