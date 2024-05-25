using JamEnums;

using Godot;

using System;

public partial class Player : CharacterBody2D {
    [Export]
    public float WalkSpeed = 120f;

    [Export]
    public float RunSpeed = 200;

    [Export]
    public bool DrawDebugOverlays { get; set; } = true;

    public float facing { get; private set; } = 0f;
    private Direction direction = Direction.Up;
    private MoveMode moveMode = MoveMode.Walk;

    // references to player nodes
    private AnimatedSprite2D sprite;
    private Area2D interactableCheck;
    private CollisionShape2D interactableSensorShape;

    // debug related shit
    static Color sensorDebugColor = new Color(0f, 1f, .1f, .5f);

    public override void _Ready() {
        base._Ready();
        sprite = GetNode<AnimatedSprite2D>("Sprite");
        interactableCheck = GetNode<Area2D>("InteractionCheck");
        interactableSensorShape = interactableCheck.GetNode<CollisionShape2D>("Sensor");
    }

    private float getSpeed() {
        return moveMode == MoveMode.Walk ? WalkSpeed : RunSpeed;
    }

    public override void _PhysicsProcess(double delta) {
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
        Velocity = direction * getSpeed();

        MoveAndSlide();
        if (DrawDebugOverlays) {
            QueueRedraw();
        }
    }

    private Direction angleToDirection(double angle) {
        var normalized = angle / (Math.PI * 2) * 360;
        switch (Math.Abs(normalized)) {
            case <= 45:
                return Direction.Up;
            case <= 135:
                return normalized < 0
                    ? Direction.Right
                    : Direction.Left;
            case <= 180:
                return Direction.Down;
        }
        return Direction.Up;
    }

    static Vector2[] sensorPosition = new Vector2[4] {
        new Vector2(0, -45), // up
        new Vector2(30,  5), // right
        new Vector2(0,  40), // down
        new Vector2(-30, 5), // left
    };
    static Vector2 sensorScaledUp = new Vector2(1, 3);
    static Vector2 sensorNormal = new Vector2(1, 1);
    private void maybeUpdateSensorPosition() {
        switch (direction) {
            case Direction.Up:
                interactableSensorShape.Position = sensorPosition[0];
                interactableSensorShape.Scale = sensorNormal;
                break;
            case Direction.Right:
                interactableSensorShape.Position = sensorPosition[1];
                interactableSensorShape.Scale = sensorScaledUp;
                break;
            case Direction.Down:
                interactableSensorShape.Position = sensorPosition[2];
                interactableSensorShape.Scale = sensorNormal;
                break;
            case Direction.Left:
                interactableSensorShape.Position = sensorPosition[3];
                interactableSensorShape.Scale = sensorScaledUp;
                break;
        }
    }

    public override void _Draw() {
        base._Draw();
        if (!DrawDebugOverlays) {
            return;
        }

        #region draw interactable sensor rect
        var shapeRect = interactableSensorShape.Shape.GetRect();
        var shapeSize = shapeRect.Size * interactableSensorShape.Scale;
        var sensorXy = interactableCheck.Position + interactableSensorShape.Position;
        sensorXy -= shapeSize / 2;
        var drawnRect = new Rect2(sensorXy, shapeSize);
        DrawRect(drawnRect, sensorDebugColor, true, 2);
        #endregion
    }

    public override void _Process(double delta) {
        moveMode = Input.IsActionPressed(KeyName.PadX.Name())
            ? MoveMode.Run
            : MoveMode.Walk;
        maybeUpdateSensorPosition();
        var facing = Velocity.AngleTo(Vector2.Up);

        direction = angleToDirection(facing);
        if (Velocity == Vector2.Zero) {
            sprite.Play("idle");
        } else {
            sprite.Play(moveMode.Name() + "_" + Util.DirectionToString(direction));
        }
    }


    private void InteractableEnter(Area2D area) {
        GD.Print($"found interactable object: " + area.GetParent());
    }


    private void InteractableExit(Area2D area) {
        GD.Print($"lost interactable object: " + area.GetParent());
    }
}
