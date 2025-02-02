using HopFrame.Core.Config;

namespace HopFrame.Core.Callbacks;

public static class CallbackTypes {

    private const string Prefix = "HopFrame.";
    private const string CreateEntryPrefix = Prefix + "Entry.Create.";
    private const string UpdateEntryPrefix = Prefix + "Entry.Update.";
    private const string DeleteEntryPrefix = Prefix + "Entry.Delete.";

    public static string CreateEntry(TableConfig config) => CreateEntryPrefix + config.PropertyName;
    public static string UpdateEntry(TableConfig config) => UpdateEntryPrefix + config.PropertyName;
    public static string DeleteEntry(TableConfig config) => DeleteEntryPrefix + config.PropertyName;

    public static string ConstructCallbackName(CallbackType type, TableConfig config) {
        return type switch {
            CallbackType.CreateEntry => CreateEntry(config),
            CallbackType.UpdateEntry => UpdateEntry(config),
            CallbackType.DeleteEntry => DeleteEntry(config),
            _ => Prefix
        };
    }

}

public enum CallbackType {
    CreateEntry = 0,
    UpdateEntry = 1,
    DeleteEntry = 2
}
