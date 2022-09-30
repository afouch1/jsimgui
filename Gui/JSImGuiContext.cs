using System.Collections;
using System.Numerics;
using ImGuiNET;
using Microsoft.ClearScript;

namespace Gui;

public class JsImGuiTable
{
    public void headers(params string[] texts)
    {
        foreach (var header in texts)
        {
            ImGui.TableSetupColumn(header);
        }

        ImGui.TableHeadersRow();
    }

    public JsImGuiTable()
    {
        
    }

    public void row(params string[] values)
    {
        ImGui.TableNextRow();
        foreach (var value in values)
        {
            ImGui.TableNextColumn();
            ImGui.Text(value);
        }
    }
}

public class JsImGuiMenu
{
    private JSImGuiContext _context;

    public JsImGuiMenu(JSImGuiContext context)
    {
        _context = context;
    }

    public bool menuItem(string label, bool enabled = true)
    {
        return ImGui.MenuItem(label, enabled);
    }

    public bool checkbox(string label, bool isChecked)
    {
        ImGui.MenuItem(label, "", ref isChecked);
        return isChecked;
    }

    public void beginMenu(string label, ScriptObject fn)
    {
        if (ImGui.BeginMenu(label))
        {
            fn.Invoke(false, this);
            ImGui.EndMenu();
        }
    }
}

public class JsImGuiMenuBar
{
    private JSImGuiContext _context;

    public JsImGuiMenuBar(JSImGuiContext context)
    {
        _context = context;
    }
    
    public void beginMenu(string label, ScriptObject fn)
    {
        if (ImGui.BeginMenu(label))
        {
            var menu = new JsImGuiMenu(_context);
            fn.Invoke(false, menu, _context);
            ImGui.EndMenu();
        }
    }
}

public class JSImGuiContext
{
    public void text(string text)
    {
        ImGui.Text(text);
    }

    public bool button(string label)
    {
        return ImGui.Button(label);
    }

    public dynamic acad(ScriptObject fn)
    {
        return fn.Invoke(false);
    }

    public void sameLine(float offset = 0)
    {
        ImGui.SameLine(offset);
    }

    public void box(string label, int width, int height, ScriptObject fn)
    {
        ImGui.BeginChild(label, new Vector2(width, height), true);
        fn.Invoke(false, this);
        ImGui.EndChild();
    }

    public bool selectable(string text, bool isSelected)
    {
        ImGui.Selectable(text, ref isSelected);
        return isSelected;
    }

    public void group(string label, ScriptObject fn)
    {
        ImGui.BeginGroup();
        fn.Invoke(false, this);
        ImGui.EndGroup();
    }

    public bool collapsableHeader(string label)
    {
        return ImGui.CollapsingHeader(label);
    }

    public bool window(string title, bool isOpen, ScriptObject fn)
    {
        if (isOpen)
        {
            ImGui.Begin(title, ref isOpen);
            fn.Invoke(false, this);
            ImGui.End();
        }

        return isOpen;
    }

    public void separator()
    {
        ImGui.Separator();
    }

    public int combo(string label, IEnumerable options, int current)
    {
        var items = options.OfType<string>().ToArray();
        ImGui.Combo(label, ref current, items, items.Length);
        return current;
    }

    public int inputInt(string label, int value)
    {
        ImGui.InputInt(label, ref value);
        return value;
    }

    public double inputDouble(string label, double value)
    {
        ImGui.InputDouble(label, ref value);
        return value;
    }

    private static Vector4[] ColorPalette = CreatePallete();

    private static Vector4[] CreatePallete()
    {
        var savedPalette = new Vector4[32];
        for (int i = 0; i < savedPalette.Length; i++)
        {
            ImGui.ColorConvertHSVtoRGB(i / 31f, 0.8f, 0.8f, out savedPalette[i].X, out savedPalette[i].Y,
                out savedPalette[i].Z);
            savedPalette[i].W = 1.0f;
        }

        return savedPalette;
    }

    public void indent(ScriptObject fn)
    {
        ImGui.Indent(16);
        fn.Invoke(false, this);
        ImGui.Unindent();
    }

    public bool checkbox(string label, bool value)
    {
        ImGui.Checkbox(label, ref value);
        return value;
    }

    public void disableIf(bool shouldDisable, ScriptObject fn)
    {
        if (shouldDisable)
            ImGui.BeginDisabled();
        fn.Invoke(false, this);
        if (shouldDisable)
            ImGui.EndDisabled();
    }

    public object colorPicker(string id, IEnumerable input)
    {
        var arr = input.OfType<dynamic>().Select(x => (float)Convert.ToSingle(x)).ToArray();
        if (arr.Length != 4)
        {
            throw new ScriptEngineException("Input to colorPickerWithButton was not an array of 4 numbers");
        }

        var color = new Vector4(arr[0], arr[1], arr[2], arr[3]);
        var openPopup = ImGui.ColorButton("##MyColor", color);
        ImGui.SameLine();
        openPopup = openPopup || ImGui.Button("Palette");
        if (openPopup)
        {
            ImGui.OpenPopup(id);
        }

        if (ImGui.BeginPopup(id))
        {
            ImGui.ColorPicker4("##picker", ref color,
                ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview);
            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.Text("Current");
            ImGui.ColorButton("##current", color,
                ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.AlphaPreviewHalf, new Vector2(60, 40));
            ImGui.Separator();
            ImGui.Text("Palette");
            for (int i = 0; i < ColorPalette.Length; i++)
            {
                ImGui.PushID(i);
                if ((i % 8) != 0)
                    ImGui.SameLine();

                var flags = ImGuiColorEditFlags.NoAlpha | ImGuiColorEditFlags.NoPicker |
                            ImGuiColorEditFlags.NoTooltip;

                if (ImGui.ColorButton("##palette", ColorPalette[i], flags, new Vector2(20, 20)))
                    color = new Vector4(ColorPalette[i].X, ColorPalette[i].Y, ColorPalette[i].Z, color.W);

                ImGui.PopID();
            }

            ImGui.EndGroup();
            ImGui.EndPopup();
        }

        return new[]
        {
            color.X,
            color.Y,
            color.Z,
            color.W
        }.ToJSArray();
    }

    public string inputText(string label, string value, uint length = 100)
    {
        ImGui.InputText(label, ref value, length);
        return value;
    }

    public void table(string label, int columns, ScriptObject fn)
    {
        if (ImGui.BeginTable(label, columns,
                ImGuiTableFlags.Borders | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Resizable))
        {
            var imguiTable = new JsImGuiTable();
            fn.Invoke(false, imguiTable);
            ImGui.EndTable();
        }
    }
}