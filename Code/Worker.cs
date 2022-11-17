using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Worker : Node, ISelectable {
	// Called when the node enters the scene tree for the first time.

	public bool IsAvailable { get; set; } = true;
	public Dictionary<Resources, int> Inventory { get; set; } = new();

	public List<WorkerSkill> Skills { get; set; } = new List<WorkerSkill>();
	public override void _Ready() {
		var me = GetNode(".");

		foreach (var skill in Enum.GetValues<Skills>()) {
			var workerSkill = new WorkerSkill(skill, 0);
			Skills.Add(workerSkill);
		}
		me.Name = Names.ElementAt(new Random().Next(0, Names.Count));
	}

	public override void _Process(double delta) {
		if (IsSelected)
			EmitSignal(SignalName.InfoUpdated);
	}

	public string GetHeader() {
		return Name;
	}

	public string GetData() {
		return "Worker";
	}

	public void Select() {
		IsSelected = true;
		GetNode<MapUi>("/root/Map/").UpdateSelected(this);
	}

	public void Travel(ISelectable targetNode) {
		var currentParent = GetParent();
		currentParent.RemoveChild(this);
		(targetNode as Node).AddChild(this);
	}

	public Action DepositInventory(Resources resource) {
		return () => {
			var parent = GetParent();
			if (parent is Settlement settlement) {
				if (!settlement.Inventory.TryAdd(resource, 1)) {
					settlement.Inventory[resource]++;
				}
				Inventory[resource]--;
			}
		};
	}

	public Action PickUpInventory(Resources resource) {
		return () => {
			var parent = GetParent();
			if (parent is Settlement settlement) {
				if (!settlement.Inventory.TryAdd(resource, 1)) {
					settlement.Inventory[resource]--;
				}
				Inventory[resource]++;
			}
		};
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

	[Signal]
	public delegate void InfoUpdatedEventHandler();
}