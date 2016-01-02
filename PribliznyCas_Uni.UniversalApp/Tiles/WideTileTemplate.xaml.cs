using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PribliznyCas_Uni.Tiles
{
    public sealed partial class WideTileTemplate : UserControl
    {
        public WideTileTemplate()
        {
            this.InitializeComponent();
        }

        public string TimeText
        {
            get { return (string)GetValue(TimeTextProperty); }
            set { SetValue(TimeTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeTextProperty =
            DependencyProperty.Register("TimeText", typeof(string), typeof(WideTileTemplate), new PropertyMetadata("někdy"));

        
    }
}
