using System;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace ATZ.XamarinExtensions
{
    public static class GridExtensions
    {
        [NotNull]
        public static T Place<T>([NotNull] this Grid inside, [NotNull] T content, int atRow, int andColumn, int acrossRows = 1)
            where T : View
        {
            Grid.SetRow(content, atRow);
            Grid.SetRowSpan(content, acrossRows);
            Grid.SetColumn(content, andColumn);
            // ReSharper disable once PossibleNullReferenceException => Xamarin controls have correct Children collection.
            inside.Children.Add(content);

            return content;
        }
   }
}
