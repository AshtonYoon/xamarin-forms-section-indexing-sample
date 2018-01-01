using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainPage()
        {
            InitializeComponent();

            var source = new List<string>();
            for (int count = 200; count > 0; --count)
            {
                source.Add(RandomString.Generate(10));
            }

            RandomStringsList.ItemsSource = new List<IGrouping<string, string>>(
                source.OrderBy(x => x).GroupBy(x => x[0].ToString().ToUpper()));
        }
    }
}