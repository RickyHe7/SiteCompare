using StdfLab.Extensions;
using System.Windows.Controls;

namespace SiteCompare
{
    /// <summary>
    /// Interaction logic for SiteErrorView.xaml
    /// </summary>
    public partial class SiteErrorView : UserControl, IPluginPage
    {
        public string ViewKey => "SiteError";

        public string PluginName => "SiteCompare";

        public SiteErrorView()
        {
            InitializeComponent();
        }
    }
}
