using Godot;

using System;

public partial class DayNightCycle : CanvasModulate {
    [Export] public int DayLengthSecs = 600;

    [Export] public Color DayColor = Colors.White;
    [Export] public Color NightColor = Colors.DarkGray;
    [Export] public bool BeginsDay = true;

    private Tween dayTween;

    public override void _Ready() {
        Color = BeginsDay ? DayColor : NightColor;
        dayTween = GetTree().CreateTween();
        dayTween.SetTrans(Tween.TransitionType.Sine);
        dayTween.TweenProperty(this, "color", NightColor, DayLengthSecs / 2);
        dayTween.TweenProperty(this, "color", DayColor, DayLengthSecs / 2);
        dayTween.SetLoops();
    }
}
