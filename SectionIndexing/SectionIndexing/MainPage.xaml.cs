using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SectionIndexing
{
    public static class RandomString
    {
        private static Random random = new Random();
        public static string Generate(int length)
        {
            return new string(Enumerable.Repeat('a', length).Select(x => (char)(x + random.Next(26))).ToArray());
        }
    }

    public partial class MainPage : ContentPage
    {
        public bool AllowOrdering => true;

        public MainPage()
        {
            InitializeComponent();
            //var source = new List<string>();
            //for (int count = 20000000; count > 0; --count)
            //{
            //    source.Add(RandomString.Generate(10));
            //}

            Sorting.SetIsSortable(RandomStringsList, true);

            Task.Run(() =>
            {
                var source = Enumerable.Range(0, 100).Select(x => x.ToString()).ToArray();

                var templatedSource = new List<IGrouping<string, string>>(
                    source.OrderBy(x => x).GroupBy(x => x[0].ToString().ToUpper()));

                return templatedSource;
            }).ContinueWith(task =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    RandomStringsList.ItemsSource = task.Result;
                });
            });
        }

        private void OnDelete(object sender, EventArgs e)
        {
            var menu = ((MenuItem)sender);
            DisplayAlert("Delete Context Action", menu.CommandParameter + " delete context action", "OK");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Sorting.SetIsEditable(RandomStringsList, !Sorting.GetIsEditable(RandomStringsList));
        }
    }
}