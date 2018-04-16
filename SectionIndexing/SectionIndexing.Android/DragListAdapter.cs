using Xamarin.Forms;
using Android.Widget;
using Android.Views;
using Android.Database;
using System.Collections;
using Android.Content;
using System.Collections.Generic;
using Android.Animation;

namespace SectionIndexing.Droid
{
    public class DragListAdapter : BaseAdapter, IWrapperListAdapter, Android.Views.View.IOnDragListener, AdapterView.IOnItemLongClickListener
    {
        private IListAdapter _listAdapter;

        private Android.Widget.ListView _listView;

        private Xamarin.Forms.ListView _element;
        private IList<Android.Views.View> _translatedItems;

        public DragListAdapter(Android.Widget.ListView listView, Xamarin.Forms.ListView element)
        {
            _listView = listView;
            // NOTE: careful, the listAdapter might not always be an IWrapperListAdapter
            _listAdapter = ((IWrapperListAdapter)_listView.Adapter).WrappedAdapter;
            _element = element;
        }

        public bool DragDropEnabled { get; set; } = true;

        //... removed for brevity
        #region IWrapperListAdapter Members

        public IListAdapter WrappedAdapter => _listAdapter;

        public override int Count => WrappedAdapter.Count;

        public override bool HasStableIds => WrappedAdapter.HasStableIds;

        public override bool IsEmpty => WrappedAdapter.IsEmpty;

        public override int ViewTypeCount => WrappedAdapter.ViewTypeCount;

        public override bool AreAllItemsEnabled() => WrappedAdapter.AreAllItemsEnabled();

        public override Java.Lang.Object GetItem(int position)
        {
            return WrappedAdapter.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return WrappedAdapter.GetItemId(position);
        }

        public override int GetItemViewType(int position)
        {
            return WrappedAdapter.GetItemViewType(position);
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var view = WrappedAdapter.GetView(position, convertView, parent);
            view.SetOnDragListener(this);
            return view;
        }

        public override bool IsEnabled(int position)
        {
            return WrappedAdapter.IsEnabled(position);
        }

        public bool OnDrag(Android.Views.View v, DragEvent e)
        {
            if (_translatedItems == null)
                _translatedItems = new List<Android.Views.View>();

            switch (e.Action)
            {
                case DragAction.Started:
                    break;
                case DragAction.Entered:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Entered from {v.GetType()}");

                    if (!(v is Android.Widget.ListView))
                    {
                        var dragItem = (DragItem)e.LocalState;

                        var targetPosition = InsertOntoView(v, dragItem);

                        dragItem.Index = targetPosition;

                        // Keep a list of items that has translation so we can reset
                        // them once the drag'n'drop is finished.
                        
                        _translatedItems.Add(v);
                        _listView.Invalidate();
                    }
                    break;
                case DragAction.Location:
                    break;
                case DragAction.Exited:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Entered from {v.GetType()}");

                    if (!(v is Android.Widget.ListView))
                    {
                        var positionEntered = GetListPositionForView(v);

                        System.Diagnostics.Debug.WriteLine($"DragAction.Exited index {positionEntered}");
                    }
                    break;
                case DragAction.Drop:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Drop from {v.GetType()}");
                    break;
                case DragAction.Ended:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Ended from {v.GetType()}");

                    if (!(v is Android.Widget.ListView))
                    {
                        return false;
                    }

                    var mobileItem = (DragItem)e.LocalState;

                    mobileItem.View.Visibility = ViewStates.Visible;

                    foreach (var view in _translatedItems)
                    {
                        view.TranslationY = 0;
                    }

                    _translatedItems.Clear();

                    if (_element.ItemsSource is IOrderable orderable)
                    {
                        orderable.ChangeOrdinal(mobileItem.OriginalIndex, mobileItem.Index);
                    }
                    
                    _translatedItems.Clear();
                    break;
            }

            return true;
        }

        public override void RegisterDataSetObserver(DataSetObserver observer)
        {
            base.RegisterDataSetObserver(observer);
            WrappedAdapter.RegisterDataSetObserver(observer);
        }

        public override void UnregisterDataSetObserver(DataSetObserver observer)
        {
            base.UnregisterDataSetObserver(observer);
            WrappedAdapter.UnregisterDataSetObserver(observer);
        }

        private int GetListPositionForView(Android.Views.View view)
        {
            return NormalizeListPosition(_listView.GetPositionForView(view));
        }

        private int NormalizeListPosition(int position)
        {
            // We do not want to count the headers into the item source index
            return position - _listView.HeaderViewsCount;
        }

        public bool OnItemLongClick(AdapterView parent, Android.Views.View view, int position, long id)
        {
            var selectedItem = ((IList)_element.ItemsSource)[(int)id];

            // Creating drag state
            DragItem dragItem = new DragItem(NormalizeListPosition(position), view, selectedItem);

            // Creating a blank clip data object (we won't depend on this) 
            var data = ClipData.NewPlainText(string.Empty, string.Empty);

            // Creating the default drag shadow for the item (the translucent version of the view)
            // NOTE: Can create a custom view in order to change the dragged item view
            Android.Views.View.DragShadowBuilder shadowBuilder = new Android.Views.View.DragShadowBuilder(view);

            // Setting the original view cell to be invisible
            view.Visibility = ViewStates.Invisible;

            // NOTE: this method is introduced in Android 24, for earlier versions the StartDrag method should be used
            view.StartDragAndDrop(data, shadowBuilder, dragItem, 0);

            return true;
        }

        private int InsertOntoView(Android.Views.View view, DragItem item)
        {
            var positionEntered = GetListPositionForView(view);
            var correctedPosition = positionEntered;

            // If the view already has a translation, we need to adjust the position
            // If the view has a positive translation, that means that the current position
            // is actually one index down then where it started.
            // If the view has a negative translation, that means it actually moved
            // up previous now we will need to move it down.
            if (view.TranslationY > 0)
            {
                correctedPosition += 1;
            }
            else if (view.TranslationY < 0)
            {
                correctedPosition -= 1;
            }

            // If the current index of the dragging item is bigger than the target
            // That means the dragging item is moving up, and the target view should
            // move down, and vice-versa
            var translationCoef = item.Index > correctedPosition ? 1 : -1;

            // We translate the item as much as the height of the drag item (up or down)
            var translationTarget = view.TranslationY + (translationCoef * item.View.Height);

            ObjectAnimator anim = ObjectAnimator.OfFloat(view, "TranslationY", view.TranslationY, translationTarget);
            anim.SetDuration(100);
            anim.Start();

            return correctedPosition;
        }
        #endregion
    }
}