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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
}
