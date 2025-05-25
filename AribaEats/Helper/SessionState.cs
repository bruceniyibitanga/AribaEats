namespace AribaEats.Helper;

/// <summary>
/// Provides session-level state management to track user-specific actions or flags during a session.
/// This static helper class is designed for managing simple application states.
/// </summary>
public static class SessionState
{
   /// <summary>
   /// Indicates whether the user has visited a restaurant during the current session.
   /// Defaults to <c>false</c>.
   /// </summary>
   public static bool HasVisitedRestaurant = false;

   /// <summary>
   /// Resets the <see cref="HasVisitedRestaurant"/> flag to <c>false</c>, 
   /// typically used to clear session state when needed.
   /// </summary>
   public static void ResetVisitedRestaurant()
   {
      HasVisitedRestaurant = false;
   }
}
