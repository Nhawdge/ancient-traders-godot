using Godot;

public interface ISelectable {
    public bool IsSelected { get; set; }

    public string GetHeader();
    public string GetData();
    [Signal]
    public delegate void InfoUpdatedEventHandler();
}