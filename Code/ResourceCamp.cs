using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ResourceCamp : Node2D {
	// Called when the node enters the scene tree for the first time.

	public bool IsSelected { get; set; }

	public override void _Ready() {
		var home = GetNode(".");
		var rand = new Random();

		var resourcesToGen = rand.Next(1, 5);
		while (resourcesToGen-- > 0) {
			var resource = new GameResource();
			resource.ResourceName = Resources.Lumber;
			resource.Amount = rand.Next(1, 100);
			home.AddChild(resource);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (IsSelected) {
			var labelText = string.Empty;
			var nodes = GetNode(".").GetChildren().OfType<GameResource>();
			foreach (var resource in nodes) {
				labelText += $"{resource.ResourceName} - {resource.Amount}\n";
			}
			var label = GetNode("Label");
			label.Set("text", labelText);
		} else {
			GetNode<Label>("Label").Set("text", string.Empty);
		}

		if (Input.IsKeyPressed(Key.Space)) {
			IsSelected = false;
		}
	}

	public void InputEvent(Node viewport, InputEvent evt, int shape) {
		if (evt is InputEventMouseButton button) {
			if (button.Pressed == true) {
				IsSelected = true;
			}
		}
	}
}