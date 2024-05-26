using Godot;

using JamEnums;

using System;

public partial class NPC : CharacterBody2D, IInteractable<Player> {
    [Export] public string name { get; private set; }
    public void Interact(Player withActor) {
        GD.Print("Talk to player");
    }

    public bool InteractsWith(ActorType actorType) {
        return actorType == ActorType.Player;
    }

    public string ActionSummary() {
        return "Talk";
    }
}
