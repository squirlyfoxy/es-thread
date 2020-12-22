using System;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace EsWPF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Thread t1;
        Thread t2;
        Thread finishController;

        private void posG1()
        {
            while(t1.IsAlive)
            {
                //MUOVI G1
            }
        }

        private void posG2()
        {
            while (t2.IsAlive)
            {
                //MUOVI G2
            }
        }

        private void FinishController()
        {
            while (finishController.IsAlive)
            {
                //COLLISION DETECTION
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            t1 = new Thread(new ThreadStart(posG1)); t1.Start();
            t2 = new Thread(new ThreadStart(posG2)); t2.Start();
            finishController = new Thread(new ThreadStart(FinishController)); finishController.Start();
        }
    }
}
