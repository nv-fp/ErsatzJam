using JamEnums;

using Godot;

using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D {
    #region player configs
    [Export] public float WalkSpeed = 120f;
    [Export] public float RunSpeed = 200;
    [Export] public float DashSpeed = 600;
    [Export] public uint MaxHealth = 100;
    [Export] public uint MaxHunger = 100;
    [Export] public uint MaxJuice = 50;

    [Export] public uint DashPoints = 2;
    [Export] public uint StrengthPoints = 5;
    [Export] public uint PersuasionPoints = 20;
    [Export] public uint HealPoints = 4;

    [Export] public uint HungerGainRateSec = 1;
    [Export] public uint RegenRateMin = 10;

    [Export] public bool DrawDebugOverlays { get; set; } = true;
    #endregion

    #region player stats
    // stats
    private double _health;
    public double Health {
        get => _health;
        private set {
            _health = value;
            HUD.Instance?.HealtBar()?.SetValue((int)value, (int)MaxHealth);
        }
    }

    private double _hunger;
    public double Hunger {
        get => _hunger;
        private set {
            _hunger = value;
            HUD.Instance?.HungerBar()?.SetValue((int)value, (int)MaxHunger);
        }
    }

    private uint _juice;
    public uint Juice {
        get => _juice;
        private set {
            _juice = value;
            HUD.Instance?.JuiceBar()?.SetValue((int)value, (int)MaxJuice);
        }
    }
    public uint Vampirism { get; private set; }
    #endregion

    #region player state tracking -- "is walking", "is attacking" etc
    // angle, in radians from "up"
    public float facing { get; private set; } = 0f;
    private bool isAttacking = false;
    private bool vampiricAttack = false;

    private IInteractable<Player> focusObject;
    private bool focusAttackable = false;

    private Direction direction = Direction.Up;
    // how are we moving
    private MoveMode moveMode = MoveMode.Walk;
    // used to save dashVelocity at the time of pressing dash
    private Vector2 dashVelocity;

    public Dictionary<JamEnums.Item, int> Inventory { get; private set; }

    private bool EffectPlaying() {
        return effectsSprite.IsPlaying();
    }
    #endregion

    #region node references
    // references to player nodes
    private AnimatedSprite2D sprite;
    private Area2D interactableCheck;
    private AnimatedSprite2D effectsSprite;
    private CollisionShape2D interactableCollider;
    private CollisionShape2D physicsCollider;
    private CollisionShape2D hitboxCollider;
    #endregion

    #region consts and shit
    static Vector2 vectorTwo = new Vector2(2, 2);
    static Vector2[] sensorPosition = new Vector2[4] {
        new Vector2(0, -45), // up
        new Vector2(30,  5), // right
        new Vector2(0,  40), // down
        new Vector2(-30, 5), // left
    };
    static Vector2 sensorScaledUp = new Vector2(1, 3);
    static Vector2 sensorNormal = new Vector2(1, 1);

    #endregion

    #region debug stuff
    // debug related shit
    static Color colorTransparent = new Color(1, 1, 1, 0);
    [Export] public Color defaultSensorColor = new Color(0f, 1f, .1f, .5f);
    [Export] public Color physicsSensorColor = new Color(0f, .1f, 1f, .5f);
    [Export] public Color hitboxSensorColor = new Color(1f, 0, 0, .5f);
    #endregion

    public Player() {
        Inventory = new Dictionary<JamEnums.Item, int>();
    }

    public override void _Ready() {
        base._Ready();
        sprite = GetNode<AnimatedSprite2D>("Sprite");
        effectsSprite = GetNode<AnimatedSprite2D>("EffectsSprite");

        interactableCheck = GetNode<Area2D>("InteractionCheck");
        interactableCollider = interactableCheck.GetNode<CollisionShape2D>("Sensor");
        physicsCollider = GetNode<CollisionShape2D>("Collider");
        hitboxCollider = GetNode<CollisionShape2D>("Hitbox/Collider");

        GD.Print("Player._Ready setting initial stats");
        Health = MaxHealth;
        Hunger = 0;
        Juice = MaxJuice;
        GD.Print($"Initial Stats: {Health}, {Hunger}, {Juice}");
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
        if (HUD.Instance.GetDialogueManager().WaitingForInput()) {
            return;
        }
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

    private void maybeUpdateSensorPosition() {
        switch (direction) {
            case Direction.Up:
                interactableCollider.Position = sensorPosition[0];
                interactableCollider.Scale = sensorNormal;
                break;
            case Direction.Right:
                interactableCollider.Position = sensorPosition[1];
                interactableCollider.Scale = sensorScaledUp;
                break;
            case Direction.Down:
                interactableCollider.Position = sensorPosition[2];
                interactableCollider.Scale = sensorNormal;
                break;
            case Direction.Left:
                interactableCollider.Position = sensorPosition[3];
                interactableCollider.Scale = sensorScaledUp;
                break;
        }
    }

    public override void _Draw() {
        base._Draw();
        if (!DrawDebugOverlays) {
            return;
        }

        DrawCollisionShape(interactableCollider, defaultSensorColor);
        DrawCollisionShape(hitboxCollider, hitboxSensorColor);
        DrawCollisionShape(physicsCollider, physicsSensorColor);
    }

    private void DrawCollisionShape(CollisionShape2D shape, Color color) {
        var shapeRect = shape.Shape.GetRect();
        var shapeSize = shapeRect.Size * shape.Scale;
        var sensorXy = shape.Position;
        sensorXy -= shapeSize / 2;
        var drawnRect = new Rect2(sensorXy, shapeSize);
        DrawRect(drawnRect, color, true);
    }

    // return true if power was able to deduct enough points
    public bool UsePower(uint points) {
        if (Juice < points) {
            return false;
        }
        Juice -= points;
        return true;
    }

    private void StartDash() {
        if (!UsePower(DashPoints)) {
            return;
        }

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
        effectsSprite.Scale = vectorTwo;
        effectsSprite.Visible = true;

        // start the tween that kicks off the dash
        var dashTween = GetTree().CreateTween();
        dashTween.TweenProperty(sprite, "modulate", colorTransparent, .15);
        dashTween.TweenCallback(Callable.From(() => moveMode = MoveMode.Dash));
    }

    private void StopDash() {
        var undashTween = GetTree().CreateTween();
        undashTween.TweenProperty(sprite, "modulate", Colors.White, .05);
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

    private void StartAttack(bool vampiricPowered = false) {
        isAttacking = true;
        vampiricAttack = vampiricPowered;
        effectsSprite.Scale = vectorTwo;
        effectsSprite.Visible = true;
        effectsSprite.Position = interactableCollider.Position;
        effectsSprite.FlipH = direction == Direction.Left;
        if (direction == Direction.Up) {
            effectsSprite.RotationDegrees = -90;
        } else if (direction == Direction.Down) {
            effectsSprite.RotationDegrees = 90;
        } else {
            effectsSprite.RotationDegrees = 0;
        }
        effectsSprite.Play(EffectsAnimations.Attack.Name());
    }

    private void StopAttack() {
        isAttacking = false;
        effectsSprite.Scale = Vector2.One;
        effectsSprite.Visible = false;
    }

    // returns if we should skip the rest of processing
    private bool HandleInput() {
        if (HUD.Instance.GetDialogueManager().WaitingForInput()) {
            return true;
        }

        if (!EffectPlaying() && moveMode != MoveMode.Dash && Input.IsActionJustPressed(JamEnums.Key.PadY.Name())) {
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

        if (!EffectPlaying() && Input.IsActionJustPressed(JamEnums.Key.PadB.Name())) {
            if (Input.IsActionPressed(JamEnums.Key.PadLT.Name()) && UsePower(StrengthPoints)) {
                StartAttack(true);
            } else {
                StartAttack();
            }
            return true;
        }

        return false;
    }

    private void RegenAndHunger(double delta) {
        if (HUD.Instance.GetDialogueManager().WaitingForInput()) {
            return;
        }
        Hunger += HungerGainRateSec * delta;
        Health += RegenRateMin * (delta / 60);
    }

    public override void _Process(double delta) {
        RegenAndHunger(delta);
        if (isAttacking && !EffectPlaying()) {
            StopAttack();
        }
        if (isAttacking) {
            effectsSprite.Position = interactableCollider.Position;
        }

        // check if we're dashing and bail or stop if completed
        if (moveMode == MoveMode.Dash) {
            if (EffectPlaying()) {
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

    #region signal callbacks
    private void InteractableEnter(Area2D area) {
        GD.Print($"found interactable object: " + area.GetParent().Name);

        var interactable = IInteractable<Player>.FromArea2D<Player>(area);
        if (interactable.InteractsWith(ActorType.Player)) {
            focusObject = interactable;
            focusAttackable = area.GetCollisionLayerValue(3);
        }
    }

    private void InteractableExit(Area2D area) {
        GD.Print($"lost interactable object: " + area.GetParent().Name);
        focusObject = null;
    }

    private void DoHitResolution() {
        if (focusObject != null && focusAttackable) {
            GD.Print($"Hit {((Node2D)focusObject).Name}");
            var attackable = (IAttackable)focusObject;
            attackable.Attacked(vampiricAttack);
        } else {
            GD.Print("miss!");
        }
    }
    #endregion
}
