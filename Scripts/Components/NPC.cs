using Godot;

using JamEnums;

public partial class NPC : CharacterBody2D, IInteractable<Player> {
    public TestScene level { get; set; }
    [Export] public string name { get; private set; }

    public void Interact(Player withActor) {
        GD.Print($"current level is: {level}");
        if (level != null) {
            level.Hud.GetDialogueManager().DisplayScript(
                "Frank: Hi Bob\n" +
                "Bob: Hey Frank\n" +
                "Frank: Busy week eh?\n" +
                "Bob: These heretics aren't going to burn themselves now are they?\n",
                ResourceLoader.Load<Texture2D>("res://Art/dude-portrait.png")
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
