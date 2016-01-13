using System;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;


namespace TicketVendingNETMF
{
    public class HighlightingListBoxItem : ListBoxItem
    {
        public Object Tag;

        public HighlightingListBoxItem()
        {
            this.Background = null;
        }


        public HighlightingListBoxItem(UIElement content)
        {
            this.Child = content;
            this.Background = null;
        }


        protected override void OnIsSelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                this.Background = new SolidColorBrush(Colors.Blue);
            }
            else
                this.Background = null;
        }
    }

    public class HighlightingTextListBoxItem : HighlightingListBoxItem
    {
        private readonly Text text;


        public HighlightingTextListBoxItem(Microsoft.SPOT.Font font, string content)
            : base()
        {
            this.text = new Text(font, content);
            this.text.SetMargin(2);
            this.Child = this.text;
        }


        protected override void OnIsSelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                this.Background = new SolidColorBrush(Colors.Blue);
                this.text.ForeColor = Color.White;
            }
            else
            {
                this.Background = null;
                this.text.ForeColor = Color.Black;
            }
        }
    }


    public class HighlightingRoundTextListBoxItem : HighlightingListBoxItem
    {
        private readonly Text text;
        private readonly Ellipse backgroundShape;
        private readonly Color selectionColor;


        public HighlightingRoundTextListBoxItem(Microsoft.SPOT.Font font, string content)
            : base()
        {
            this.selectionColor = Colors.Blue;

            this.backgroundShape = new Ellipse(13, 13);
            this.backgroundShape.Stroke = new Pen(Color.Black, 1);
            this.backgroundShape.HorizontalAlignment = HorizontalAlignment.Center;
            this.backgroundShape.VerticalAlignment = VerticalAlignment.Center;

            this.text = new Text(font, content);
            this.text.HorizontalAlignment = HorizontalAlignment.Center;
            this.text.VerticalAlignment = VerticalAlignment.Center;
            this.text.SetMargin(2);

            Panel panel = new Panel();
            panel.Children.Add(this.backgroundShape);
            panel.Children.Add(this.text);

            this.Child = panel;
        }


        protected override void OnIsSelectedChanged(bool isSelected)
        {
            if (isSelected)
            {
                this.backgroundShape.Fill = new SolidColorBrush(this.selectionColor);
                this.text.ForeColor = Color.White;
            }
            else
            {
                this.backgroundShape.Fill = null;
                this.text.ForeColor = Color.Black;
            }
        }
    }


    public class StripPanel : Panel
    {
        private int[] columnSizes;
        private Orientation orientation;

        public StripPanel(Orientation orientation, int[] columnSizes)
        {
            if (columnSizes == null)
            {
                throw new ArgumentNullException("columnSizes");
            }
            if (columnSizes.Length == 0)
            {
                throw new ArgumentException("At least one column is required", "columnSizes");
            }
            for (int i = 0; i < columnSizes.Length; i++)
            {
                if (columnSizes[i] <= 0)
                {
                    throw new ArgumentException("Column proportions must be greater than 0.", "columnSizes");
                }
            }
            this.columnSizes = (int[])columnSizes.Clone();
            this.orientation = orientation;
        }


        protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
        {
            bool horizontal = this.orientation == Orientation.Horizontal;
            long[] childPositions = new long[this.columnSizes.Length + 1];
            long norm = 0;
            for (int i = 0; i < columnSizes.Length; i++)
            {
                childPositions[i] = norm;
                if (horizontal)
                {
                    norm += columnSizes[i] * arrangeWidth;
                }
                else
                {
                    norm += columnSizes[i] * arrangeHeight;
                }
            }
            childPositions[columnSizes.Length] = norm;

            int nChildren = base.Children.Count;
            for (int i = 0; i < nChildren; i++)
            {
                int column = Math.Min(i, this.columnSizes.Length);
 
                UIElement child = base.Children[i];
                if (child.Visibility != Visibility.Collapsed)
                {
                    int childDesiredWidth;
                    int childDesiredHeight;
                    child.GetDesiredSize(out childDesiredWidth, out childDesiredHeight);
                    if (horizontal)
                    {
                        int childPosition = (int)((childPositions[column] * arrangeWidth) / norm);
                        int childSize = (int)((childPositions[column + 1] * arrangeWidth) / norm) - childPosition;
                        child.Arrange(childPosition, 0, childSize, Math.Max(arrangeHeight, childDesiredHeight));
                    }
                    else
                    {
                        int childPosition = (int)((childPositions[column] * arrangeHeight) / norm);
                        int childSize = (int)((childPositions[column + 1] * arrangeHeight) / norm) - childPosition;
                        child.Arrange(0, childPosition, Math.Max(arrangeWidth, childDesiredWidth), childSize);
                    }
                }
            }
        }

        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            base.MeasureOverride(availableWidth, availableHeight, out desiredWidth, out desiredHeight);

            bool horizontal = this.orientation == Orientation.Horizontal;
            if (horizontal)
            {
                desiredWidth = availableWidth;
            }
            else
            {
                desiredHeight = availableHeight;
            }
        }
    }
}
