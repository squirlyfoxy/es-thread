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
    //TODO: MUOVI LE CLASSI IN FILE SEPARATI

    /// <summary>
    /// Classe che descrive un pixel dello screenshot
    /// </summary>
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

        /// <summary>
        /// VELOCITà MOVIMENTO PRIMO SCOOTER
        /// </summary>
        public float VelocitaG1 { get; set; } = 5.3f; 

        /// <summary>
        /// VELOCITà MOVIMENTO SECONDO SCOOTER
        /// </summary>
        public float VelocitaG2 { get; set; } = 4.3f;

        //VPF = Volte Per Secondo

        /// <summary>
        /// Quante volte un thread deve essere eseguito in un secondo
        /// </summary>
        public const double VPS = 60.0;

        //TODO: MEGLIO UNA COSTANTE

        /// <summary>
        /// Raggio screenshot AI
        /// </summary>
        private int ray = 100;

        private void posG1()
        {
            int currentVPS = 0;

            DateTime prevDateTime = DateTime.Now;
            double msPrimaDiFrame = 1000.0 / VPS;
            DateTime now;

            while (t1.IsAlive)
            {
                now = DateTime.Now;

                //MUOVI G1
                //Controlla se è passato un secondo
                if (now.Second >= prevDateTime.Second + 1)
                {
                    //Stampa i VPS
                    Debug.WriteLine("VPS THREAD G1: " + currentVPS);

                    currentVPS = 0;
                    prevDateTime = now;
                } else
                {
                    //Controlla se sono passati "msPrimaDiFrame" millisecondi
                    if (now.Millisecond >= prevDateTime.Millisecond + msPrimaDiFrame)
                    {
                        //Frame
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Bitmap s = Screenshot(G1);

                            Pixel[,] dim = new Pixel[s.Width, s.Height];
                            
                            //Populate px
                            for (int x = 0; x < s.Width; x++)
                            {
                                for (int y = 0; y < s.Height; y++)
                                {
                                    dim[x, y] = new Pixel(s.GetPixel(x, y), x, y);
                                }
                            }

                            s.Save("gg.png");

                            //AI SECTION

                            ///
                            /// Schema funzionamento (teorico):
                            /// Leggo tutta la matrice e individuo le aree che coincidono con il cambiamento di colore dei pixel (bianco == non andare, nero == continua ad andare)
                            /// farò in modo che l'immagine si muova verso la quantità maggiore possibile di pixel neri cambiano, se necessario, la direzione
                            /// Per fare ciò l'immagine verrà divisa in 4 e per ogni lato farò la media della quantità di pixel neri tra i due blocchi da 16px del lato
                            /// Il lato con maggiore quantità di px neri sarà il vincente
                            /// 
                            /// Se lo screenshot è tutto nero continuo con il movimento precedente
                            ///

                            int cosaFare = AI.Process(dim);

                            switch(cosaFare)
                            {

                            }
                        }));

                        currentVPS++;
                        prevDateTime = now;
                    }
                }
            }
        }

        private void posG2()
        {
            int currentVPS = 0;

            DateTime prevDateTime = DateTime.Now;
            double msPrimaDiFrame = 1000.0 / VPS;
            DateTime now;

            while (t2.IsAlive)
            {
                now = DateTime.Now;

                //MUOVI G2
                //Controlla se è passato un secondo
                if (now.Second >= prevDateTime.Second + 1)
                {
                    //Stampa i VPS
                    Debug.WriteLine("VPS THREAD G2: " + currentVPS);

                    currentVPS = 0;
                    prevDateTime = now;
                }
                else
                {
                    //Controlla se sono passati "msPrimaDiFrame" millisecondi
                    if (now.Millisecond >= prevDateTime.Millisecond + msPrimaDiFrame)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Bitmap s = Screenshot(G2);

                            Pixel[,] dim = new Pixel[s.Width, s.Height];

                            //Populate px
                            for (int x = 0; x < s.Width; x++)
                            {
                                for (int y = 0; y < s.Height; y++)
                                {
                                    dim[x, y] = new Pixel(s.GetPixel(x, y), x, y);
                                }
                            }

                            //AI SECTION
                        }));

                        currentVPS++;
                        prevDateTime = now;
                    }
                }

            }
        }

        private void FinishController()
        {
            int currentVPS = 0;

            DateTime prevDateTime = DateTime.Now;
            double msPrimaDiFrame = 1000.0 / VPS;
            DateTime now;

            while (finishController.IsAlive)
            {
                Thread.Sleep(10);

                now = DateTime.Now;

                //Controlla se è passato un secondo
                if (now.Second >= prevDateTime.Second + 1)
                {
                    //Stampa i VPS
                    Debug.WriteLine("VPS THREAD FC: " + currentVPS);

                    currentVPS = 0;
                    prevDateTime = now;
                }
                else
                {
                    //Controlla se sono passati "msPrimaDiFrame" millisecondi
                    if (now.Millisecond >= prevDateTime.Millisecond + msPrimaDiFrame)
                    {
                        //Frame

                        //COLLISION DETECTION

                        ///
                        /// Il seguente codice dovrà capire quando uno dei due scooter taglia il traguardo
                        ///

                        currentVPS++;
                        prevDateTime = now;
                    }
                }
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
