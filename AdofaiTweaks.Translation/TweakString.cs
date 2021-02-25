namespace AdofaiTweaks.Translation
{
    /// <summary>
    /// The POCO for storing translation data.
    /// </summary>
    public class TweakString
    {
        /// <summary>
        /// The translation key for this tweak string.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The content of the tweak string.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The language of the tweak string.
        /// </summary>
        public LanguageEnum Language { get; set; }
    }
}
