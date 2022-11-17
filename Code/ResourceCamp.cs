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

		var resource = new GameResource();
		resource.ResourceName = Resources.Lumber;
		resource.Amount = rand.Next(30, 100);
		AvailableResources.Add(resource);
		Name = resource.ResourceName.ToString() + " camp";

		GetNode<MapUi>("/root/Map").CampHarvest += HarvestResource;
	}

	public override void _Process(double delta) {
		if (Input.IsKeyPressed(Key.Space)) {
			IsSelected = false;
		}
		if (IsSelected)
			EmitSignal(SignalName.InfoUpdated);
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
		var resource = AvailableResources.First();
		var worker = GetChildren().OfType<Worker>().First();
		GD.Print($"{worker.Name} is harvesting {resource.ResourceName} from  {Name}");
		if (resource.Amount > 0) {
			resource.Amount--;
			if (!worker.Inventory.TryAdd(resource.ResourceName, 1)) {
				worker.Inventory[resource.ResourceName]++;
			}
		}
		EmitSignal(SignalName.InfoUpdated);
	}

	public string GetHeader() {
		return Name;
	}

	public string GetData() {
		var labelText = string.Empty;
		foreach (var resource in AvailableResources) {
			labelText += $"{resource.ResourceName}: {resource.Amount}\n";
		}
		return labelText;
	}

	[Signal]
	public delegate void InfoUpdatedEventHandler();

}