using System.Collections;
using System.Collections.Generic;

using SeaFight.Helpers;

namespace SeaFight.Models
{
    public struct Colors : IEnumerable
    {
        public static string ColorChanging { get; set; }
        private static List<FieldColor> All { get; set; } = new List<FieldColor>();

        static Colors()
        {
            DefaultColor = new FieldColor(Xamarin.Forms.Color.Wheat, nameof(DefaultColor));
            PropertyHandlerHelper<Xamarin.Forms.Color>.DefaultValueList.Add(DefaultColor);

            InitializeColorNames();
            InitializeEnumerator();
        }

        public static FieldColor DefaultColor { get; private set; }

        public static FieldColor Idle { get; private set; }

        public static FieldColor IdleOvercovered { get; private set; }

        public static FieldColor ShipIdle { get; private set; }

        public static FieldColor ShipAttacked { get; private set; }

        public static FieldColor ShipOvercovered { get; private set; }

        static void InitializeColorNames()
        {
            Idle = new FieldColor(DefaultColor, nameof(Idle)) { IsInert = true };
            IdleOvercovered = new FieldColor(DefaultColor, nameof(IdleOvercovered)) { IsInert = true };
            ShipIdle = new FieldColor(DefaultColor, nameof(ShipIdle)) { IsInert = true };
            ShipAttacked = new FieldColor(DefaultColor, nameof(ShipAttacked)) { IsInert = true };
            ShipOvercovered = new FieldColor(ShipOvercovered, nameof(ShipOvercovered)) { IsInert = true };
        }

        static void InitializeEnumerator()
        {
            All.Add(Idle);
            All.Add(IdleOvercovered);
            All.Add(ShipIdle);
            All.Add(ShipAttacked);
            All.Add(ShipOvercovered);
        }

        public static void Init()
        {
            ColorChanging = string.Empty;
        }

        public static FieldColor[] GetAll()
        {
            return All.ToArray();
        }

        public static FieldColor GetChangingColor()
        {
            switch (ColorChanging)
            {
                case nameof(Idle): return Idle;
                case nameof(IdleOvercovered): return IdleOvercovered;
                case nameof(ShipIdle): return ShipIdle;
                case nameof(ShipAttacked): return ShipAttacked;
                case nameof(ShipOvercovered): return ShipOvercovered;

                default: return DefaultColor;
            }
        }

        public static void MarkColorToChange(string colorToChange)
        {
            ColorChanging = colorToChange;
        }

        public static void ResetColor(object obj)
        {
            if (!(obj is FieldColor))
            {
                var name = obj as string;
                if (!string.IsNullOrEmpty(name))
                    ColorChanging = name;
                
                return;
            }

            var color = (FieldColor)obj;
            switch (color.Name)
            {
                case nameof(Idle): Idle = color;
                    break;
                case nameof(IdleOvercovered): IdleOvercovered = color;
                    break;
                case nameof(ShipIdle): ShipIdle = color;
                    break;
                case nameof(ShipAttacked): ShipAttacked = color;
                    break;
                case nameof(ShipOvercovered): ShipOvercovered = color;
                    break;
                default: DefaultColor = color;
                    break;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return All.GetEnumerator();
        }
    }
}
