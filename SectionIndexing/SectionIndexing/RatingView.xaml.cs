using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SectionIndexing
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RatingView : Grid
	{
		public RatingView ()
		{
			InitializeComponent ();
		}
	}
}