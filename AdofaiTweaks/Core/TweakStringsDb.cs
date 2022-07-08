using System.Collections.Generic;
using System.IO;
using AdofaiTweaks.Translation;
using LiteDB;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Wrapper around LiteDB to read the tweak strings from the database. When
    /// accessing any string for a given language, all strings for that language
    /// are read and cached.
    /// </summary>
    internal class TweakStringsDb
    {
        private static TweakStringsDb _instance;

        /// <summary>
        /// Singleton instance for <see cref="TweakStringsDb"/>.
        /// </summary>
        public static TweakStringsDb Instance => _instance ??= new TweakStringsDb();

        private readonly Dictionary<LanguageEnum, Dictionary<string, TweakString>> cache;

        private void LoadFromDb(LanguageEnum language) {
            string dbPath = Path.Combine("Mods", "AdofaiTweaks", "TweakStrings.db");
            using var db = new LiteDatabase(dbPath);
            var collection = db.GetCollection<TweakString>();
            var results = collection.Query()
                .Where(ts => ts.Language == language)
                .ToEnumerable();
            var dict = new Dictionary<string, TweakString>();
            foreach (TweakString tweakString in results) {
                dict[tweakString.Key] = tweakString;
            }
            cache[language] = dict;
        }

        /// <summary>
        /// Gets the tweak string for the given language and key. If no
        /// translation is found, then the English translation is returned.
        /// </summary>
        /// <param name="language">
        /// The language that the tweak string will be translated as.
        /// </param>
        /// <param name="key">The key for the tweak string.</param>
        /// <returns>The translated tweak string.</returns>
        public string GetForLanguage(LanguageEnum language, string key) {
            if (!cache.ContainsKey(language)) {
                LoadFromDb(language);
            }
            Dictionary<string, TweakString> dict = cache[language];
            if (!dict.ContainsKey(key))
            {
                return $"no such key {key}";
            }
            if (string.IsNullOrEmpty(dict[key].Content)) {
                return cache[LanguageEnum.ENGLISH][key].Content;
            }
            return dict[key].Content;
        }

        private TweakStringsDb() {
            cache = new Dictionary<LanguageEnum, Dictionary<string, TweakString>>();
            LoadFromDb(LanguageEnum.ENGLISH);
        }
    }
}
