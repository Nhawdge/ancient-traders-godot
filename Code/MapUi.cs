using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Building;

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
			settlement.InfoUpdated += () => {
				GetNode<Label>("UI/SelectedView/Header").Set("text", selected.GetHeader());
				GetNode<Label>("UI/SelectedView/Data").Set("text", selected.GetData());
			};

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
				if (currentStatus is Building.StatusOptions.Building or Building.StatusOptions.Working) {
					var progress = new ProgressBar();
					Action updater = () => { progress.Value = building.Progress; progress.MaxValue = building.ActiveRecipe.CraftTime; };
					building.ProgressChanged += updater.Invoke;
					progress.TreeExiting += () => building.ProgressChanged -= updater.Invoke;
					group.AddChild(progress);
				}
				foreach (var worker in building.GetChildren().OfType<Worker>()) {
					var button = new Button() { Text = worker.Name };
					group.AddChild(button);
					button.Disabled = !worker.IsAvailable;
					button.Pressed += worker.Select;
				}
				var container = new PanelContainer();
				var grid = new GridContainer();
				grid.Columns = 3;
				foreach (var recipe in building.AvailableRecipes) {
					var button = new Button() { Text = recipe.Name };
					if (building.Status is not StatusOptions.Idle) {
						button.Disabled = true;
						button.TooltipText = "Building is busy";
					} else {
						var canCraft = building.HasCorrectInventory(recipe);
						if (!canCraft) {
							button.Disabled = true;
							button.TooltipText = "Insufficient resources";
						}
					}
					button.Pressed += () => {
						building.Craft(recipe);
						UpdateSelected(selected);
					};
					grid.AddChild(button);
				}
				container.AddChild(grid);
				group.AddChild(container);

				buildingsNode.AddChild(group);
			}
			var inventoryContainer = new PanelContainer();
			var inventorylayout = new VBoxContainer();

			foreach (var row in settlement.Inventory) {
				var inventoryLabel = new Label() { Text = $"{row.Key}: {row.Value}" };
				inventorylayout.AddChild(inventoryLabel);
			}
			inventoryContainer.AddChild(inventorylayout);
			buildingsNode.AddChild(inventoryContainer);
		} else if (selected is ResourceCamp camp) {
			GetNode<Control>("UI/SelectedView/Camp").Show();
			camp.InfoUpdated += () => {
				GetNode<Label>("UI/SelectedView/Header").Set("text", selected.GetHeader());
				GetNode<Label>("UI/SelectedView/Data").Set("text", selected.GetData());
			};
			var workerListing = new VBoxContainer();
			foreach (var worker in camp.GetChildren().OfType<Worker>()) {
				var button = new Button() { Text = worker.Name };
				button.Pressed += worker.Select;
				workerListing.AddChild(button);

				var harvest = new Button() { Text = "Harvest" };
				harvest.Pressed += camp.HarvestResource;
				workerListing.AddChild(harvest);
			}
			var campNode = GetNode<Control>("UI/SelectedView/Camp");
			campNode.GetChildren().ToList().ForEach(child => child.QueueFree());
			campNode.AddChild(workerListing);
		} else if (selected is Worker worker) {
			worker.InfoUpdated += () => {
				GetNode<Label>("UI/SelectedView/Header").Set("text", selected.GetHeader());
				GetNode<Label>("UI/SelectedView/Data").Set("text", selected.GetData());
			};
			var workerNode = GetNode<Control>("UI/SelectedView/Worker");
			workerNode.Show();

			var inventoryPanel = GetNode<PanelContainer>("UI/SelectedView/Worker/Inventory");
			inventoryPanel.GetChildren().ToList().ForEach(child => child.QueueFree());
			var inventoryGrid = new GridContainer();
			inventoryGrid.Columns = 3;
			foreach (var row in worker.Inventory) {
				var inventoryLabel = new Label() { Text = $"{row.Key}: {row.Value}" };
				var addInventory = new Button() { Text = "+" };
				addInventory.Pressed += worker.PickUpInventory(row.Key);
				var removeInventory = new Button() { Text = "-" };
				removeInventory.Pressed += worker.DepositInventory(row.Key);
				inventoryGrid.AddChild(inventoryLabel);
				inventoryGrid.AddChild(addInventory);
				inventoryGrid.AddChild(removeInventory);
			}
			inventoryPanel.AddChild(inventoryGrid);
			GetNode<Control>("UI/SelectedView/Worker").AddChild(inventoryPanel);

			var skills = GetNode<Control>("UI/SelectedView/Worker/Skills");
			skills.GetChildren().ToList().ForEach(child => child.QueueFree());
			foreach (var skill in worker.Skills) {
				var label = new Label() { Text = skill.Name + ": " + skill.Amount };
				skills.AddChild(label);
			}

			var travelNode = GetNode<Control>("UI/SelectedView/Worker/Travel");
			var destinations = GetChildren().OfType<ISelectable>().ToList();

			travelNode.GetChildren().ToList().ForEach(child => child.QueueFree());

			var container = new PanelContainer();
			var innercontainer = new VBoxContainer();
			foreach (var dest in destinations) {
				var button = new Button() { Text = (dest as Node).Name };
				button.Pressed += () => {
					worker.Travel(dest);
					UpdateSelected(dest);
				};
				innercontainer.AddChild(button);
			}
			container.AddChild(innercontainer);
			travelNode.AddChild(container);

		}

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
		UpdateSelected(settlementToBuildIn);
	}
}