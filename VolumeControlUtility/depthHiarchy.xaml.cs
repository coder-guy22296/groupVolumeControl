using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VolumeControlUtility
{
    /// <summary>
    /// Interaction logic for newUI_mockup.xaml
    /// </summary>
    public partial class DepthHiarchy : UserControl
    {
        
        

        public DepthHiarchy()
        {
            // Theme
            Color thmBackground = new Color();
            thmBackground.R = 64;
            thmBackground.G = 64;
            thmBackground.B = 64;
            thmBackground.A = 100;

            Background.BorderBrush = new SolidColorBrush(thmBackground);

            InitializeComponent();
            
        }
    }
}
