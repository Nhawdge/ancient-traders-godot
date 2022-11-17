using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Settlement : Node, ISelectable {
	public Settlement() {
		Name = "Settlement";
		Population = 30;
		Wealth = 1;
	}

	public int Population { get; set; }
	public double Wealth { get; set; }
	private float GrowingPopulation = 0;
	private float ShrinkingPopulation = 0;

	public double wealthGrowthRate = 0.1f;
	public double babyChance = 0.001f;
	public double deathChance = 0.00001f;

	public Dictionary<Resources, int> Inventory { get; set; } = new();

	public override void _Ready() {
		GetNode<MapUi>("/root/Map").SettlementHireWorker += HireWorkerClicked;
	}

	public override void _Process(double delta) {

		var addedWeath = wealthGrowthRate * delta;
		Wealth += addedWeath;

		var couples = Population / 2;
		var babies = couples * babyChance * delta;
		GrowingPopulation += (float) babies;

		if (Input.IsKeyPressed(Key.Space)) {
			IsSelected = false;
			GetNode<Control>("/root/Map/UI/SelectedView").Hide();
		}
		if (IsSelected)
			EmitSignal(SignalName.InfoUpdated);

	}

	public void on_clicked(Node node, InputEvent evt, int shape) {
		if (evt is InputEventMouseButton mouseButton) {
			if (mouseButton.Pressed) {
				IsSelected = true;
				GetNode<MapUi>("/root/Map/").UpdateSelected(this);
			}
		}
	}

	#region Actions 

	public void HireWorkerClicked() {
		if (!IsSelected) {
			return;
		}
		if (Wealth < 1f) {
			return;
		}
		Wealth -= 1f;
		Population -= 1;

		var worker = new Worker();
		GetNode(".").AddChild(worker);
		GetNode<MapUi>("/root/Map/").UpdateSelected(this);
	}

	#endregion

	#region Helpers

	private IEnumerable<Worker> GetWorkers => GetNode(".").GetChildren().OfType<Worker>();

	#endregion

	#region ISelectable 

	[Signal]
	public delegate void InfoUpdatedEventHandler();

	public bool IsSelected { get; set; }

	public string GetHeader() {
		return $"{Name}";
	}

	public string GetData() {
		return $"Population: {Population}\nWealth: {(int)Wealth}\nWorkers: {GetWorkers.Count()}";
	}
	#endregion

}