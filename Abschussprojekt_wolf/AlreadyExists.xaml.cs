using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Abschussprojekt_wolf
{
    /// <summary>
    /// Interaktionslogik für AlreadyExists.xaml
    /// </summary>
    public partial class AlreadyExists : Window
    {
        public int yesNo;
        public AlreadyExists()
        {
            InitializeComponent();
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            yesNo = 1;
            Close();
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            yesNo = 0;
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
