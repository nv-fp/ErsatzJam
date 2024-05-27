using Godot;

public partial class HUD : CanvasLayer {
    public static HUD Instance {  get; private set; }
    
    private DialogueManager dlgMgr;

    public HUD() {
        HUD.Instance = this;
    }

    public DialogueManager GetDialogueManager() {
        return dlgMgr;
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        dlgMgr = GetNode<DialogueManager>("DialogueManager");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
}
