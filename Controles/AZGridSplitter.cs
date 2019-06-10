using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Controles
{
    public class AZGridSplitter : AZBoxView // TemplatedView
    {
        public AZEcran m_p;
        public AZBlocDonnees m_bloc;
        //        private double m_x_sv;
        static PropertyInfo RowDefinitionActualHeightProperty;
        static PropertyInfo ColumnDefinitionActualWidthProperty;
        public AZGridSplitter() : base()
        {
            /*
            int largeur_milieu = 100;
            BoxView bv1 = new BoxView() { BackgroundColor = Color.AliceBlue };
            BoxView bv2 = new BoxView() { BackgroundColor = Color.AliceBlue };
            BoxView bv3 = new BoxView() { BackgroundColor = Color.AliceBlue };
            if (IsRowSplitter())
            {
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(largeur_milieu, GridUnitType.Absolute) });
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                Grid.SetColumn(bv1, 0);
                Grid.SetColumn(bv2, 1);
                Grid.SetColumn(bv3, 2);
            }
            else
            {
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(largeur_milieu, GridUnitType.Absolute) });
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetRow(bv1, 0);
                Grid.SetRow(bv2, 1);
                Grid.SetRow(bv3, 2);
            }
            Children.Add(bv1);
            Children.Add(bv2);
            Children.Add(bv3);
            */
        }
        public void DebutMouvement()
        {
            if (m_bloc != null)
            {
                /*
                m_x_sv = m_bloc.sv.ScrollX;
                m_bloc.sv.Orientation = ScrollOrientation.Vertical;
                m_bloc.sv.TranslationX = -m_x_sv;
                m_bloc.sv.IsClippedToBounds = false;
//                m_bloc.sv.IsEnabled = false;
                */
                m_bloc.sv.IsEnabled = false;
                m_bloc.sv.InputTransparent = true;
            }
        }
        public void FinMouvement()
        {
            if (m_bloc != null)
            {
                m_bloc.DeplacerFrontiereVerticale(ClassId, 0, true);
                //                m_bloc.sv.Orientation = ScrollOrientation.Horizontal;
                //m_bloc.sv.IsEnabled = true;
                m_bloc.sv.IsEnabled = true;
                m_bloc.sv.InputTransparent = false;
            }
        }
        public void Mouvement(double dragOffsetX, double dragOffsetY)
        {
            if (m_p != null)
            {
                m_p.AfficherTrace("appel de UpdateGrid: x=" + this.X.ToString() + ", y=" + this.Y.ToString());
            }
            if (Parent as Grid == null)
            {
                return;
            }
            if (IsRowSplitter())
            {
                UpdateRow(dragOffsetY);
            }
            else
            {
                UpdateColumn(dragOffsetX);
            }
        }
        private bool IsRowSplitter()
        {
            return HorizontalOptions.Alignment == LayoutAlignment.Fill;
        }
        private void UpdateRow(double offsetY)
        {
            if (offsetY == 0)
            {
                return;
            }
            var grid = Parent as Grid;
            var row = Grid.GetRow(this);
            int rowCount = grid.RowDefinitions.Count();
            if (rowCount <= 1 ||
                row == 0 ||
                row == rowCount - 1 ||
                row + Grid.GetRowSpan(this) >= rowCount)
            {
                return;
            }
            RowDefinition rowAbove = grid.RowDefinitions[row - 1];
            var actualHeight = GetRowDefinitionActualHeight(rowAbove) + offsetY;
            double max_height = 500.0;
            if (actualHeight < 0)
            {
                actualHeight = 0;
            }
            else if (actualHeight > max_height)
                actualHeight = max_height;
            rowAbove.Height = new GridLength(actualHeight);
        }
        private void UpdateColumn(double offsetX)
        {
            if (offsetX == 0)
            {
                return;
            }
            var grid = Parent as Grid;
            int column = Grid.GetColumn(this);
            int columnCount = grid.ColumnDefinitions.Count();
            if (columnCount <= 1 ||
                column == 0 ||
                column == columnCount - 1 ||
                column + Grid.GetColumnSpan(this) >= columnCount)
            {
                return;
            }
            //            this.AnchorX += offsetX;
            ColumnDefinition columnLeft = grid.ColumnDefinitions[column - 1];
            double actualWidth = GetColumnDefinitionActualWidth(columnLeft) + offsetX;
            if (actualWidth < 0)
            {
                actualWidth = 0;
            }
            columnLeft.Width = new GridLength(actualWidth);
            if (m_bloc != null)
            {
                //                int num_champ = Convert.ToInt32(ClassId);
                //                m_bloc.lc[num_champ].lg_champ_ecran = (int)(actualWidth + 0.5);
                int offset = offsetX > 0.0 ? (int)(0.5 + offsetX) : (int)(-0.5 + offsetX);
                m_bloc.DeplacerFrontiereVerticale(ClassId, offset, false);
                //                m_bloc.sv.Orientation = ScrollOrientation.Horizontal;
            }
        }
        static private double GetRowDefinitionActualHeight(RowDefinition row)
        {
            double actualHeight;
            if (row.Height.IsAbsolute)
            {
                actualHeight = row.Height.Value;
            }
            else
            {
                if (RowDefinitionActualHeightProperty == null)
                {
                    RowDefinitionActualHeightProperty = row.GetType().GetRuntimeProperties().First((p) => p.Name == "ActualHeight");
                }
                actualHeight = (double)RowDefinitionActualHeightProperty.GetValue(row);
            }
            return actualHeight;
        }
        static private double GetColumnDefinitionActualWidth(ColumnDefinition column)
        {
            double actualWidth;
            if (column.Width.IsAbsolute)
            {
                actualWidth = column.Width.Value;
            }
            else
            {
                if (ColumnDefinitionActualWidthProperty == null)
                {
                    ColumnDefinitionActualWidthProperty = column.GetType().GetRuntimeProperties().First((p) => p.Name == "ActualWidth");
                }
                actualWidth = (double)ColumnDefinitionActualWidthProperty.GetValue(column);
            }
            return actualWidth;
        }
    }
}