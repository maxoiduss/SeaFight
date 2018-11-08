using SeaFight.Enums;
using static SeaFight.Helpers.ErrorSignalizationHelper;

namespace SeaFight.Helpers
{
    public class XamlHelperBase : Xamarin.Forms.BindableObject
    {
        public static Xamarin.Forms.BindingMode BindingMode { get; set; }
        //public static Xamarin.Forms.BindableProperty NameProperty;
        //public static Xamarin.Forms.BindableProperty ValueProperty;
    }

    public class XamlHelper<TValue> : XamlHelperBase, System.IEquatable<XamlHelper<TValue>>
        where TValue : struct//, System.IEquatable<TValue>
                                                                  
    {
        public System.Boolean IsInert;
        public static readonly Xamarin.Forms.BindableProperty NameProperty;
        public static readonly Xamarin.Forms.BindableProperty ValueProperty;

        public System.String Name
        { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }

        public TValue Value
        { get => (TValue)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        static XamlHelper()
        {
            NameProperty = Xamarin.Forms.BindableProperty.Create(
                nameof(Name),
                typeof(System.String),
                typeof(XamlHelper<TValue>),
                System.String.Empty,
                BindingMode,
                propertyChanging: (b, o, n) =>
                PropertyHandlerHelper<TValue>.HandlePropertyChanging(o, n, nameof(Name),
                                                                     ((XamlHelper<TValue>)b).IsInert));
            ValueProperty = Xamarin.Forms.BindableProperty.Create(
                nameof(Value),
                typeof(TValue),
                typeof(XamlHelper<TValue>),
                default(TValue),
                BindingMode,
                propertyChanging: (b, o, n) =>
                PropertyHandlerHelper<TValue>.HandlePropertyChanging(o, n, nameof(Value),
                                                                     ((XamlHelper<TValue>)b).IsInert));
        }
        public XamlHelper() { Value = default(TValue); Name = string.Empty; }
        public XamlHelper(TValue value) { Value = value; Name = string.Empty; }
        public XamlHelper(TValue value, string name) { Value = value; Name = name; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as XamlHelper<TValue>;
            if (other is null)
            {
                ErrorDetected($"Error in {nameof(XamlHelper<TValue>)}'s {nameof(Equals)}: {nameof(obj)}", ReasonType.NullError);
                return false;
            }

            return Equals(other);
        }

        public bool Equals(XamlHelper<TValue> other)
        {
            if (other is null)
            {
                ErrorDetected($"Error in {nameof(XamlHelper<TValue>)}'s {nameof(Equals)}: {nameof(other)}", ReasonType.NullError);
                return false;
            }

            return Name.Equals(other.Name) && Value.Equals(other.Value);
        }

        public static implicit operator TValue(XamlHelper<TValue> self)
        {
            return !(self is null) ? self.Value : new TValue();
        }

        public static implicit operator XamlHelper<TValue>(TValue value)
        {
            return new XamlHelper<TValue>(value, string.Empty);
        }

        public static bool operator == (XamlHelper<TValue> obj1, XamlHelper<TValue> obj2)
        {
            if (obj1 is null && obj2 is null)
                return true;

            return obj1.Equals(obj2);
        }

        public static bool operator != (XamlHelper<TValue> obj1, XamlHelper<TValue> obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
