using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Worker : Node, ISelectable {
	// Called when the node enters the scene tree for the first time.

	public bool IsAvailable { get; set; } = true;

	public List<WorkerSkill> Skills { get; set; } = new List<WorkerSkill>();
	public override void _Ready() {
		var me = GetNode(".");

		foreach (var skill in Enum.GetValues<Skills>()) {
			var workerSkill = new WorkerSkill(skill, 0);
			Skills.Add(workerSkill);
		}
		me.Name = Names.ElementAt(new Random().Next(0, Names.Count));
	}

	public string GetHeader() {
		return Name;
	}

	public string GetData() {
		return "Worker"; // string.Join("\n", Skills.Select(s => $"{s.Name}: {s.Amount}"));
	}

	public void Select() {
		IsSelected = true;
		GetNode<MapUi>("/root/Map/").UpdateSelected(this);
	}

	private List<string> Names => new List<string> {
		"Aarav",
		"Alexander",
		"Amadeus",
		"Amias",
		"Andreas",
		"Arit",
		"Arram",
		"Atlas",
		"Atticus",
		"Augustus",
		"Aurelius",
		"Balthasar",
		"Bharat",
		"Bodhi",
		"Bruce",
		"Bruno",
		"Caesar",
		"Caius",
		"Cassius",
		"Castor",
		"Cato",
		"Caxton",
		"Corbett",
		"Cornelius",
		"Cosmo",
		"Cyrus",
		"Damon",
		"Decimus",
		"Demetrius",
		"Divit",
		"Engjell",
		"Evander",
		"Felix",
		"Flavius",
		"Ivo",
		"Hardik",
		"Helios",
		"Hiro",
		"Horatio",
		"Icarus",
		"Jason",
		"Jasper",
		"Julius",
		"Jupiter",
		"Lazarus",
		"Leander",
		"Loki",
		"Lucius",
		"Magnus",
		"Marcellus",
		"Marcus",
		"Marius",
		"Maximus",
		"Mercury",
		"Neptune",
		"Nero",
		"Obi",
		"Octavius",
		"Odysseus",
		"Orion",
		"Orpheus",
		"Osirus",
		"Otto",
		"Ozius",
		"Quintus",
		"Remus",
		"Rhodes",
		"Romulus",
		"Rufus",
		"Scorpius",
		"Sebastian",
		"Seneca",
		"Septimus",
		"Severus",
		"Shadrack",
		"Tarquin",
		"Theon",
		"Thor",
		"Tiberius",
		"Timon",
		"Titus",
		"Urban",
		"Wolfgang",
		"Xerxes",
		"Zephyr",
		"Zotikos"
	};

	public bool IsSelected { get; set; }

}