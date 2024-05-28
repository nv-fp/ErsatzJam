using Godot;

using JamEnums;

public partial class NPC : CharacterBody2D,
    IInteractable<Player>,
    IAttackable {

    public TestScene level { get; set; }
    [Export] public string name { get; private set; }

    [Export] public int MaxHitPoints { get; set; } = 5;

    [Export] public bool FreeOnDeath = true;

    public int HitPoints { get; set; }

    public NPC() {
        HitPoints = MaxHitPoints;
    }

    public void Interact(Player withActor) {
        GD.Print($"current level is: {level}");
        if (level != null) {
            level.Hud.GetDialogueManager().DisplayScript(
                "<kolvir-portrait>Frank: Hi Bob\n" +
                "<cleric-portrait>Bob: Hey Frank\n" +
                "<kolvir-portrait>Frank: Busy week eh?\n" +
                "<cleric-portrait>Bob: These heretics aren't going to burn themselves now are they?\n"
            );
        }
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
        throw new System.NotImplementedException();
    }
}
