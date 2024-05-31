using Godot;

public partial class DebugGrid : Node2D {
    private float width;
    private float height;

    [Export] public int StepSize = 50;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        var rect = GetViewport().GetVisibleRect();
        width = rect.Size.X;
        height = rect.Size.Y;
        QueueRedraw();
    }

    public override void _Draw() {
        for (var x = 0; x <= width; x += StepSize) {
            DrawLine(new Vector2(x, 0), new Vector2(x, height), Colors.Green, 2);
            for (var y = 0; y <= height; y += StepSize) {
                DrawLine(new Vector2(0, y), new Vector2(width, y), Colors.Green, 2);
            }
        }

        var gx = GlobalPosition.X;
        var gy = GlobalPosition.Y;
        if (gx != 0 && gy != 0) {
            for (var x = 0 - gx; x <= width; x += StepSize) {
                DrawLine(new Vector2(x, 0), new Vector2(x, height + gy), Colors.Yellow, 2);
                for (var y = 0 - gy; y <= height; y += StepSize) {
                    DrawLine(new Vector2(0, y), new Vector2(width + gx, y), Colors.Yellow, 2);
                }
            }
        }
    }
}
