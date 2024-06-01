using Godot;

using JamEnums;

using System;

public partial class Guard : Node2D, IBehavior {
    private NPC npc;
    private NavigationAgent2D agent;
    private Polygon2D wanderArea;

    [Export] public GuardWanderMode WanderMode = GuardWanderMode.None;
    [Export] public int LingerTimeSec = 10;
    [Export] public Vector2[] PatrolPath;

    [Export] public int WalkSpeed = 120;

    [Export] public bool DrawDebugOverlay = false;

    double hasLingered = 0;
    public GuardBehavior currentBehavior;
    int nextPatrolPoint = 0;

    public override void _Ready() {
        var node = GetParent();
        if (node is not NPC) {
            throw new NotSupportedException("Guard parent must be NPC");
        }

        npc = (NPC)node;
        agent = GetNode<NavigationAgent2D>("NavAgent");

        if (WanderMode != GuardWanderMode.None) {
            foreach (var child in GetChildren()) {
                GD.Print("Examining Guard children: " + child.Name + " / " + child.GetType());
                if (child.Name == "Patrol Path") {
                    var poly = (Polygon2D)child;
                    if (poly.Position != Vector2.Zero) {
                        GD.Print("your patrol path is not centered on your Guard node. Adjusting for this but it may be a mistake.");
                    }
                    PatrolPath = ((Polygon2D)child).Polygon;
                    child.QueueFree();
                    for (var i = 0; i < PatrolPath.Length; i++) {
                        // convert the path positions into a peer position of the hosting NPC. This assumes that the NavigationRegion2D is also a peer
                        var newP = npc.GlobalPosition + (Position * npc.Scale) + (poly.Position * npc.Scale * Scale) + (PatrolPath[i] * npc.Scale * Scale * poly.Scale);
                        GD.Print($"{PatrolPath[i]} -> {newP}");
                        PatrolPath[i] = newP;
                    }
                    for (var i = 0; i < PatrolPath.Length; i++) {
                        GD.Print($"{i}: {PatrolPath[i]}");
                    }
                    break;
                }
            }
        }

        currentBehavior = WanderMode == GuardWanderMode.None ? GuardBehavior.Halt : GuardBehavior.Travel;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta) {
        switch (currentBehavior) {
            case GuardBehavior.Travel:
                DoWander(delta);
                break;
            case GuardBehavior.Alert: break;
            case GuardBehavior.Pursue: break;
            case GuardBehavior.Halt: break;
        }
        QueueRedraw();
    }

    #region Behavior logic
    private Vector2 GetNextWaypoint() {
        if (WanderMode == GuardWanderMode.Patrol) {
            if (nextPatrolPoint >= PatrolPath.Length) {
                nextPatrolPoint = 0;
            }
            nextPatrolPoint++;
            return PatrolPath[nextPatrolPoint - 1];
        }
        throw new NotSupportedException("Should not get next waypoint if not in a wander mode");
    }
    private void DoWander(double delta) {
        if (agent.IsNavigationFinished()) {
            npc.Velocity = Vector2.Zero;
            if (hasLingered < LingerTimeSec) {
                hasLingered += delta;
                return;
            }
            var nextTarget = GetNextWaypoint();
            GD.Print($"NextTarget ({nextPatrolPoint - 1}): {nextTarget}");
            agent.TargetPosition = nextTarget;
            hasLingered = 0;
        }

        var direction = (agent.GetNextPathPosition() - npc.GlobalPosition).Normalized();

        npc.Velocity = direction * WalkSpeed;
        npc.MoveAndSlide();
    }
    #endregion

    #region detection logic
    public void DetectionAreaEntered(Area2D area) {

    }

    public void DetectionAreaExited(Area2D area) {

    }
    #endregion

    public override void _Draw() {

        if (DrawDebugOverlay) {
            // A - B => ->BA
            foreach (var p in PatrolPath) {
                DrawCircle((p - npc.Position), 10, Colors.Red);
                DrawCircle((p - npc.Position) / npc.Scale, 7, Colors.Green);
            }

            DrawLine(Position, Position + npc.Velocity, Colors.Blue);
        }
    }

    public float facing;
    public Direction direction;
    public override void _Process(double delta) {
        if (npc.Velocity != Vector2.Zero) {
            facing = npc.Velocity.AngleTo(Vector2.Up);
            direction = angleToDirection(facing);
        }

        if (npc.Velocity == Vector2.Zero) {
            npc.Sprite.Play("idle_" + direction.Name());
        } else {
            npc.Sprite.Play("walk_" + direction.Name());
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

}

public enum GuardBehavior {
    Travel,
    Alert,
    Pursue,
    Halt
}