using ImGuiNET;
using System;
using Veldrid;
using ClickableTransparentOverlay;
using System.Threading.Tasks;
using System.Numerics;

namespace OfflineChat;

internal class WindowAbstraction : Overlay
{
    public Vector2 SizeMin { get; set; }
    public Vector2 SizeMax { get; set; }

    public event Action OnExit;
    public readonly ImGuiWindowFlags windowState;
    public readonly Rectangle dimensions;

    private readonly string _title;
    private Action[] drawFuncs;

    public WindowAbstraction(string title, Rectangle dimensions) : base(title, true)
    {
        windowState = ImGuiWindowFlags.None;
        this.dimensions = dimensions;
        _title = title;

        drawFuncs = new[] { () => { } };
    }

    public WindowAbstraction(string title, ImGuiWindowFlags windowState, Rectangle dimensions) : base(title, true)
	{
        this.windowState = windowState;
        this.dimensions = dimensions;
        _title = title;

        drawFuncs = new[] { () => { } };
    }

    public void SetDrawFunc(Action drawFunc)
    {
        drawFuncs[0] = drawFunc;
    }

    public void SetDrawFunc(params Action[] drawFuncs)
	{
        this.drawFuncs = drawFuncs;
	}

    private string error = "";
    private bool errorOccured = false;
    private bool windowOpen = true;
	protected override Task Render()
	{
        //ImGui.ShowDemoWindow();
        ImGui.SetNextWindowSizeConstraints(SizeMin, SizeMax);
        ImGui.Begin(_title, ref windowOpen, windowState);

        if (!windowOpen)
		{
			OnExit?.Invoke();
            Environment.Exit(0);
		}

        ImGui.SetWindowPos(_title, dimensions.Position, ImGuiCond.Once);
        ImGui.SetWindowSize(_title, dimensions.Size, ImGuiCond.Once);

        try
        {
            foreach (var func in drawFuncs)
            {
                func();
            }
        }
        catch (Exception ex)
		{
            error = ex.Message;
            errorOccured = true;
        }

        if (errorOccured) {
            ImGui.SetNextWindowSizeConstraints(new(300, 40), new(800, 500));
            ImGui.Begin("ERROR", ref errorOccured, ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0, 0, 1)); // give red color
            ImGui.TextWrapped(error);
            ImGui.PopStyleColor();

            ImGui.End();
        }

        ImGui.End();

        return Task.CompletedTask;
    }
}

