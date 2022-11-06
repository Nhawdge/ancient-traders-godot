using System;
using Godot;

public partial class GameTime : Node2D {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() { }

	public double Time { get; set; }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Time += delta;

		var days = (int) (Time / 100);

		GetNode<Label>("UI/Label").Set("text", $"{days} day{(days == 1 ? "" : "s")}");
		GetNode<ProgressBar>("UI/ProgressBar").Set("value", Time % 100);

	}
}