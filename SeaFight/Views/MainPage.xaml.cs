using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SeaFight.Converters;
using SeaFight.Enums;
using SeaFight.Extensions;
using SeaFight.Triggers;
using SeaFight.ViewModels;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        static Dictionary<Color, Color> themePalette = new Dictionary<Color, Color>();

        MainViewModel viewModel;

        int colorPickerCellsCount;
        public int ColorPickerCellsCount
        {
            get => colorPickerCellsCount;

            private set
            {
                colorPickerCellsCount = value;
                OnPropertyChanged(nameof(ColorPickerCellsCount));
            }
        }

        protected void InitializeView(out int count)
        {
            count = colorPicker.Children.Count;
            CreateResources();
            CreateField("BattleFieldScale", "BattleCellStyle");
            CreateThemes("MainBackgroundColor", "FrameBackgoundColor", count, 0.7);
        }

        public MainPage()
        {
            InitializeComponent();
            InitializeView(out int cellsCount);
            if (Device.RuntimePlatform.Equals(Device.Android))
                Task.Run(async () =>
                {
                    await Task.Delay(1500); //bug with slow UI initializing
                    ColorPickerCellsCount = cellsCount;
                });
            else ColorPickerCellsCount = cellsCount;
        }

        void CreateResources()
        {
            if (Resources.Count > 0) return;
            if (Application.Current.Resources?.MergedDictionaries?.Count > 0)
                Resources.AddRange(Application.Current.Resources.MergedDictionaries.FirstOrDefault());
        }

        void CreateThemes(string colorName, string subColorName, int cellsCount, double saturation)
        {
            if (themePalette.Count > 0) return;

            var resDictionaryIsIntact = true;
            var useDictionaryColors = true;

            if (!Resources.ContainsKeys(colorName.GenerateAggregatedStrings(1, cellsCount).ToArray()))
            {
                ErrorDetected(colorName, ReasonType.ResourceDictionaryError);
                useDictionaryColors = false;
            }
            if (!Resources.ContainsKeys(subColorName.GenerateAggregatedStrings(1, cellsCount).ToArray()))
            {
                ErrorDetected(subColorName, ReasonType.ResourceDictionaryError);
                resDictionaryIsIntact = false;
            }

            try
            {
                if (useDictionaryColors && resDictionaryIsIntact)
                    for (int i = 1; i < cellsCount + 1; ++i)
                    {
                        var color = colorName.AggregateStringWith(i);
                        var subColor = subColorName.AggregateStringWith(i);
                        themePalette.Add((Color)Resources[color], (Color)Resources[subColor]);
                    }
                else if (!useDictionaryColors && resDictionaryIsIntact)
                    for (int i = 1; i < cellsCount + 1; ++i)
                    {
                        var subColor = subColorName.AggregateStringWith(i);
                        themePalette.Add(colorPicker.Children[i - 1].BackgroundColor, (Color)Resources[subColor]);
                    }
                else
                    for (int i = 1; i < cellsCount + 1; ++i)
                    {
                        var color = colorPicker.Children[i - 1].BackgroundColor;
                        themePalette.Add(color, color.WithSaturation(saturation));
                    }
            }
            catch (Exception ex)
            {
                ErrorDetected(ex.Message + ex.StackTrace, ReasonType.Exception);
            }
        }

        void CreateField(string scaleName, string cellStyleName)
        {
            var scale = 1;
            var cellStyle = new Style(typeof(Button));
            var cellLength = new GridLength(1, GridUnitType.Star);

            if (Resources.ContainsKey(scaleName))
                scale = (int)Resources[scaleName];

            BindingContext = viewModel = new MainViewModel(scale, "Jhon Doe");

            if (Resources.ContainsKey(cellStyleName))
                cellStyle = (Style)Resources[cellStyleName];

            if (battleField == null) battleField = new Grid();
            if (battleFieldEnemy == null) battleFieldEnemy = new Grid();
            battleField.RowDefinitions = new RowDefinitionCollection();
            battleFieldEnemy.RowDefinitions = new RowDefinitionCollection();
            battleField.ColumnDefinitions = new ColumnDefinitionCollection();
            battleFieldEnemy.ColumnDefinitions = new ColumnDefinitionCollection();

            for (int i = 0; i < scale; ++i)
            {
                battleField.RowDefinitions.Add(new RowDefinition { Height = cellLength });
                battleFieldEnemy.RowDefinitions.Add(new RowDefinition { Height = cellLength });
                battleField.ColumnDefinitions.Add(new ColumnDefinition { Width = cellLength });
                battleFieldEnemy.ColumnDefinitions.Add(new ColumnDefinition { Width = cellLength });
            }
            for (int i = 0; i < scale; ++i)
                for (int j = 0; j < scale; ++j)
                {
                    var trigger = new EventTrigger { Event = nameof(SizeChanged) };
                    trigger.Actions.Add(new HeightEqualWidthTriggerAction());

                    var binding = new Binding(nameof(viewModel.BattleField),
                                              BindingMode.OneWay,
                                              new CellStateToColorConverter(),
                                              (i, j));
                    var bindingEnemy = new Binding(nameof(viewModel.BattleFieldEnemy),
                                                   BindingMode.OneWay,
                                                   new CellStateToColorConverter(),
                                                   (i, j));

                    var cell = new Button { Style = cellStyle };
                    var cellEnemy = new Button { Style = cellStyle };
                    cell.Triggers.Add(trigger);
                    cell.InputTransparent = true;
                    cell.SetBinding(BackgroundColorProperty, binding);
                    cellEnemy.Triggers.Add(trigger);
                    cellEnemy.SetBinding(BackgroundColorProperty, bindingEnemy);
                    cellEnemy.SetBinding(InputTransparentProperty, nameof(viewModel.InteractionsDisallowed));
                    cellEnemy.Command = viewModel.ClickCellCommand;
                    cellEnemy.CommandParameter = (i, j);

                    battleField.Children.Add(cell, i, j);
                    battleFieldEnemy.Children.Add(cellEnemy, i, j);
                }
        }

        #region Handlers

        void ColorPickerClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button == null || scrollView == null
                || button.BackgroundColor == scrollView.BackgroundColor)
                return;
            if (mainFrame == null || infoFrame == null || buttonsFrame == null)
                return;

            scrollView.BackgroundColor = button.BackgroundColor;

            if (themePalette.ContainsKey(button.BackgroundColor))
                mainFrame.BackgroundColor = infoFrame.BackgroundColor = buttonsFrame.BackgroundColor =
                    themePalette[button.BackgroundColor];
        }

        #endregion
    }
}