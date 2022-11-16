using System;
using System.Linq;
using Godot;

public partial class MapUi : Node2D {

	public override void _Ready() { }

	[Signal]
	public delegate void SettlementHireWorkerEventHandler();

	[Signal]
	public delegate void CampHarvestEventHandler();

	public ISelectable Selected { get; set; }

	public double Time { get; set; }
	public override void _Process(double delta) {
		Time += delta;
		var days = (int) (Time / 100);
		GetNode<Label>("UI/Time/TimeLabel").Set("text", $"{days} day{(days == 1 ? "" : "s")}");
		GetNode<ProgressBar>("UI/Time/TimeProgress").Set("value", Time % 100);
	}

	public void UpdateSelected(ISelectable selected) {
		if (Selected is not null) {
			Selected.IsSelected = false;
		}
		Selected = selected;
		selected.IsSelected = true;

		var buttonContainers = GetNode<Control>("UI/SelectedView/").GetChildren();
		foreach (var child in buttonContainers) {
			if (child is Control control) {
				if (child is not Label) {
					control.Hide();
				}
			}
		}

		if (selected is Settlement settlement) {
			var settlementNode = GetNode<Control>("UI/SelectedView/Settlement");
			settlementNode.Show();

			var workerListing = settlementNode.GetNode<Control>("Workers");
			workerListing.GetChildren().ToList().ForEach(child => child.QueueFree());

			foreach (var worker in settlement.GetChildren().OfType<Worker>()) {
				var button = new Button() { Text = worker.Name };
				workerListing.AddChild(button);
				button.Pressed += worker.Select;
			}

			var buildingsNode = settlementNode.GetNode<Control>("Buildings");
			buildingsNode.GetChildren().ToList().ForEach(child => child.QueueFree());

			foreach (var building in settlement.GetChildren().OfType<Building>()) {
				var group = new VBoxContainer();
				var label = new Label() { Text = building.Name };
				group.AddChild(label);
				var currentStatus = building.Status;
				var status = new Label() { Text = currentStatus.ToString() };
				group.AddChild(status);
				if (currentStatus == Building.StatusOptions.Building) {
					var progress = new ProgressBar() { Value = building.Progress, MaxValue = 100 };
					building.ProgressChanged += () => progress.Value = building.Progress;
					group.AddChild(progress);
				}

				foreach (var worker in building.GetChildren().OfType<Worker>()) {
					var button = new Button() { Text = worker.Name };
					group.AddChild(button);
					button.Disabled = !worker.IsAvailable;
					button.Pressed += worker.Select;
				}

				buildingsNode.AddChild(group);
			}
		}

		if (selected is ResourceCamp camp) {
			GetNode<Control>("UI/SelectedView/Camp").Show();
		}

		if (selected is Worker workerNode) {
			GetNode<Control>("UI/SelectedView/Worker").Show();
			var skills = GetNode<Control>("UI/SelectedView/Worker/Skills");
			foreach (var skill in workerNode.Skills) {
				var label = new Label() { Text = skill.Name + ": " + skill.Amount };
				skills.AddChild(label);
			}
		}

		GetNode<Label>("UI/SelectedView/Header").Set("text", selected.GetHeader());
		GetNode<Label>("UI/SelectedView/Data").Set("text", selected.GetData());
	}

	public void SettlementHireWorkerClick() {
		EmitSignal(SignalName.SettlementHireWorker);
	}

	public void CampHarvestClick() {
		EmitSignal(SignalName.CampHarvest);
	}

	public void WorkerTravelToggle(bool pressed) {
		GetNode<Control>("UI/SelectedView/Worker/Travel").Visible = pressed;
	}
	public void WorkerSkillsToggle(bool pressed) {
		GetNode<Control>("UI/SelectedView/Worker/Skills").Visible = pressed;
	}
	public void WorkerCraftToggle(bool pressed) {
		GetNode<Control>("UI/SelectedView/Worker/Crafting").Visible = pressed;
	}

	public void WorkerCraftWorkshop() {
		var workerToBuild = Selected as Worker;
		var settlementToBuildIn = workerToBuild.GetParent() as Settlement;
		GD.Print($"Building workshop in {settlementToBuildIn.Name} for {workerToBuild.Name}");
		var workshop = new Building();
		workshop.Name = "Workshop";
		workerToBuild.GetParent().RemoveChild(workerToBuild);
		workshop.AddChild(workerToBuild);
		workerToBuild.IsAvailable = false;
		settlementToBuildIn.AddChild(workshop);
	}
}