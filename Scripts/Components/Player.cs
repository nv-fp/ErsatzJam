using JamEnums;

using Godot;

using System;

public partial class Player : CharacterBody2D {
    [Export]
    public float WalkSpeed = 120f;

    [Export]
    public float RunSpeed = 200;

    [Export]
    public float DashSpeed = 600;

    [Export]
    public bool DrawDebugOverlays { get; set; } = true;

    public float facing { get; private set; } = 0f;
    private Direction direction = Direction.Up;
    private MoveMode moveMode = MoveMode.Walk;

    // references to player nodes
    private AnimatedSprite2D sprite;
    private Area2D interactableCheck;
    private CollisionShape2D interactableSensorShape;
    private AnimatedSprite2D effectsSprite;

    // debug related shit
    static Color sensorDebugColor = new Color(0f, 1f, .1f, .5f);

    public override void _Ready() {
        base._Ready();
        sprite = GetNode<AnimatedSprite2D>("Sprite");
        interactableCheck = GetNode<Area2D>("InteractionCheck");
        interactableSensorShape = interactableCheck.GetNode<CollisionShape2D>("Sensor");
        effectsSprite = GetNode<AnimatedSprite2D>("Effects Overlay Sprite");
    }

    private float getSpeed() {
        switch (moveMode) {
            case MoveMode.Walk: return WalkSpeed;
            case MoveMode.Run: return RunSpeed;
            case MoveMode.Dash: return DashSpeed;
        }
        GD.Print("defaulting to walk speed - moveMode: " + moveMode);
        return WalkSpeed;
    }

    public override void _PhysicsProcess(double delta) {
        if (moveMode != MoveMode.Dash) {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
            Velocity = direction * getSpeed();
        } else {
            Velocity = dashVelocity;
        }

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
        DrawRect(drawnRect, sensorDebugColor, true);
        #endregion
    }

    static Color colorTransparent = new Color(1, 1, 1, 0);

    // used to save dashVelocity at the time of pressing dash
    private Vector2 dashVelocity;
    private void StartDash() {
        // capture current direction and save for duration of dash
        Vector2 impulse = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
        if (impulse == Vector2.Zero) {
            switch (angleToDirection(facing)) {
                case Direction.Left: impulse = Vector2.Left; break;
                case Direction.Right: impulse = Vector2.Right; break;
                case Direction.Up: impulse = Vector2.Up; break;
                case Direction.Down: impulse = Vector2.Down; break;
            }
        }
        dashVelocity = impulse * DashSpeed;

        // detach the effects sprite from the player since we want to leave it behind
        effectsSprite.TopLevel = true;
        effectsSprite.Position = GlobalPosition;

        // set up the effects sprite te play
        effectsSprite.Play(EffectsAnimations.BloodDash.Name());
        effectsSprite.Scale = new Vector2(2, 2);
        effectsSprite.Visible = true;

        // start the tween that kicks off the dash
        var dashTween = GetTree().CreateTween();
        dashTween.TweenProperty(sprite, "modulate", colorTransparent, .15);
        dashTween.TweenCallback(Callable.From(() => moveMode = MoveMode.Dash));
    }

    private void StopDash() {
        var undashTween = GetTree().CreateTween();
        undashTween.TweenProperty(sprite, "modulate", Colors.White, .15);
        undashTween.TweenCallback(Callable.From(() => {
            // unwind effects sprite changes
            effectsSprite.Visible = false;
            effectsSprite.TopLevel = false;
            effectsSprite.Position = Vector2.Zero;
            effectsSprite.Scale = Vector2.One;
            // reset move mode and dash velocity
            moveMode = MoveMode.Walk;
            dashVelocity = Vector2.Zero;
        }));
    }

    // returns if we should skip the rest of processing
    private bool HandleInput() {
        if (Input.IsActionJustPressed(JamEnums.Key.PadY.Name())) {
            StartDash();
            return true;
        }

        moveMode = Input.IsActionPressed(JamEnums.Key.PadX.Name())
            ? MoveMode.Run
            : MoveMode.Walk;

        if (Input.IsActionJustPressed(JamEnums.Key.PadA.Name())) {
            if (focusObject != null) {
                focusObject.Interact(this);
            }
        }

        return false;
    }

    public override void _Process(double delta) {
        // check if we're dashing and bail or stop if relevant
        if (moveMode == MoveMode.Dash) {
            if (effectsSprite.IsPlaying()) {
                return;
            } else {
                StopDash();
            }
        }

        if (HandleInput()) {
            return;
        }

        maybeUpdateSensorPosition();
        if (Velocity != Vector2.Zero) {
            facing = Velocity.AngleTo(Vector2.Up);
            direction = angleToDirection(facing);
        }

        if (Velocity == Vector2.Zero) {
            sprite.Play("idle_" + direction.Name());
        } else {
            sprite.Play(moveMode.Name() + "_" + direction.Name());
        }
    }

    private IInteractable<Player> focusObject;

    private void InteractableEnter(Area2D area) {
        GD.Print($"found interactable object: " + area.GetParent().Name);

        var interactable = IInteractable<Player>.FromArea2D<Player>(area);
        if (interactable.InteractsWith(ActorType.Player)) {
            focusObject = interactable;
        }
    }


    private void InteractableExit(Area2D area) {
        GD.Print($"lost interactable object: " + area.GetParent().Name);
        focusObject = null;
    }
}
