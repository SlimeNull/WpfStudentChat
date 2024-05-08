// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StudentChat;

internal class Program
{
    static TextBox textBox = new TextBox { };
    private static Brush brush = Brushes.Red;

    [STAThread]
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var btn = new Button();
        btn.Click += Btn_Click;

        Window win = new();
        textBox.Text = "qwq";
        win.Content = new StackPanel() { Children = { textBox, btn } };

        textBox.MouseDoubleClick += TextBox_MouseDoubleClick;

        win.ShowDialog();


    }

    private static void Btn_Click(object sender, RoutedEventArgs e)
    {
        //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_effectiveValues")]
        //static extern ref EffectiveValueEntry[] Get_effectiveValues(DependencyObject textbox);
        //var arr = Get_effectiveValues(textBox);

        var field = typeof(DependencyObject).GetField("_effectiveValues", (System.Reflection.BindingFlags)(-1));
        var obj = field.GetValue(textBox);
        var arr = Unsafe.As<object, EffectiveValueEntry[]>(ref obj);

        var brush = arr.Where(v => v._value is Brush).First();

        foreach (ref var item in arr.AsSpan())
        {
            if(item._value is Brush)
            {
                Console.WriteLine(item._value);
                item._value = Brushes.Red;
            }
        }

    }

    private static void TextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        textBox.Foreground = Brushes.Red;
        textBox.Foreground = Brushes.Green;
    }

}

struct EffectiveValueEntry
{
    public object _value;

    // Token: 0x04000F88 RID: 3976
    public short _propertyIndex;

    // Token: 0x04000F89 RID: 3977
    public FullValueSource _source;
}

internal enum FullValueSource : short
{
    // Token: 0x04000F8B RID: 3979
    ValueSourceMask = 15,
    // Token: 0x04000F8C RID: 3980
    ModifiersMask = 112,
    // Token: 0x04000F8D RID: 3981
    IsExpression = 16,
    // Token: 0x04000F8E RID: 3982
    IsAnimated = 32,
    // Token: 0x04000F8F RID: 3983
    IsCoerced = 64,
    // Token: 0x04000F90 RID: 3984
    IsPotentiallyADeferredReference = 128,
    // Token: 0x04000F91 RID: 3985
    HasExpressionMarker = 256,
    // Token: 0x04000F92 RID: 3986
    IsCoercedWithCurrentValue = 512
}