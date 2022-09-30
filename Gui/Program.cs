// See https://aka.ms/new-console-template for more information

using System.Numerics;
using Gui;
using ImGuiNET;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Veldrid;
using Veldrid.StartupUtilities;

if (args.Length != 1)
{
    Console.WriteLine("Invalid argument(s). Missing Javascript file. ");
    return 1;
}

var js = File.ReadAllText(args[0]);

var engine = new V8ScriptEngine();

JSEngineConfig.Configure(engine);

try
{
    engine.Execute(new DocumentInfo { Category = ModuleCategory.Standard }, js);
}
catch (Exception e)
{
    var removeErrorPrefix = e.Message.StartsWith("Error: ") ? e.Message.Remove(0, 7): e.Message;
    var colonIndex = removeErrorPrefix.IndexOf(":");
    var errorType = removeErrorPrefix.Substring(0, colonIndex + 1);
    var message = removeErrorPrefix.Substring(colonIndex + 1);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" ");
    Console.Write(errorType);
    Console.ResetColor();
    Console.WriteLine(message);
    Console.WriteLine(" ");
}

return 0;