using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FindingMirroredElements.Views
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow
    {
        public ReportWindow(string report, string extra = "")
        {
            InitializeComponent();
            Title = App.Title;
            ReportMessage.Text = report;
            if (extra != "")
            {
                ReportMessage.ToolTip = extra;
            }
            Icon = GetImageSourceByBitMapFromResource(Properties.Resources.flip_FILL0_wght400_GRAD0_opsz48, 16);
        }

        private ImageSource GetImageSourceByBitMapFromResource(Bitmap source, int size)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size, size)
            );
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
