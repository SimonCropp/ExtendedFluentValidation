public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        UseProjectRelativeDirectory("");
        ValidatorConventions.ValidateEmptyLists();
        VerifierSettings.InitializePlugins();
    }
}