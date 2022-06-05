using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdofaiTweaks.Core
{
    /// <summary>
    /// Additional <see cref="GUILayout"/> components.
    /// </summary>
    public static class MoreGUILayout
    {
        /// <summary>
        /// Begins an indented section with a given indentation size.
        /// </summary>
        /// <param name="indentSize">The size of the indentation.</param>
        public static void BeginIndent(float indentSize = 20f) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentSize);
            GUILayout.BeginVertical();
        }

        /// <summary>
        /// Ends an indented section.
        /// </summary>
        public static void EndIndent() {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Displays RGB sliders for the given color.
        /// </summary>
        /// <param name="color">The color to set the sliders to.</param>
        /// <returns>The resulting color from any changed sliders.</returns>
        public static Color ColorRgbSliders(Color color) {
            float oldR = Mathf.Round(color.r * 255);
            float oldG = Mathf.Round(color.g * 255);
            float oldB = Mathf.Round(color.b * 255);
            float newR = NamedSlider("R:", oldR, 0, 255, 300f, 1, 40f);
            float newG = NamedSlider("G:", oldG, 0, 255, 300f, 1, 40f);
            float newB = NamedSlider("B:", oldB, 0, 255, 300f, 1, 40f);
            if (oldR != newR || oldG != newG || oldB != newB) {
                return new Color(newR / 255, newG / 255, newB / 255);
            }
            return color;
        }

        /// <summary>
        /// Displays RGBA sliders for the given color.
        /// </summary>
        /// <param name="color">The color to set the sliders to.</param>
        /// <returns>The resulting color from any changed sliders.</returns>
        public static Color ColorRgbaSliders(Color color) {
            float oldR = Mathf.Round(color.r * 255);
            float oldG = Mathf.Round(color.g * 255);
            float oldB = Mathf.Round(color.b * 255);
            float oldA = Mathf.Round(color.a * 255);
            float newR = NamedSlider("R:", oldR, 0, 255, 300f, 1, 40f);
            float newG = NamedSlider("G:", oldG, 0, 255, 300f, 1, 40f);
            float newB = NamedSlider("B:", oldB, 0, 255, 300f, 1, 40f);
            float newA = NamedSlider("A:", oldA, 0, 255, 300f, 1, 40f);
            if (oldR != newR || oldG != newG || oldB != newB || oldA != newA) {
                return new Color(newR / 255, newG / 255, newB / 255, newA / 255);
            }
            return color;
        }

        /// <summary>
        /// Displays two RGB sliders for two different colors horizontally
        /// adjacent to each other.
        /// </summary>
        /// <param name="color1">The left color to set the sliders to.</param>
        /// <param name="color2">The right color to set the sliders to.</param>
        /// <returns>
        /// The resulting colors pair from any changed sliders.
        /// </returns>
        public static (Color, Color) ColorRgbSlidersPair(Color color1, Color color2) {
            float newR1, newR2, newG1, newG2, newB1, newB2;
            float oldR1 = Mathf.Round(color1.r * 255);
            float oldG1 = Mathf.Round(color1.g * 255);
            float oldB1 = Mathf.Round(color1.b * 255);
            float oldR2 = Mathf.Round(color2.r * 255);
            float oldG2 = Mathf.Round(color2.g * 255);
            float oldB2 = Mathf.Round(color2.b * 255);
            (newR1, newR2) = NamedSliderPair("R:", "R:", oldR1, oldR2, 0, 255, 300f, 1, 40f);
            (newG1, newG2) = NamedSliderPair("G:", "G:", oldG1, oldG2, 0, 255, 300f, 1, 40f);
            (newB1, newB2) = NamedSliderPair("B:", "B:", oldB1, oldB2, 0, 255, 300f, 1, 40f);
            if (oldR1 != newR1 || oldG1 != newG1 || oldB1 != newB1) {
                color1 = new Color(newR1 / 255, newG1 / 255, newB1 / 255);
            }
            if (oldR2 != newR2 || oldG2 != newG2 || oldB2 != newB2) {
                color2 = new Color(newR2 / 255, newG2 / 255, newB2 / 255);
            }
            return (color1, color2);
        }

        /// <summary>
        /// Displays two RGBA sliders for two different colors horizontally
        /// adjacent to each other.
        /// </summary>
        /// <param name="color1">The left color to set the sliders to.</param>
        /// <param name="color2">The right color to set the sliders to.</param>
        /// <returns>
        /// The resulting colors pair from any changed sliders.
        /// </returns>
        public static (Color, Color) ColorRgbaSlidersPair(Color color1, Color color2) {
            float newR1, newR2, newG1, newG2, newB1, newB2, newA1, newA2;
            float oldR1 = Mathf.Round(color1.r * 255);
            float oldG1 = Mathf.Round(color1.g * 255);
            float oldB1 = Mathf.Round(color1.b * 255);
            float oldA1 = Mathf.Round(color1.a * 255);
            float oldR2 = Mathf.Round(color2.r * 255);
            float oldG2 = Mathf.Round(color2.g * 255);
            float oldB2 = Mathf.Round(color2.b * 255);
            float oldA2 = Mathf.Round(color2.a * 255);
            (newR1, newR2) = NamedSliderPair("R:", "R:", oldR1, oldR2, 0, 255, 300f, 1, 40f);
            (newG1, newG2) = NamedSliderPair("G:", "G:", oldG1, oldG2, 0, 255, 300f, 1, 40f);
            (newB1, newB2) = NamedSliderPair("B:", "B:", oldB1, oldB2, 0, 255, 300f, 1, 40f);
            (newA1, newA2) = NamedSliderPair("A:", "A:", oldA1, oldA2, 0, 255, 300f, 1, 40f);
            if (oldR1 != newR1 || oldG1 != newG1 || oldB1 != newB1 || oldA1 != newA1) {
                color1 = new Color(newR1 / 255, newG1 / 255, newB1 / 255, newA1 / 255);
            }
            if (oldR2 != newR2 || oldG2 != newG2 || oldB2 != newB2 || oldA2 != newA2) {
                color2 = new Color(newR2 / 255, newG2 / 255, newB2 / 255, newA2 / 255);
            }
            return (color1, color2);
        }

        /// <summary>
        /// Displays a slider with a name on the left and its value on the
        /// right.
        /// </summary>
        /// <param name="name">The name to show on the left.</param>
        /// <param name="value">The value of the slider.</param>
        /// <param name="leftValue">The minimum value for the slider.</param>
        /// <param name="rightValue">The maximum value for the slider.</param>
        /// <param name="sliderWidth">The width of the slider.</param>
        /// <param name="roundNearest">
        /// The fraction to round the value to. By default will result in no
        /// rounding.
        /// </param>
        /// <param name="labelWidth">
        /// The width of the name label on the left. By default will expand to
        /// fit the name's width.
        /// </param>
        /// <param name="valueFormat">
        /// The formatting for the value label on the right. By default will
        /// simply show the value.
        /// </param>
        /// <returns>The resulting value from any change in the slider.</returns>
        public static float NamedSlider(
            string name,
            float value,
            float leftValue,
            float rightValue,
            float sliderWidth,
            float roundNearest = 0,
            float labelWidth = 0,
            string valueFormat = "{0}") {
            GUILayout.BeginHorizontal();
            float newValue =
                NamedSliderContent(
                    name,
                    value,
                    leftValue,
                    rightValue,
                    sliderWidth,
                    roundNearest,
                    labelWidth,
                    valueFormat);
            GUILayout.EndHorizontal();
            return newValue;
        }

        /// <summary>
        /// Displays a two sliders that are horizontally adjacent with their
        /// names on the left and their values on the right.
        /// </summary>
        /// <param name="name1">
        /// The name for the left slider to show on the left.
        /// </param>
        /// <param name="name2">
        /// The name for the right slider to show on the left.
        /// </param>
        /// <param name="value1">The value of the left slider.</param>
        /// <param name="value2">The value of the right slider.</param>
        /// <param name="leftValue">The minimum value for the slider.</param>
        /// <param name="rightValue">The maximum value for the slider.</param>
        /// <param name="sliderWidth">The width of the slider.</param>
        /// <param name="roundNearest">
        /// The fraction to round the value to. By default will result in no
        /// rounding.
        /// </param>
        /// <param name="labelWidth">
        /// The width of the name label on the left. By default will expand to
        /// fit the name's width.
        /// </param>
        /// <param name="valueFormat">
        /// The formatting for the value label on the right. By default will
        /// simply show the value.
        /// </param>
        /// <returns>
        /// The resulting values pair from any change in the sliders.
        /// </returns>
        public static (float, float) NamedSliderPair(
            string name1,
            string name2,
            float value1,
            float value2,
            float leftValue,
            float rightValue,
            float sliderWidth,
            float roundNearest = 0,
            float labelWidth = 0,
            string valueFormat = "{0}") {
            GUILayout.BeginHorizontal();
            float newValue1 =
                NamedSliderContent(
                    name1,
                    value1,
                    leftValue,
                    rightValue,
                    sliderWidth,
                    roundNearest,
                    labelWidth,
                    valueFormat);
            float newValue2 =
                NamedSliderContent(
                    name2,
                    value2,
                    leftValue,
                    rightValue,
                    sliderWidth,
                    roundNearest,
                    labelWidth,
                    valueFormat);
            GUILayout.EndHorizontal();
            return (newValue1, newValue2);
        }

        private static float NamedSliderContent(
            string name,
            float value,
            float leftValue,
            float rightValue,
            float sliderWidth,
            float roundNearest = 0,
            float labelWidth = 0,
            string valueFormat = "{0}") {
            if (labelWidth == 0) {
                GUILayout.Label(name);
                GUILayout.Space(4f);
            } else {
                GUILayout.Label(name, GUILayout.Width(labelWidth));
            }
            float newValue =
                GUILayout.HorizontalSlider(
                    value, leftValue, rightValue, GUILayout.Width(sliderWidth));
            if (roundNearest != 0) {
                newValue = Mathf.Round(newValue / roundNearest) * roundNearest;
            }
            GUILayout.Space(8f);
            GUILayout.Label(string.Format(valueFormat, newValue), GUILayout.Width(40f));
            GUILayout.FlexibleSpace();
            return newValue;
        }

        /// <summary>
        /// Displays a text field with a name on the left.
        /// </summary>
        /// <param name="name">The name to show on the left.</param>
        /// <param name="value">The value of the text field.</param>
        /// <param name="fieldWidth">The width of the text field.</param>
        /// <param name="labelWidth">
        /// The width of the name label on the left. By default will expand to
        /// fit the name's width.
        /// </param>
        /// <returns>
        /// The resulting value of the text field from any changes.
        /// </returns>
        public static string NamedTextField(
            string name,
            string value,
            float fieldWidth,
            float labelWidth = 0) {
            GUILayout.BeginHorizontal();
            string newValue = NamedTextFieldContent(name, value, fieldWidth, labelWidth);
            GUILayout.EndHorizontal();
            return newValue;
        }

        /// <summary>
        /// Displays two text fields that are horizontally adjacent with their
        /// names on the left.
        /// </summary>
        /// <param name="name1">
        /// The name of the left text field to show on the left.
        /// </param>
        /// <param name="name2">
        /// The name of the right text field to show on the left.
        /// </param>
        /// <param name="value1">The value of the left text field.</param>
        /// <param name="value2">The value of the right text field.</param>
        /// <param name="fieldWidth">The width of the text field.</param>
        /// <param name="labelWidth">
        /// The width of the name label on the left. By default will expand to
        /// fit the name's width.
        /// </param>
        /// <returns>
        /// The resulting values pair from any change in the text fields.
        /// </returns>
        public static (string, string) NamedTextFieldPair(
            string name1,
            string name2,
            string value1,
            string value2,
            float fieldWidth,
            float labelWidth = 0) {
            GUILayout.BeginHorizontal();
            string newValue1 = NamedTextFieldContent(name1, value1, fieldWidth, labelWidth);
            string newValue2 = NamedTextFieldContent(name2, value2, fieldWidth, labelWidth);
            GUILayout.EndHorizontal();
            return (newValue1, newValue2);
        }

        private static string NamedTextFieldContent(
            string name,
            string value,
            float fieldWidth,
            float labelWidth = 0) {
            if (labelWidth == 0) {
                GUILayout.Label(name);
                GUILayout.Space(4f);
            } else {
                GUILayout.Label(name, GUILayout.Width(labelWidth));
            }
            string newValue = GUILayout.TextField(value, GUILayout.Width(fieldWidth));
            GUILayout.FlexibleSpace();
            return newValue;
        }

        /// <summary>
        /// Displays a generic list of items where only one can be selected. The
        /// lists can be reordered via up and down buttons to the left of the
        /// items.
        /// </summary>
        /// <typeparam name="T">The object type to display.</typeparam>
        /// <param name="list">The list of generic objects to display.</param>
        /// <param name="selectedIndex">
        /// The currently selected index. If set to any value outside of the
        /// range of the list, no items will appear as selected.
        /// </param>
        /// <param name="nameFunc">
        /// The function determining what gets displayed for each object of type
        /// <see cref="T"/>.
        /// </param>
        /// <returns><c>true</c> if the selected item changed.</returns>
        public static bool ToggleList<T>(List<T> list, ref int selectedIndex, Func<T, string> nameFunc) {
            bool changed = false;
            int moveUp = -1, moveDown = -1;
            for (int i = 0; i < list.Count; i++) {
                T curr = list[i];
                string name = nameFunc.Invoke(curr);
                GUILayout.BeginHorizontal();

                // Move up/down
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("▲") && i > 0) {
                    moveUp = i;
                }
                if (GUILayout.Button("▼") && i < list.Count - 1) {
                    moveDown = i;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(8f);

                if (GUILayout.Toggle(selectedIndex == i, name) && selectedIndex != i) {
                    selectedIndex = i;
                    changed = true;
                }
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
            }
            if (moveUp != -1) {
                changed = true;
                T temp = list[moveUp];
                list[moveUp] = list[moveUp - 1];
                list[moveUp - 1] = temp;
                if (moveUp - 1 == selectedIndex) {
                    selectedIndex++;
                } else if (moveUp == selectedIndex) {
                    selectedIndex--;
                }
            } else if (moveDown != -1) {
                changed = true;
                T temp = list[moveDown];
                list[moveDown] = list[moveDown + 1];
                list[moveDown + 1] = temp;
                if (moveDown + 1 == selectedIndex) {
                    selectedIndex--;
                } else if (moveDown == selectedIndex) {
                    selectedIndex++;
                }
            }

            return changed;
        }

        /// <summary>
        /// Displays a horizontal line.
        /// </summary>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="length">
        /// The length of the line. By default expands to take the remaining
        /// width of the GUI.
        /// </param>
        public static void HorizontalLine(float thickness, float length = 0f) {
            GUILayout.Box(GUIContent.none, new GUIStyle() {
                margin = new RectOffset(8, 8, 4, 4),
                padding = new RectOffset(),
                fixedHeight = thickness,
                fixedWidth = length,
                normal = {
                    background = Texture2D.whiteTexture,
                },
            });
        }
    }
}
