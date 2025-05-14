
public static class TestFileHelper
{
    public static string LoadInput(string fileName)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Inputs", fileName);
        return File.ReadAllText(path);
    }

    public static string LoadOutput(string fileName)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "RefOutputs", fileName);
        return File.ReadAllText(path);
    }
}
