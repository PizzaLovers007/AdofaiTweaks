using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Translation;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Public-facing API for retrieving the translated tweak string from the database.
    /// </summary>
    public static class TweakStrings
    {
        [SyncTweakSettings]
        private static GlobalSettings Settings { get; set; }

        /// <summary>
        /// <para>
        /// Gets the tweak string for the specified key with optional arguments
        /// to insert into the string. The arguments will be inserted via
        /// standard <see cref="string.Format"/> rules.
        /// </para>
        /// <para>
        /// The string will be translated based on the user's current settings.
        /// Use the keys from <see cref="TranslationKeys"/> to ensure the
        /// correct strings are retrieved.
        /// </para>
        /// </summary>
        /// <param name="key">The key of the tweak string.</param>
        /// <param name="args">The arguments to insert into the string.</param>
        /// <returns>The translated string.</returns>
        public static string Get(string key, params object[] args) {
            return GetForLanguage(key, Settings.Language, args);
        }

        /// <summary>
        /// <para>
        /// Gets the tweak string for the specified key and language with
        /// optional arguments to insert into the string. The arguments will be
        /// inserted via standard <see cref="string.Format"/> rules.
        /// </para>
        /// <para>
        /// Use the keys from <see cref="TranslationKeys"/> to ensure the
        /// correct strings are retrieved.
        /// </para>
        /// </summary>
        /// <param name="key">The key of the tweak string.</param>
        /// <param name="language">The language to use for the translation.</param>
        /// <param name="args">The arguments to insert into the string.</param>
        /// <returns>The translated string.</returns>
        public static string GetForLanguage(
            string key, LanguageEnum language, params object[] args) {
            string format = TweakStringsDb.Instance.GetForLanguage(language, key);
            if (format == null) {
                return $"no such key {key}";
            }
            if (args.Length == 0) {
                return format;
            }
            return string.Format(format, args);
        }
    }
}
