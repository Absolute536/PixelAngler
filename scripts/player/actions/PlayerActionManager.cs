using GamePlayer;
using Godot;
using SignalBusNS;
using System;
using System.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerActionManager : Node
{
    [Export] public Player ActionBody; // Obtains the Player node reference from exported property

    // Put a series of delegates for each of the selected items into the dictionary and query them for invocation in a method

    public delegate void ActionEventHandler();

    public ActionEventHandler CastActionStart;
    public ActionEventHandler CastActionEnd;
    public ActionEventHandler NetActionStart;
    public ActionEventHandler NetActionEnd;

    // Ok, don't need library, we use pattern matching to check the selected item and invoke the respective delegate

    public override void _Ready()
    {

    }

    public void StartAction(string selectedItem)
    {
        switch (selectedItem)
        {
            case "Fishing Rod":
                SignalBus.Instance.OnCastActionStart(this, EventArgs.Empty);
                break;
            case "Bug Net":
                SignalBus.Instance.OnCastActionEnd(this, EventArgs.Empty);
                break;
            default:
                throw new ArgumentException("No matching item type found");
        }
    }

    public void EndAction(string selectedItem)
    {
        switch (selectedItem)
        {
            case "Fishing Rod":
                SignalBus.Instance.OnCastActionEnd(this, EventArgs.Empty);
                break;
            case "Bug Net":
                SignalBus.Instance.OnNetActionEnd(this, EventArgs.Empty);
                break;
            default:
                throw new ArgumentException("No matching item type found");
        }
    }
    // ActionManager
    // The player ActionState should have a reference to this
    // This one will be placed within the Player scene as well
    // based on the selected item of the player, we invoke the respective functions to execute the action
    // so selected item as key, get the delegate(?) then invoke the method

    // YOU KNOW WHAT?! SCREW IT!!!
    // WE'LL PUT EVERYTHING IN HERE FIRST
    // THEN WE'LL SEE
    // OR MAYBE WE DON'T EVEN NEED a dictionary
    // just define the functions here and the ActionState can call it.

    // OK Should ItemAction do?
    // What should it do?

    // Fishing Rod
    // switch to fishing animation (probably should be in the ActionState)
    // create a Line2D, add it to player
    // create a cast marker, add it to player
    // copied from 
}