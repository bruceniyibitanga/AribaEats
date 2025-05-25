namespace AribaEats.Helper;

public static class SessionState
{
   public static bool HasVisitedRestaurant = false;

   public static void ResetVisitedRestaurant()
   {
      HasVisitedRestaurant = false;
   }
}
