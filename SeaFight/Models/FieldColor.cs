using System.Collections.Generic;
using Xamarin.Forms;
using static Xamarin.Forms.BindingMode;

using SeaFight.Extensions;
using SeaFight.Helpers;

namespace SeaFight.Models
{
    public class FieldColor : XamlHelper<Color>
    {
        static FieldColor() { PropertyHandlerHelper<Color>.DefaultValueList.Add(default(Color)); }
        public FieldColor() : base() { BindingMode = OneTime; }
        public FieldColor(Color value) : base(value) { BindingMode = OneTime; }
        public FieldColor(Color value, string name) : base(value, name) { BindingMode = OneTime; }

        #region FieldColor buffer object logic

        static object locker = new object();
        static FieldColor buffer = new FieldColor { IsInert = true };
        static FieldColor Buffer
        {
            get
            {
                if (buffer is null)
                    buffer = new FieldColor { IsInert = true };
                return buffer;
            }
            set
            {
                lock (locker)
                { buffer = value; }
            }
        }

        public static void HandleFieldColorBuffer(object obj, bool nameChanged)
        {
            Buffer.IsInert = true;

            if (!Buffer.HasDefaultColorOrName(!(Colors.DefaultColor is null)))
            {
                Colors.ResetColor(Buffer); Buffer = null;
            }

            if (nameChanged)
            {
                Buffer.Name = (string)obj;
                Buffer.IsInert = true;
                if (!Buffer.HasDefaultColorOrNull(!(Colors.DefaultColor is null)))
                    Colors.ResetColor(Buffer);
            }
            else
            {
                Buffer.Value = (Color)obj;
                Buffer.IsInert = true;
                if (!Buffer.HasDefaultNameOrNull())
                    Colors.ResetColor(Buffer);
            }
        }

        #endregion
    }
}
