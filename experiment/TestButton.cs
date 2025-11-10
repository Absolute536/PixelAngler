using Godot;
using System;

public partial class TestButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		Pressed += ButtonPress;
    }

    // public override void _GuiInput(InputEvent @event)
    // {
    //     if (@event.IsAction("Action"))
	// 	{
	// 		GD.Print("LMB detected in _UnhandledInput() of Button");
	// 		GetViewport().SetInputAsHandled();
	// 	}
    // }


	public void ButtonPress()
    {
		GD.Print("LMB detected on button pressed");
		AcceptEvent();
    }

}
