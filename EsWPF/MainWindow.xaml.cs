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
using System.Drawing;
using System.Windows.Interop;

namespace EsWPF
{
    public class Pixel
    {
        public System.Drawing.Color Color { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Pixel(System.Drawing.Color color, int x, int y)
        {
            X = x;
            Y = y;
            Color = color;
        }

    }

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

        public float VelocitaG1 { get; set; } = 5.3f; 
        public float VelocitaG2 { get; set; } = 4.3f;

        /// <summary>
        /// Quante volte il thread deve essere eseguito in un secondo
        /// </summary>
        public const int VPS = 60;

        /// <summary>
        /// Raggio screenshot AI
        /// </summary>
        private int ray = 100;

        private void posG1()
        {
            while(t1.IsAlive)
            {
                Thread.Sleep(1);

                //MUOVI G1

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Bitmap s = Screenshot(G1);

                    System.Drawing.Color[,] dim = new System.Drawing.Color[s.Width, s.Height];
                    //Populate px
                    for(int x = 0; x < s.Width; x++)
                    {
                        for (int y = 0; y < s.Height; y++)
                        {
                            dim[x, y] = s.GetPixel(x, y);
                        }
                    }

                    //
                }));
            }
        }

        private void posG2()
        {
            while (t2.IsAlive)
            {
                Thread.Sleep(1);

                //MUOVI G2

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Bitmap s = Screenshot(G2);
                }));

            }
        }

        private void FinishController()
        {
            while (finishController.IsAlive)
            {
                Thread.Sleep(10);

                //COLLISION DETECTION
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            t1 = new Thread(new ThreadStart(posG1)); t1.Start();
            t2 = new Thread(new ThreadStart(posG2)); t2.Start();
            finishController = new Thread(new ThreadStart(FinishController)); finishController.Start();
        }

        private Bitmap Screenshot(System.Windows.Controls.Image img)
        {
            Bitmap screenshot;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)this.Left + (int)img.Margin.Left, (int)this.Top + (int)img.Margin.Top, ray, ray);
            screenshot = new Bitmap(rect.Width, rect.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            //Get the graphics context
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy); ;
            }

            return screenshot;
        }
    }
}
