using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AZVoirImagePage : ContentPage
    {
        public AZVoirImagePage()
        {
            InitializeComponent();
        }
        public AZVoirImagePage(Stream s)
        {
            InitializeComponent();
            ImageSource im_str = ImageSource.FromStream(() => s);
            image.Source = im_str;
            //            Device.BeginInvokeOnMainThread(() => image.Source = im_str);
        }

        public AZVoirImagePage(byte[] contenu)
        {
            InitializeComponent();
            MemoryStream s = new MemoryStream(contenu);
            ImageSource im_str = ImageSource.FromStream(() => s);
            image.Source = im_str;
            //            Device.BeginInvokeOnMainThread(() => image.Source = im_str);
        }

        private async void fin_Clicked(object sender, EventArgs e)
        {
            /*
            if(m_nom_fic.Length>0)
            {
                File.Delete(m_nom_fic);
            }
            */
            await Navigation.PopAsync();
        }
    }
}