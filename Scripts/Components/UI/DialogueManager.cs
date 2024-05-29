using System;
using System.Linq;

using Godot;

using JamEnums;

public partial class DialogueManager : Control {
    private Button continueBtn;
    private Dialogue dlg;

    [Signal] public delegate void DialogueStartedEventHandler();
    [Signal] public delegate void DialogueStoppedEventHandler();

    private string[] currentScript;
    private int currentLine;
    private bool waitForContinue = false;
    private Action completedCallback;

    public override void _Ready() {
        continueBtn = GetNode<Button>("ContinueButton");
        dlg = GetNode<Dialogue>("Dialogue");
    }

    public void DisplayScript(string script, Texture2D tex = null, Action callback = null) {
        dlg.SetProfile(tex);

        currentScript = script.Split("\n").Where((string e) => e.Trim().Length > 0).ToArray();
        completedCallback = callback;
        if (currentScript.Length > 0) {
            EmitSignal(SignalName.DialogueStarted);
            currentLine = 0;
            Visible = true;
            dlg.Visible = true;
            DisplayLine();
        }
    }

    public void DisplayLine() {
        var line = currentScript[currentLine];
        if (line.Trim().Length == 0) {

        }
        var parts = line.Split(':');

        if (parts.Length == 2) {
            var speaker = parts[0];
            if (speaker.StartsWith("<")) {
                var endPortrait = speaker.IndexOf(">");
                var portrait = speaker.Substr(1, endPortrait - 1);
                dlg.SetProfile(TextureFactory.Instance.Get($"res://Art/{portrait}.png"));
                speaker = speaker.Substring(endPortrait + 1);
            }
            dlg.SetSpeaker(speaker);
            dlg.SetText(parts[1]);
        } else {
            dlg.NoSpeaker();
            dlg.SetText(parts[1]);
        }
        waitForContinue = true;
        continueBtn.Visible = true;
    }

    public void ContinuePressed() {
        currentLine++;

        if (currentLine >= currentScript.Length) {
            waitForContinue = false;
            continueBtn.Visible = false;
            dlg.Visible = false;
            completedCallback?.Invoke();
            EmitSignal(SignalName.DialogueStopped);
        } else {
            DisplayLine();
        }
    }

    public bool WaitingForInput() {
        return waitForContinue;
    }

    public override void _Process(double delta) {
        if (waitForContinue) {
            if (Input.IsActionJustPressed(JamEnums.Key.PadA.Name())) {
                ContinuePressed();
            }
        }
    }
}
