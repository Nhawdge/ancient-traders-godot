public interface ISelectable {
    public bool IsSelected { get; set; }

    public string GetHeader();
    public string GetData();
    public delegate void InfoUpdated();
}