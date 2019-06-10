using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PMX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }
        private async void btn_login_Clicked(object sender, EventArgs e)
        {
            DebutPage p = new DebutPage();
            await Navigation.PushAsync(p);
        }
        private async void btn_loges_Clicked(object sender, EventArgs e)
        {
            LogePage p = new LogePage();
            await Navigation.PushAsync(p);
        }
        private async void btn_prs_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrsPage());
        }
        private async void btn_req_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReqPage());
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (((App)Application.Current).LoginFait)
            {
                btn_loges.IsEnabled = true;
                btn_prs.IsEnabled = true;
                btn_req.IsEnabled = true;
            }
        }
    }
}