using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Building : Node {

	public double Progress { get; set; }
	public StatusOptions Status { get; set; }

	public List<Recipe> Recipes { get; set; } = new();

	public int Storage { get; set; }

	public override void _Ready() {
		Progress = 0;
		Status = StatusOptions.Building;
		Recipes.Add(new Recipe("Building Materials", 100, new List<GameResource> { new(Resources.Lumber, 10) }, ManufacturedResources.BasicBuildingMaterials));
	}

	public override void _Process(double delta) {
		if (Status == StatusOptions.Building) {
			var worker = GetNode<Building>(".").GetChildren().OfType<Worker>().FirstOrDefault();
			var buildingSkill = worker.Skills.Find(x => x.Name == Skills.Building);
			if (buildingSkill is not null)
				buildingSkill.Amount += (float) delta;

			if (Progress > 100) {
				Status = StatusOptions.Idle;
				Progress = 0;
			}
			Progress += delta;
			EmitSignal(SignalName.ProgressChanged);
		}
		if (Status == StatusOptions.Working) {

		}
	}

	[Signal]
	public delegate void ProgressChangedEventHandler();

	public enum StatusOptions {
		Building,
		Idle,
		Working,
		Complete
	}
}