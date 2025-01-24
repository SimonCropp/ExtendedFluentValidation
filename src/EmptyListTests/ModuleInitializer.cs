public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        UseProjectRelativeDirectory("");
        VerifyDiffPlex.Initialize();
        ValidatorConventions.ValidateEmptyLists();
    }
}