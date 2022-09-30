using System.Collections;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Gui;

public static class Utilities
{
    public static ScriptObject ToJSArray(this IEnumerable list)
    {
        var ArrayConstructor = (ScriptObject) ScriptEngine.Current.Script.Array;
        dynamic arr = ArrayConstructor.Invoke(true);
        foreach (var item in list)
        {
            arr.push(item);
        }
            
        return arr;
    }
}