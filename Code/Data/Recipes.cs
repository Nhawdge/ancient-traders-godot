using System.Collections.Generic;

public static class Recipes {
    public static Recipe BasicBuildingMaterials => new Recipe("Building Materials", 25, new List<GameResource> { new(Resources.Lumber, 10) }, Resources.BasicBuildingMaterials);
    public static Recipe BasicTools => new Recipe("Basic Tools", 25, new List<GameResource> { new(Resources.Lumber, 10) }, Resources.BasicTools);
    public static Recipe BasicTransportMaterials => new Recipe("Basic Transport Materials", 25, new List<GameResource> { new(Resources.Lumber, 10) }, Resources.BasicTransportMaterials);
    public static Recipe BasicSailingMaterials => new Recipe("Basic Sailing Materials", 25, new List<GameResource> { new(Resources.Lumber, 10) }, Resources.BasicSailingMaterials);
}