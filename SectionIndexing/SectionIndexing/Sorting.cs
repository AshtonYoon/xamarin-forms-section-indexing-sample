using System;
using System.Linq;
using Xamarin.Forms;

namespace SectionIndexing
{
    public static class Sorting
    {

        public static readonly BindableProperty IsSortableProperty =
            BindableProperty.CreateAttached(
                            "IsSortabble", typeof(bool),
                            typeof(ListViewSortableEffect), 
                            false,
                            propertyChanged: OnIsEditableChanged);

        private static void OnIsEditableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as ListView;
            var isEditable = (bool)newValue;
            if (view == null || isEditable)
            {
                return;
            }

            view.Effects.Clear();
        }

        public static bool GetIsSortable(BindableObject view)
        {
            return (bool)view.GetValue(IsSortableProperty);
        }

        public static void SetIsSortable(BindableObject view, bool value)
        {
            view.SetValue(IsSortableProperty, value);
        }

        public static readonly BindableProperty IsEditableProperty =
            BindableProperty.CreateAttached(
                            "IsEditable", typeof(bool),
                            typeof(ListViewSortableEffect), 
                            false,
                            propertyChanged: OnIsSortabbleChanged);

        public static bool GetIsEditable(BindableObject view)
        {
            return (bool)view.GetValue(IsEditableProperty);
        }

        public static void SetIsEditable(BindableObject view, bool value)
        {
            view.SetValue(IsSortableProperty, value);
        }

        static void OnIsSortabbleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as ListView;
            if (view == null)
            {
                return;
            }

            if (!view.Effects.Any(item => item is ListViewSortableEffect))
            {
                //view.Effects.Add(new ListViewSortableEffect());
            }
        }

        public class ListViewSortableEffect : RoutingEffect
        {
            public ListViewSortableEffect() : base("SectionIndexing.ListViewSortableEffect")
            {

            }
        }
    }
}
