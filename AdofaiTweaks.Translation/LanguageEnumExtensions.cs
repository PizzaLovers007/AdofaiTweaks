namespace AdofaiTweaks.Translation
{
    /// <summary>
    /// Extensions for <see cref="LanguageEnum"/>.
    /// </summary>
    public static class LanguageEnumExtensions
    {
        /// <summary>
        /// Returns <c>true</c> for languages that have hard to read symbols on
        /// smaller fonts (e.g. Korean, Chinese, etc.).
        /// </summary>
        /// <param name="language">The language to check.</param>
        /// <returns><c>true</c> if the language contains symbols.</returns>
        public static bool IsSymbolLanguage(this LanguageEnum language) {
            return language == LanguageEnum.KOREAN
                || language == LanguageEnum.CHINESE_SIMPLIFIED;
        }
    }
}
