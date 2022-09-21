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

VeldridStartup.CreateWindowAndGraphicsDevice(
    new WindowCreateInfo(100, 100, 1080, 720, WindowState.Normal, "Testing"),
    out var window,
    out var gd
);

var renderer = new Gui.ImGuiRenderer(
    gd, gd.MainSwapchain.Framebuffer.OutputDescription,
    (int)gd.MainSwapchain.Framebuffer.Width, (int)gd.MainSwapchain.Framebuffer.Height
);

var cl = gd.ResourceFactory.CreateCommandList();

while (window.Exists)
{
    var input = window.PumpEvents();
    if (!window.Exists) { break; }
    renderer.Update(1f / 60f, input); // Compute actual value for deltaSeconds.

    var io = ImGui.GetIO();
    ImGui.SetNextWindowPos(Vector2.Zero);
    ImGui.SetNextWindowSize(new Vector2(io.DisplaySize.X, io.DisplaySize.Y));

    ImGui.Begin("Main", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);

    engine.Execute(new DocumentInfo { Category = ModuleCategory.Standard }, js);

    ImGui.End();

    cl.Begin();
    cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
    cl.ClearColorTarget(0, RgbaFloat.Black);
    renderer.Render(gd, cl);
    cl.End();
    gd.SubmitCommands(cl);
    gd.SwapBuffers(gd.MainSwapchain);
}

return 0;