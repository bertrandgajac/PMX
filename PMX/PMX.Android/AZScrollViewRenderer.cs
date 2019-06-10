/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PMX.Droid
{
    class AZScrollViewRenderer
    {
    }
}
*/
using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using PMX;
using Controles;
using PMX.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AZScrollView), typeof(AZScrollViewRenderer))]

namespace PMX.Droid.Renderers
{
    public class AZScrollViewRenderer : ScrollViewRenderer  // <AZScrollView>
    {
        public AZScrollViewRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            base.OnElementPropertyChanged(sender, e);

            if (this.Element == null || this.Control == null)
                return;

            if (e.PropertyName == HorizontalScroll.ScrollingEnabledProperty.PropertyName)
            {

                Control.VerticalFadingEdgeEnabled = Element.ScrollingEnabled;

                Control.HorizontalScrollBarEnabled = Element.ScrollingEnabled;
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _lastPoint = new Point(e.RawX, e.RawY);
                    Element.DebutMouvement();
                    break;
                case MotionEventActions.Move:
                    //                        Element.UpdateGrid(Context.FromPixels(e.RawX - _lastPoint.X), Context.FromPixels(e.RawY - _lastPoint.Y));
                    Element.Mouvement(e.RawX - _lastPoint.X, e.RawY - _lastPoint.Y);
                    _lastPoint = new Point(e.RawX, e.RawY);
                    break;
                case MotionEventActions.ButtonRelease:
                case MotionEventActions.Up:
                    Element.FinMouvement();
                    break;
            }
            return true;
        }
    }
}
