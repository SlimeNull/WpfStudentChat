using System.Windows.Documents;
using System.Windows.Media;

namespace WpfStudentChat.Adorners;

public class UnreadAdorner : Adorner
{
    private bool _isShow;

    public bool IsShow
    {
        get => _isShow; set
        {
            Dispatcher.Invoke(InvalidateVisual);
            _isShow = value;
        }
    }

    public UnreadAdorner(UIElement adornedElement) : base(adornedElement)
    {
        
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if(IsShow)
            drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1), new Point(ActualWidth - 5, 5), 3, 3);
    }

}
