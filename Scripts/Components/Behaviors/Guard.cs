using Godot;

using JamEnums;

using System;

public partial class Guard : Node2D {
    private NPC npc;
    private NavigationAgent2D agent;
    private Polygon2D wanderArea;
    [Export] public GuardWanderMode WanderMode = GuardWanderMode.None;
    [Export] public int LingerTimeSec = 10;
    [Export] public Vector2[] PatrolPath;

    [Export] public int WalkSpeed = 120;

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
                    GD.Print("locations");
                    GD.Print("npc.Scale: " + npc.Scale);
                    GD.Print($"npc.GlobalPosition: {npc.GlobalPosition}");
                    GD.Print($"Position: {Position}");
                    GD.Print($"poly.Position: {poly.Position}");
                    PatrolPath = ((Polygon2D)child).Polygon;
                    child.QueueFree();
                    for (var i = 0; i < PatrolPath.Length; i++) {
                        var newP_1 = PatrolPath[i] + npc.GlobalPosition;
                        var newP_2 = Vector2.Zero - (PatrolPath[i] - npc.GlobalPosition);
                        var newP_3 = npc.GlobalPosition + Position + poly.Position + PatrolPath[i];
                        var newP_4 = (npc.GlobalPosition * npc.Scale) + (Position * Scale) + (poly.Position * poly.Scale) + PatrolPath[i];
                        var newP = Vector2.Zero - ((PatrolPath[i] - npc.GlobalPosition) / npc.Scale);
                        GD.Print($"{PatrolPath[i]} -> {newP_3}"); //  {newP_1}, {newP_2}, {newP_3}, {newP}");
                        PatrolPath[i] = newP_4;
                    }
                    for (var i = 0; i < PatrolPath.Length; i++) {
                        GD.Print($"{i}: {PatrolPath[i]}");
                    }
                    break;
                }
            }
        }
        // WanderMode = GuardWanderMode.None;

        currentBehavior = WanderMode == GuardWanderMode.None ? GuardBehavior.Halt : GuardBehavior.Travel;
        GD.Print("currentBehavior: " + currentBehavior);
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
        base._Draw();
        var adjustedPosition = npc.GlobalPosition * npc.Scale;
        foreach (var p in PatrolPath) {
            // DrawCircle(p - npc.Position, 15, Colors.Blue);
            DrawCircle((p - adjustedPosition), 10, Colors.Red);
            DrawCircle(Vector2.Zero - adjustedPosition + p, 7, Colors.Green);
            // DrawCircle(p, 5, Colors.Purple); -- wrong
        }


        /*
        DrawCircle(npc.GlobalPosition, 5, Colors.SeaGreen);
        DrawCircle(GlobalPosition, 8, Colors.CornflowerBlue);
        DrawCircle(npc.Position, 10, Colors.AliceBlue);

        DrawCircle(Vector2.Zero, 5, Colors.Green);
        DrawCircle(new Vector2(100, 100), 5, Colors.Orange);
        DrawCircle(new Vector2(250, 250), 5, Colors.RoyalBlue);
        */
        DrawLine(npc.GlobalPosition, (npc.GlobalPosition + npc.Velocity), Colors.Blue);
    }

    // TODO: does this even work with c#?
    public override string[] _GetConfigurationWarnings() {
        var parent = GetParent();
        if (parent == null || parent is not NPC) {
            return new string[] { "Must be attached to an NPC node" };
        }
        return new string[] { };
    }
}

public enum GuardBehavior {
    Travel,
    Alert,
    Pursue,
    Halt
}