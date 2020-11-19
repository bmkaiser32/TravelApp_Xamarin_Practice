using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApp.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                var postTable = conn.Table<Post>().ToList();

                //cat and cat2 do the same thing
                var categories = (from p in postTable
                                  orderby p.CategoryId
                                  select p.CategoryName).Distinct().ToList();

                var categories2 = postTable.OrderBy(p => p.CategoryId).Distinct().ToList();

                Dictionary<string, int> catergoriesCount = new Dictionary<string, int>();
                foreach (var category in categories)
                {

                    // count and count2 do the same thing
                    var count = (from post in postTable
                                 where post.CategoryName == category
                                 select post).ToList().Count;

                    var count2 = postTable.Where(p => p.CategoryName == category).ToList().Count;

                    //Added if else check because category was = to null
                    if (category != null)
                    {
                        catergoriesCount.Add(category, count);
                    }
                    else
                    {
                        catergoriesCount.Add(" *No Category Specified* ", count);
                    }

                    
                }

                categoriesListView.ItemsSource = catergoriesCount;

                postCountLabel.Text = postTable.Count.ToString();

            }
        }
    }
}