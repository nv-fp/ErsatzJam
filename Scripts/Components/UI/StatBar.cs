using Godot;

using System;

public partial class StatBar : Control {
    [Export] public Color Tint = Colors.Green;

    private TextureProgressBar progressBar;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        progressBar = GetNode<TextureProgressBar>("TextureProgressBar");
        progressBar.TintProgress = Tint;
    }

    public void SetValue(int v, int max) {
        int pct = 100 * v / max;
        progressBar.Value = pct;
    }
}
