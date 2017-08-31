using JetBrains.Annotations;
using Xamarin.Forms;

namespace ATZ.XamarinExtensions
{
    // TODO: This class has been copied from ATZ.XFCells, remove it from there, as the goal is to extract it into this separated library.
    public static class BindableObjectExtensions
    {
        public static T GetValue<T>([NotNull] this BindableObject obj, BindableProperty property, T nullValue)
        {
            var value = obj.GetValue(property);
            if (value == null)
            {
                return nullValue;
            }

            return (T)value;
        }

    }
}
