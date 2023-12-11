using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using AdofaiTweaks.Translation;
using ExcelDataReader;
using LiteDB;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AdofaiTweaks.Generator
{
    /// <summary>
    /// Generates an <c>AdofaiTweaks.Strings.dll</c> containing a class
    /// <c>TranslationKeys</c> that holds all the translation keys in the
    /// downloaded translations spreadsheet.
    /// </summary>
    internal class TranslationGenerator
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        public static void Main() {
            GenerateDatabase();
            GenerateKeyClass();
        }

        private static void GenerateDatabase() {
            // Required by ExcelDataReader to parse strings in binary BIFF2-5
            // Excel documents encoded with DOS-era code pages.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var db = new LiteDatabase("TweakStrings.db")) {
                db.DropCollection(typeof(TweakString).Name);
                var collection = db.GetCollection<TweakString>();
                using (FileStream stream = File.OpenRead("translations.xlsx"))
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream)) {
                    DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() {
                            UseHeaderRow = true,
                        },
                    });
                    foreach (DataTable table in dataSet.Tables) {
                        // Skip the dashboard sheet as it is just used for
                        // translators to know what needs to be translated
                        if (table.TableName == "DASHBOARD") {
                            continue;
                        }

                        collection.InsertBulk(ReadTweakStrings(table));
                    }
                }
                db.Commit();
            }
        }

        private static IEnumerable<TweakString> ReadTweakStrings(DataTable table) {
            string tweakNameUpperCamel = PascalToUpperCamel(table.TableName);
            foreach (DataRow row in table.Select()) {
                string key = tweakNameUpperCamel + "_" + (row["KEY"] as string);
                foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum))) {
                    yield return new TweakString() {
                        Key = key,
                        Content = row[language.ToString()] as string,
                        Language = language,
                    };
                }
            }
        }

        private static void GenerateKeyClass() {
            StringBuilder sb = new StringBuilder();
            sb.Append("namespace AdofaiTweaks.Strings { ");
            sb.Append("public class TranslationKeys { ");

            using (FileStream stream = File.OpenRead("translations.xlsx"))
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream)) {
                DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration() {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() {
                        UseHeaderRow = true,
                    },
                });
                foreach (DataTable table in dataSet.Tables) {
                    // Skip the dashboard sheet as it is just used for
                    // translators to know what needs to be translated
                    if (table.TableName == "DASHBOARD") {
                        continue;
                    }

                    string tweakName = table.TableName;
                    string tweakNameUpperCamel = PascalToUpperCamel(table.TableName);
                    sb.Append("public class ").Append(tweakName).Append(" { ");
                    foreach (DataRow row in table.Select()) {
                        string shortKey = (row["KEY"] as string).ToUpper();
                        if (shortKey.StartsWith("I_")) {
                            // Ignore key if we don't want to generate code for
                            // it
                            continue;
                        }
                        string fullKey = tweakNameUpperCamel + "_" + shortKey;
                        sb.AppendFormat("public static string {0} = \"{1}\"; ", shortKey, fullKey);
                    }
                    sb.Append("} ");
                }
            }

            sb.Append("} }");

            var compilation =
                CSharpCompilation
                    .Create("AdofaiTweaks.Strings")
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(sb.ToString()));
            var emitResult = compilation.Emit("AdofaiTweaks.Strings.dll");
            if (!emitResult.Success) {
                throw new Exception("Errors: " + string.Join("\n", emitResult.Diagnostics));
            }
        }

        private static string PascalToUpperCamel(string s) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(s[0]);
            for (int i = 1; i < s.Length; i++) {
                if (char.IsUpper(s[i])) {
                    sb.Append('_').Append(s[i]);
                } else {
                    sb.Append(char.ToUpper(s[i]));
                }
            }
            return sb.ToString();
        }
    }
}
