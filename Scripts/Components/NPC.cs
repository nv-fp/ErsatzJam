using Godot;

using JamEnums;

public partial class NPC : CharacterBody2D, IInteractable<Player> {
    public TestScene level { get; set; }
    [Export] public string name { get; private set; }

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

    public bool InteractsWith(ActorType actorType) {
        return actorType == ActorType.Player;
    }

    public string ActionSummary() {
        return "Talk";
    }
}
