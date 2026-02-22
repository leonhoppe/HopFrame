namespace HopFrame.Tests.Core;

public class TestModel {
    public int Id { get; set; }
    public string Name { get; set; }

    public int Method() => 42;

    public int FieldBacking;
}