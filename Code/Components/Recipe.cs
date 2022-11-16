using System.Collections.Generic;

public record Recipe(string Name, int CraftTime, List<GameResource> Resources, ManufacturedResources Result);