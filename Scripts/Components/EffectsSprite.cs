using Godot;

using System;

public partial class EffectsSprite : AnimatedSprite2D {
    [Export] public string AttackAnimationName = "attack";
    [Export] public uint HitAnimationFrame = 3;
    [Signal] public delegate void CheckHitEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        Connect(SignalName.FrameChanged, Callable.From(() => OnFrameChanged()));
    }

    private void OnFrameChanged() {
        if (Animation == AttackAnimationName && Frame == HitAnimationFrame) {
            EmitSignal(SignalName.CheckHit);
        }
    }
}
