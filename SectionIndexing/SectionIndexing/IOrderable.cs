using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionIndexing
{
    /// <summary>
    /// Used by bound collections to expose ordering methods/events
    /// </summary>
    public interface IOrderable
    {
        /// <summary>
        /// Event fired when the items in the collection are re-ordered.
        /// </summary>
        event EventHandler OrderChanged;

        /// <summary>
        /// Used to change the item orders in an enumerable
        /// </summary>
        /// 
        /// The old index of the item.
        /// 
        /// 
        /// The new index of the item.
        /// 
        void ChangeOrdinal(int oldIndex, int newIndex);
    }
}
