using System;
using Godot;

public partial class WorkerSkill : Node {
	[Export]
	public Resources SkillName { get; set; }

	[Export]
	public int Amount { get; set; }

	public GameResource(, int amount = 0) {
		SkillName = name;
		Amount = amount;
	}

}