// using ImGuiNET;

internal class ConfigValue<T>
{
    public bool constantUpdating;
    public T value;

    public ConfigValue(T value, bool constantUpdating = false)
    {
        this.value = value;
        this.constantUpdating = constantUpdating;
    }
}

internal class ConfigValues
{
    public static ConfigValue<float> speed = new ConfigValue<float>(1.0f);
    public static ConfigValue<float> jump = new ConfigValue<float>(1.0f);
    public static ConfigValue<float> flySpeed = new ConfigValue<float>(5.0f);
    public static bool windowLocked = false;
    public static ConfigValue<bool> fly = new ConfigValue<bool>(false);
    public static ConfigValue<bool> eruptionSpawn = new ConfigValue<bool>(false);
    public static ConfigValue<bool> noFallDamage = new ConfigValue<bool>(false);
    public static ConfigValue<bool> infiniteStamina = new ConfigValue<bool>(false);
    public static ConfigValue<bool> statusLock = new ConfigValue<bool>(false);
    // public static ImGuiWindowFlags everythingWindowFlags = ImGuiWindowFlags.None;
}