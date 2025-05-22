namespace AribaEats.Helper;

public static class SessionState
{
    public static bool HasVisitedOrderScreen = false;

    public static void Reset()
    {
        HasVisitedOrderScreen = false;
    }
}