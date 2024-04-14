using System.Windows.Controls;

namespace WpfStudentChat.Extensions;

public static class ScrollViewerExtensions
{
    public static bool IsAtBottom(this ScrollViewer scrollViewer, float faultTolerance = 10)
    {
        return scrollViewer.VerticalOffset > scrollViewer.ScrollableHeight - faultTolerance;
    }
}
