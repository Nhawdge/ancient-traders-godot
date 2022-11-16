using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ResourceCamp : Node2D, ISelectable {
	public bool IsSelected { get; set; }
	public List<GameResource> AvailableResources { get; set; } = new();

	public override void _Ready() {
		var home = GetNode(".");
		var rand = new Random();

		var resourcesToGen = rand.Next(1, 5);
		while (resourcesToGen-- > 0) {
			var resource = new GameResource();
			resource.ResourceName = Resources.Lumber;
			resource.Amount = rand.Next(1, 100);
			AvailableResources.Add(resource);
		}

		GetNode<MapUi>("/root/Map").CampHarvest += HarvestResource;
	}

	public override void _Process(double delta) {
		if (Input.IsKeyPressed(Key.Space)) {
			IsSelected = false;
		}
	}

	public void InputEvent(Node viewport, InputEvent evt, int shape) {
		if (evt is InputEventMouseButton button) {
			if (button.Pressed == true) {
				IsSelected = true;
				GetNode<MapUi>("/root/Map/").UpdateSelected(this);
			}
		}
	}

	public void HarvestResource() {
		if (!IsSelected) {
			return;
		}
	}

	public string GetHeader() {
		return Name;
	}

	public string GetData() {
		var labelText = string.Empty;
		var nodes = GetNode(".").GetChildren().OfType<GameResource>();
		foreach (var resource in nodes) {
			labelText += $"{resource.ResourceName}: {resource.Amount}\n";
		}
		return labelText;
	}

}