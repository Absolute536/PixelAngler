using GamePlayer;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class ActionManager : Node
{
    public Player Target;

    private IDictionary<string, ItemAction> ItemActions = new Dictionary<string, ItemAction>();

    public override void _Ready()
    {
        if (IsInsideTree())
            Target = GetTree().GetFirstNodeInGroup("Player") as Player; // Should be able to cast right?

        ItemActions.Add("Fishing Rod", new FishingRodAction(Target));
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

    public void StartItemAction(string itemAction, Node2D target)
    {
        
    }

}