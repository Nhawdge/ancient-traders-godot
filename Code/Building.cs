using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Building : Node {

	public double Progress { get; set; }
	public double ProgressGoal { get; set; }
	public Recipe ActiveRecipe { get; set; }
	public StatusOptions Status { get; set; }

	public List<Recipe> AvailableRecipes { get; set; } = new();
	public int Storage { get; set; }
	public Settlement Settlement { get; set; }

	public override void _Ready() {
		Progress = 0;
		ProgressGoal = 30;
		Status = StatusOptions.Building;
		AvailableRecipes.Add(Recipes.BasicBuildingMaterials);
		AvailableRecipes.Add(Recipes.BasicTools);
		Settlement = GetParent<Settlement>();
	}

	public override void _Process(double delta) {
		if (Status == StatusOptions.Building) {
			var worker = GetNode<Building>(".").GetChildren().OfType<Worker>().FirstOrDefault();
			var buildingSkill = worker.Skills.Find(x => x.Name == Skills.Building);
			if (buildingSkill is not null)
				buildingSkill.Amount += (float) delta;

			if (Progress > ProgressGoal) {
				Status = StatusOptions.Idle;
				Progress = 0;
				worker.IsAvailable = true;
			}
			Progress += delta;
			EmitSignal(SignalName.ProgressChanged);
		}
		if (Status == StatusOptions.Working) {
			Progress += delta;
			if (Progress > ActiveRecipe.CraftTime) {
				if (!Settlement.Inventory.TryAdd(ActiveRecipe.Result, 1)) {
					Settlement.Inventory[ActiveRecipe.Result] += 1;
				}
				Progress = 0;
				Status = StatusOptions.Idle;
				ActiveRecipe = null;
			}
			EmitSignal(SignalName.ProgressChanged);
		}
	}

	public void Craft(Recipe recipe) {
		Status = StatusOptions.Working;
		Progress = 0;
		ActiveRecipe = recipe;
		var parent = GetParent() as Settlement;
		foreach (var ingredient in recipe.Resources) {
			parent.Inventory[ingredient.ResourceName] -= ingredient.Amount;
		}
	}

	protected override void Dispose(bool disposing) {
		//ProgressChanged -= () => progress.Value = building.Progress;;
		base.Dispose(disposing);
	}

	public bool HasCorrectInventory(Recipe recipe) {
		foreach (var item in recipe.Resources) {
			if (Settlement.Inventory.TryGetValue(item.ResourceName, out var amount)) {
				if (amount < item.Amount)
					return false;
			} else {
				return false;
			}
		}
		return true;
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