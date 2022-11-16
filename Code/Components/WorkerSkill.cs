using System;
using Godot;

public class WorkerSkill {
    public Skills Name { get; set; }
    public float Amount { get; set; }

    public WorkerSkill(Skills skill = Skills.Traveling, float amount = 0) {
        Name = skill;
        Amount = amount;
    }

}