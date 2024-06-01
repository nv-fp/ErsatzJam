using Godot;

using JamEnums;

public partial class NPC : CharacterBody2D,
    IInteractable<Player>,
    IAttackable {
    [Export] public BehaviorModel BehaviorModel { get; set; }

    [Export] public string name { get; private set; }

    [Export] public int MaxHitPoints { get; set; } = 5;

    [Export] public bool FreeOnDeath = true;

    public int HitPoints { get; set; }

    public AnimatedSprite2D Sprite { get; private set; }

    public override void _Ready() {
        HitPoints = MaxHitPoints;
        foreach (var child in GetChildren()) {
            if (child is IBehavior) {
                GD.Print("Checking " + child.Name);
                Sprite = child.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
                if (Sprite != null) {
                    break;
                }
            }
        }
        if (Sprite == null) {
            Sprite = GetNode<AnimatedSprite2D>("FallbackAnimatedSprite2D");
        } else {
            RemoveChild(GetNode<AnimatedSprite2D>("FallbackAnimatedSprite2D"));
        }
    }

    public void Interact(Player withActor) {
        HUD.Instance.GetDialogueManager().DisplayScript(
            "<kolvir-portrait>Frank: Hi Bob\n" +
            "<cleric-portrait>Bob: Hey Frank\n" +
            "<kolvir-portrait>Frank: Busy week eh?\n" +
            "<cleric-portrait>Bob: These heretics aren't going to burn themselves now are they?\n"
        );
    }

    public override void _Process(double delta) {
        if (HitPoints <= 0 && FreeOnDeath) {
            QueueFree();
        }
    }

    public bool InteractsWith(ActorType actorType) {
        return actorType == ActorType.Player;
    }

    public string ActionSummary() {
        return "Talk";
    }

    public void Attacked(bool vampiricStrength) {
        if (vampiricStrength) {
            HitPoints -= 5;
        } else {
            HitPoints -= 1;
        }
    }
}
;