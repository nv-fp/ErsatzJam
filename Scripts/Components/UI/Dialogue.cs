using Godot;

using System;

public partial class Dialogue : Control {
    private RichTextLabel dlgLabel;
    private TextureRect profileRect;

    [Export]
    public Texture2D SpeakerProfileTex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        dlgLabel = GetNode<RichTextLabel>("NinePatchRect/HBox/VBox/Text");
        profileRect = GetNode<TextureRect>("NinePathRect/HBox/Profile");

        if (SpeakerProfileTex == null ) {
            profileRect.Visible = false;
        } else {
            profileRect.Texture = SpeakerProfileTex;
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void SetText(string txt) {
        dlgLabel.Text = txt;
    }
}
