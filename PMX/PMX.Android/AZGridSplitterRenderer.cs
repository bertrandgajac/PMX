using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using PMX;
using Controles;
using PMX.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AZGridSplitter), typeof(AZGridSplitterRenderer))]

namespace PMX.Droid.Renderers
{
    public class AZGridSplitterRenderer : VisualElementRenderer<AZGridSplitter>
    {
        private Point _lastPoint;
        public AZGridSplitterRenderer(Context context) : base(context)
        {
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case (int)MotionEventActions.Down:
                    {
                        _lastPoint = new Point(e.RawX, e.RawY);
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        //                        Element.UpdateGrid(Context.FromPixels(e.RawX - _lastPoint.X), Context.FromPixels(e.RawY - _lastPoint.Y));
                        Element.UpdateGrid(e.RawX - _lastPoint.X, e.RawY - _lastPoint.Y);
                        _lastPoint = new Point(e.RawX, e.RawY);
                        break;
                    }
            }
            return true;
        }
    }
}
