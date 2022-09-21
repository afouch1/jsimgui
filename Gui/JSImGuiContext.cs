using ImGuiNET;

namespace Gui;

public class JSImGuiContext
{
    public class JSImGuiTable
    {
        
    }
    
    public void text(string label)
    {
        ImGui.Text(label);
    }

    public string inputText(string label, string current, uint max = 100)
    {
        ImGui.InputText(label, ref current, max);
        return current;
    }
}