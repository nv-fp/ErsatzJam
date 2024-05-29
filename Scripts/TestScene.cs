using Godot;

public partial class TestScene : Node {
    private Player playerNode;
    private Camera2D camera;
    private NPC npc;
    public HUD Hud {  get; private set; }

    public override void _Ready() {
        playerNode = GetNode<Player>("Player");
        camera = GetNode<Camera2D>("Player/Camera2D");
        Hud = GetNode<HUD>("Hud");
        Hud.Setup(playerNode);
        npc = GetNode<NPC>("Npc");
    }
}