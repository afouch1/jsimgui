using System.Numerics;
using ImGuiNET;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Gui;

public static class ImGuiPresenter
{
    public static void Present(Action fn, bool hasMenu)
    {
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

        var flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoTitleBar;

        if (hasMenu) flags |= ImGuiWindowFlags.MenuBar;

        while (window.Exists)
        {
            var input = window.PumpEvents();
            if (!window.Exists)
            {
                break;
            }

            renderer.Update(1f / 60f, input); // Compute actual value for deltaSeconds.

            var io = ImGui.GetIO();

            ImGui.GetStyle().FrameRounding = 2.0f;
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(new Vector2(io.DisplaySize.X, io.DisplaySize.Y));

            ImGui.Begin("Main", flags);

            fn();

            ImGui.End();

            cl.Begin();
            cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Black);
            renderer.Render(gd, cl);
            cl.End();
            gd.SubmitCommands(cl);
            gd.SwapBuffers(gd.MainSwapchain);
        }
    }
}