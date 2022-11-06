using System;
using Godot;

public partial class GameResource : Node {
	[Export]
	public Resources ResourceName { get; set; }

	[Export]
	public int Amount { get; set; }

	public GameResource(Resources name = Resources.Lumber, int amount = 0) {
		ResourceName = name;
		Amount = amount;
	}

	public GameResource() {
		ResourceName = Resources.Lumber;
		Amount = 0;
	}

}