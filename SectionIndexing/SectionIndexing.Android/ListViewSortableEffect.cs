using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using SectionIndexing.Droid;

using Android.Views;
using Android.Widget;

[assembly: ResolutionGroupName("SectionIndexing")]
[assembly: ExportEffect(typeof(ListViewSortableEffect), "ListViewSortableEffect")]
namespace SectionIndexing.Droid
{
    public class ListViewSortableEffect : PlatformEffect
    {
        private DragListAdapter _dragListAdapter = null;

        protected override void OnAttached()
        {
            var element = Element as Xamarin.Forms.ListView;

            if (Control is Android.Widget.ListView listView)
            {
                _dragListAdapter = new DragListAdapter(listView, element);
                listView.Adapter = _dragListAdapter;
                listView.SetOnDragListener(_dragListAdapter);
                listView.OnItemLongClickListener = _dragListAdapter;
            }
        }

        protected override void OnDetached()
        {
            if (Control is Android.Widget.ListView listView)
            {
                listView.Adapter = _dragListAdapter.WrappedAdapter;

                // TODO: Remove the attached listeners
            }
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == Sorting.IsSortableProperty.PropertyName)
            {
                _dragListAdapter.DragDropEnabled = Sorting.GetIsSortable(Element);
            }
        }
    }
}