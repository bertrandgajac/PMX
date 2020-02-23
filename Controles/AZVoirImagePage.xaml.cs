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
            InitialiserImage();
        }
        public AZVoirImagePage(Stream s)
        {
            InitializeComponent();
            ImageSource im_str = ImageSource.FromStream(() => s);
            image.Source = im_str;
            //            Device.BeginInvokeOnMainThread(() => image.Source = im_str);
            InitialiserImage();
        }
        public AZVoirImagePage(byte[] contenu)
        {
            InitializeComponent();
            MemoryStream s = new MemoryStream(contenu);
            ImageSource im_str = ImageSource.FromStream(() => s);
            image.Source = im_str;
            //            Device.BeginInvokeOnMainThread(() => image.Source = im_str);
            InitialiserImage();
        }
        private void InitialiserImage()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            image.GestureRecognizers.Add(pinchGesture);
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            image.GestureRecognizers.Add(panGesture);
        }
        double Cadrer(double val, double min, double max)
        {
            return Math.Min(max, Math.Max(val, min));
        }
        double zoom = 1.0;
        double zoom_debut = 1.0;
        double x0 = 0.0;
        double y0 = 0.0;
        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    // Store the current scale factor applied to the wrapped user interface element,  
                    // and zero the components for the center point of the translate transform.  
                    zoom_debut = Content.Scale;
                    Content.AnchorX = 0;
                    Content.AnchorY = 0;
                    break;
                case GestureStatus.Running:
                    // Calculate the scale factor to be applied.  
                    zoom += (e.Scale - 1) * zoom_debut;
                    zoom = Math.Max(1, zoom);

                    // The ScaleOrigin is in relative coordinates to the wrapped user interface element,  
                    // so get the X pixel coordinate.  
                    double renderedX = Content.X + x0;
                    double deltaX = renderedX / Width;
                    double deltaWidth = Width / (Content.Width * zoom_debut);
                    double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                    // The ScaleOrigin is in relative coordinates to the wrapped user interface element,  
                    // so get the Y pixel coordinate.  
                    double renderedY = Content.Y + y0;
                    double deltaY = renderedY / Height;
                    double deltaHeight = Height / (Content.Height * zoom_debut);
                    double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                    // Calculate the transformed element pixel coordinates.  
                    double targetX = x0 - (originX * Content.Width) * (zoom - zoom_debut);
                    double targetY = y0 - (originY * Content.Height) * (zoom - zoom_debut);

                    // Apply translation based on the change in origin.  
                    //                Content.TranslationX = targetX.Clamp(-Content.Width * (zoom - 1), 0);
                    //                Content.TranslationY = targetY.Clamp(-Content.Height * (zoom - 1), 0);
                    Content.TranslationX = Cadrer(targetX, -Content.Width * (zoom - 1), 0);
                    Content.TranslationY = Cadrer(targetY, -Content.Height * (zoom - 1), 0);

                    // Apply scale factor  
                    Content.Scale = zoom;
                    break;
                case GestureStatus.Completed:
                    // Store the translation delta's of the wrapped user interface element.  
                    x0 = Content.TranslationX;
                    y0 = Content.TranslationY;
                    break;
            }
        }
        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    x0 = Content.TranslationX;
                    y0 = Content.TranslationY;
                    break;
                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    //                    Content.TranslationX = Math.Max(Math.Min(0, x0 + e.TotalX), -Math.Abs(Content.Width - Application.Current.MainPage.Width));
                    //                    Content.TranslationY = Math.Max(Math.Min(0, y0 + e.TotalY), -Math.Abs(Content.Height - Application.Current.MainPage.Height));
                    Content.TranslationX += e.TotalX;
                    Content.TranslationY += e.TotalY;
                    break;
                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    x0 = Content.TranslationX;
                    y0 = Content.TranslationY;
                    break;
            }
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