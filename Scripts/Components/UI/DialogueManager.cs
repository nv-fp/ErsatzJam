using System.Collections.Generic;
using System.Linq;

using Godot;

using JamEnums;

public partial class DialogueManager : Control {
    private Button continueBtn;
    private Dialogue dlg;

    private string[] currentScript;
    private int currentLine;
    private bool waitForContinue = false;

    public override void _Ready() {
        continueBtn = GetNode<Button>("ContinueButton");
        dlg = GetNode<Dialogue>("Dialogue");
    }

    public void DisplayScript(string script, Texture2D tex = null) {
        dlg.SetProfile(tex);

        currentScript = script.Split("\n").Where((string e) => e.Trim().Length > 0).ToArray();
        if (currentScript.Length > 0) {
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
            dlg.SetSpeaker(parts[0]);
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
