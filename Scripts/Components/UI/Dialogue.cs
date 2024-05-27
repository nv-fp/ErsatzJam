using Godot;

using System;

public partial class Dialogue : Control {
    private RichTextLabel dlgLabel;
    private Label dlgSpeaker;
    private TextureRect profileRect;

    [Export]
    public Texture2D SpeakerProfileTex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        profileRect = GetNode<TextureRect>("NinePatchRect/HBox/Profile");
        dlgSpeaker = GetNode<Label>("NinePatchRect/HBox/VBox/Label");
        dlgLabel = GetNode<RichTextLabel>("NinePatchRect/HBox/VBox/Text");

        if (SpeakerProfileTex == null ) {
            profileRect.Visible = false;
        } else {
            profileRect.Texture = SpeakerProfileTex;
        }
    }


    public void SetText(string txt) {
        dlgLabel.Text = txt;
    }

    public void SetProfile(Texture2D tex) {
        profileRect.Visible = tex != null;
        profileRect.Texture = tex;
    }

    public void SetSpeaker(string name) {
        dlgSpeaker.Text = name;
        dlgSpeaker.Visible = true;
    }
    public void NoSpeaker() {
        dlgSpeaker.Visible = false;
    }
}
