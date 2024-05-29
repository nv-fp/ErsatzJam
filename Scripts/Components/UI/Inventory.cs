using Godot;

using JamEnums;

using System;
using System.Collections.Generic;

public partial class Inventory : Control {
    [Export] public float ScaleFactor = 3;

    private HBoxContainer hbox;

    private Vector2 originalPosition;
    private Vector2 vectorScale;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        hbox = GetNode<HBoxContainer>("HBox");
        originalPosition = hbox.Position;
        vectorScale = new Vector2(ScaleFactor, ScaleFactor);
        hbox.Scale = vectorScale;
    }

    public void UpdateDisplay(Dictionary<JamEnums.Item, int> inventory) {
        var curChildren = hbox.GetChildren();
        foreach (var child in curChildren) {
            child.QueueFree();
        }

        foreach (var item in inventory.Keys) {
            var textureRect = new TextureRect();
            textureRect.Texture = item.GetTexture();
            textureRect.TextureFilter = TextureFilterEnum.Nearest;
            hbox.AddChild(textureRect);
        }

        var pos = hbox.Position;
        pos.X = originalPosition.X - (16 * inventory.Count * ScaleFactor);
        hbox.Position = pos;
    }
}
