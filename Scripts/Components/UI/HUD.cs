using Godot;

public partial class HUD : CanvasLayer {
    public static HUD Instance {  get; private set; }
    
    private DialogueManager dlgMgr;
    private Inventory inventory;
    private StatBar healthBar;
    private StatBar hungerBar;
    private StatBar juiceBar;

    public HUD() {
        HUD.Instance = this;
    }

    public override void _Ready() {
        dlgMgr = GetNode<DialogueManager>("DialogueManager");
        inventory = GetNode<Inventory>("Inventory");
        healthBar = GetNode<StatBar>("Health");
        hungerBar = GetNode<StatBar>("Hunger");
        juiceBar = GetNode<StatBar>("Juice");
    }

    public DialogueManager GetDialogueManager() {
        return dlgMgr;
    }

    public Inventory GetInventory() {
        return inventory;
    }

    public void Setup(Player player) {
        GD.Print($"{player.Health} / {player.Hunger} / {player.Juice}");
        healthBar.SetValue((int)player.Health, (int)player.MaxHealth);
        hungerBar.SetValue((int)player.Hunger, (int)player.MaxHunger);
        juiceBar.SetValue((int)player.Juice, (int)player.MaxJuice);
    }

    public StatBar HealtBar() {
        return healthBar;
    }

    public StatBar HungerBar() {
        return hungerBar;
    }

    public StatBar JuiceBar() {
        return juiceBar;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }
}
