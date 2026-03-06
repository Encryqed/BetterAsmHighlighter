using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BetterAsmHighlighter.Data;
using BetterAsmHighlighter.Editor;

namespace BetterAsmHighlighter.Options
{
    internal sealed class ColorSettingsControl : UserControl
    {
        // Token display names keyed by classification type
        private static readonly (string TypeKey, string DisplayName)[] TokenEntries =
        {
            (ClassificationTypes.COMMENT,     "Comment"),
            (ClassificationTypes.INSTRUCTION, "Instruction"),
            (ClassificationTypes.REGISTER,    "Register"),
            (ClassificationTypes.DIRECTIVE,   "Directive"),
            (ClassificationTypes.NUMBER,      "Number"),
            (ClassificationTypes.STRING,      "String"),
            (ClassificationTypes.LABEL,       "Label"),
            (ClassificationTypes.STRUCTURE,   "Structure"),
            (ClassificationTypes.MEMBER,      "Member"),
            (ClassificationTypes.FUNCTION,    "Function"),
            (ClassificationTypes.GLOBAL,      "Global"),
            (ClassificationTypes.OPERATOR,    "Operator"),
        };

        private readonly ComboBox ThemeCombo;
        private readonly Dictionary<string, Rectangle> Swatches = new Dictionary<string, Rectangle>();
        private readonly Dictionary<string, CheckBox> BoldChecks = new Dictionary<string, CheckBox>();

        public ColorSettingsControl()
        {
            StackPanel Root = new StackPanel
            {
                Margin = new Thickness(8),
            };

            // Theme selector row
            DockPanel ThemeRow = new DockPanel
            {
                Margin = new Thickness(0, 0, 0, 12),
            };

            TextBlock ThemeLabel = new TextBlock
            {
                Text = "Theme:",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0),
            };

            DockPanel.SetDock(ThemeLabel, Dock.Left);

            ThemeCombo = new ComboBox
            {
                Width = 180,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            foreach (string Name in Themes.Names)
            {
                ThemeCombo.Items.Add(Name);
            }

            ThemeCombo.SelectionChanged += OnThemeSelectionChanged;

            ThemeRow.Children.Add(ThemeLabel);
            ThemeRow.Children.Add(ThemeCombo);
            Root.Children.Add(ThemeRow);

            // Token color grid
            Grid ColorGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 12),
            };

            ColorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            ColorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(44) });
            ColorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            for (int i = 0; i < TokenEntries.Length; i++)
            {
                (string TypeKey, string DisplayName) = TokenEntries[i];

                ColorGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(28) });

                // Token name
                TextBlock NameBlock = new TextBlock
                {
                    Text = DisplayName,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Grid.SetRow(NameBlock, i);
                Grid.SetColumn(NameBlock, 0);
                ColorGrid.Children.Add(NameBlock);

                // Color swatch
                Rectangle Swatch = new Rectangle
                {
                    Width = 36,
                    Height = 20,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    Cursor = Cursors.Hand,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = TypeKey,
                };

                Swatch.MouseLeftButtonDown += OnSwatchClicked;
                Grid.SetRow(Swatch, i);
                Grid.SetColumn(Swatch, 1);
                ColorGrid.Children.Add(Swatch);
                Swatches[TypeKey] = Swatch;

                // Bold checkbox
                CheckBox BoldCheck = new CheckBox
                {
                    Content = "Bold",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(8, 0, 0, 0),
                };

                Grid.SetRow(BoldCheck, i);
                Grid.SetColumn(BoldCheck, 2);
                ColorGrid.Children.Add(BoldCheck);
                BoldChecks[TypeKey] = BoldCheck;
            }

            Root.Children.Add(ColorGrid);

            // Reset button
            Button ResetButton = new Button
            {
                Content = "Reset to Theme",
                Padding = new Thickness(12, 4, 12, 4),
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            ResetButton.Click += OnResetClicked;
            Root.Children.Add(ResetButton);

            Content = Root;
        }

        private void OnThemeSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            string? SelectedTheme = ThemeCombo.SelectedItem as string;
            if (SelectedTheme == null)
                return;

            ApplyPalette(SelectedTheme);
        }

        private void ApplyPalette(string ThemeName)
        {
            if (!Themes.Palettes.TryGetValue(ThemeName, out Dictionary<string, ThemeColor> Palette))
                return;

            foreach (KeyValuePair<string, ThemeColor> Entry in Palette)
            {
                if (Swatches.TryGetValue(Entry.Key, out Rectangle Swatch))
                {
                    Swatch.Fill = new SolidColorBrush(Entry.Value.Foreground);
                }
                if (BoldChecks.TryGetValue(Entry.Key, out CheckBox Check))
                {
                    Check.IsChecked = Entry.Value.bIsBold;
                }
            }
        }

        private void OnSwatchClicked(object Sender, MouseButtonEventArgs E)
        {
            Rectangle Swatch = (Rectangle)Sender;
            SolidColorBrush? CurrentBrush = Swatch.Fill as SolidColorBrush;

            System.Windows.Forms.ColorDialog Dialog = new System.Windows.Forms.ColorDialog
            {
                FullOpen = true,
            };

            if (CurrentBrush != null)
            {
                Dialog.Color = System.Drawing.Color.FromArgb(
                    CurrentBrush.Color.R,
                    CurrentBrush.Color.G,
                    CurrentBrush.Color.B);
            }

            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color NewColor = Color.FromRgb(
                    Dialog.Color.R,
                    Dialog.Color.G,
                    Dialog.Color.B);
                Swatch.Fill = new SolidColorBrush(NewColor);
            }
        }

        private void OnResetClicked(object Sender, RoutedEventArgs E)
        {
            string? SelectedTheme = ThemeCombo.SelectedItem as string;

            if (SelectedTheme != null)
            {
                ApplyPalette(SelectedTheme);
            }
        }

        public void LoadFromSettings(string ThemeName, Dictionary<string, ThemeColor> Colors)
        {
            // Set combo without triggering palette overwrite
            ThemeCombo.SelectionChanged -= OnThemeSelectionChanged;
            ThemeCombo.SelectedItem = ThemeName;
            ThemeCombo.SelectionChanged += OnThemeSelectionChanged;

            // Apply custom colors (or theme defaults if Colors is empty)
            if (Colors != null && Colors.Count > 0)
            {
                foreach (KeyValuePair<string, ThemeColor> Entry in Colors)
                {
                    if (Swatches.TryGetValue(Entry.Key, out Rectangle Swatch))
                    {
                        Swatch.Fill = new SolidColorBrush(Entry.Value.Foreground);
                    }
                    if (BoldChecks.TryGetValue(Entry.Key, out CheckBox Check))
                    {
                        Check.IsChecked = Entry.Value.bIsBold;
                    }
                }
            }
            else
            {
                ApplyPalette(ThemeName);
            }
        }

        public string GetThemeName()
        {
            return ThemeCombo.SelectedItem as string ?? Themes.DARK;
        }

        public Dictionary<string, ThemeColor> GetColors()
        {
            Dictionary<string, ThemeColor> Colors = new Dictionary<string, ThemeColor>();

            foreach (KeyValuePair<string, Rectangle> Pair in Swatches)
            {
                SolidColorBrush? Brush = Pair.Value.Fill as SolidColorBrush;

                if (Brush == null)
                    continue;

                bool bIsBold = BoldChecks.TryGetValue(Pair.Key, out CheckBox Check)
                            && Check.IsChecked == true;

                Colors[Pair.Key] = new ThemeColor(
                    Brush.Color.R,
                    Brush.Color.G,
                    Brush.Color.B,
                    bIsBold);
            }

            return Colors;
        }
    }
}
