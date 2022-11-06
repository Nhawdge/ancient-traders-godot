using System;
using Godot;

public partial class Settlement : Node {
	public Settlement() {
		Name = "Settlement";
		Population = 30;
		Wealth = 0;
	}
	public int Population { get; set; }
	public double Wealth { get; set; }
	private float GrowingPopulation = 0;
	private float ShrinkingPopulation = 0;

	public double wealthGrowthRate = 0.001f;
	public double babyChance = 0.001f;
	public double deathChance = 0.00001f;

	[Export]
	public bool IsSelected { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
		var addedWeath = wealthGrowthRate * delta;
		Wealth += addedWeath;

		var couples = Population / 2;
		var babies = couples * babyChance * delta;
		if (babies > 0) {
			GrowingPopulation += (float) babies;
		}

		if (IsSelected) {
			GetNode<Label>("Label").Set("text", $"{Name} - {Population} - {Wealth.ToString("C")}");
		} else {
			GetNode<Label>("Label").Set("text", string.Empty);
		}
		if (Input.IsKeyPressed(Key.Space)) {
			IsSelected = false;
		}
		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
			//IsSelected = false;
		}
	}

	public void on_clicked(Node node, InputEvent evt, int shape) {
		if (evt is InputEventMouseButton mouseButton) {
			if (mouseButton.Pressed == true) {
				IsSelected = true;
			}
		}
	}
}