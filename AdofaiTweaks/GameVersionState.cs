namespace AdofaiTweaks {
    /// <summary>
    /// Check feature availability by the game version.
    /// </summary>
    public static class GameVersionState {
        // Setup
        static GameVersionState() {
            AsyncInputAvailable = AdofaiTweaks.ReleaseNumber >= 98;
        }

        /// <summary>
        /// Whether the executing version of the game has Asynchronous Input System related types.
        /// </summary>
        public static readonly bool AsyncInputAvailable;
    }
}