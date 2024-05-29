using Godot;

using JamEnums;

public partial class Item : Sprite2D, IInteractable<Player> {
    [Export] public bool FreeOnPickup = true;
    [Export] public JamEnums.Item Type { get; set; }

    public override void _Ready() {
        Texture = Type.GetTexture();
    }

    public void Interact(Player withActor) {
        int curCount;
        withActor.Inventory.TryGetValue(Type, out curCount);
        withActor.Inventory.Add(Type, curCount + 1);
        if (FreeOnPickup) {
            QueueFree();
        }
        HUD.Instance.GetInventory().UpdateDisplay(withActor.Inventory);
    }

    public bool InteractsWith(ActorType actorType) {
        return actorType == ActorType.Player;
    }

    public string ActionSummary() {
        return "Pick up";
    }
}