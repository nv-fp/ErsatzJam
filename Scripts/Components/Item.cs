using Godot;

using JamEnums;

public partial class Item : AnimatedSprite2D, IInteractable<Player> {
    [Export]
    public JamEnums.Item Type { get; set; }

    public override void _Ready() {
        Play(Type.Name());
    }

    public void Interact(Player withActor) {
        GD.Print("TODO: Add to player inventory");
        QueueFree();
    }

    public bool InteractsWith(ActorType actorType) {
        return actorType == ActorType.Player;
    }

    public string ActionSummary() {
        return "Pick up";
    }
}