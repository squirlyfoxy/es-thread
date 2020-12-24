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
using System.ComponentModel;

namespace EsWPF
{
    //TODO: MUOVI LE CLASSI IN FILE SEPARATI

    ///
    /// Programmer: Leonardo Baldazzi (@squirlyfoxy)
    ///

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
        BackgroundWorker nk;

        //PROBLEMA: LE APPLICAZIONI WPF, DOPO UN'ATTENTA RICERCA, HO SCOPERTO CHE VENGONO BLOCCATE DALLA CLASSE THREAD PERCHè IL THREAD DELLE WINDOW è DI TIPO STAThread, CHE NON PERMETTE L'ESECUZIONE DI PIù THREAD IN PARALLELO DETTA STRETTA E QUINDI NON POSSO ESEGUIRE TUTTI E 4 I THREAD E ASPETTARMI CHE QUESTI INTERAGISCANO FACENDO MUOVERE FISICAMENTE L'IMMAGINE SULLO SCHERMO (DAL PUNT ODI VISTA DEL CODICE LE COORDINATE CAMBIANO MA ALLA VISTA L'IMMAGINE RIMANE FISSA DOV'è) 
        /*
        Thread t1;
        Thread t2;
        Thread finishController;*/

        //COME LO RISOLVO?
        //BACKGROUNDWORKER: https://stackoverflow.com/questions/20816153/form-freezes-during-while-loop
        //                  https://docs.microsoft.com/it-it/dotnet/api/system.componentmodel.backgroundworker?view=net-5.0
        //CI PERMETTE DI ESEGUIRE CODICE SU UN THREAD SEPARATO DA QUELLO CHIAMANTE
        //LA COSA IN CUI DIFFERISCE è CHE IL SEGUENTE USA GLI EVENTI RISPETTO AI METODI DELLA CLASSE THREAD 
        /*
                BackgroundWorker t1;
                BackgroundWorker t2;
                BackgroundWorker finishController;*/

        //IL BACKGROUND WORKER NON FUNZIONA (FORSE NON HO CAPITO COPME FARLO FUNZIONARE IO)

        //PER RISOLVERE SI POTREBBERO USARE I  TIMER INSIEME...
        System.Timers.Timer timer1;
        System.Timers. Timer timer2;
        System.Timers.Timer timer3;

        public MainWindow()
        {
            InitializeComponent();

            timer1 = new System.Timers.Timer(1000.0 / VPS * 10); timer1.Elapsed += timer1Event;
            timer2 = new System.Timers.Timer(1000.0 / VPS * 10); timer2.Elapsed += timer2Event;
            timer3 = new System.Timers.Timer(1000.0 / VPS * 10);
/*
            t2 = new BackgroundWorker(); t2.DoWork += t2Work;
            t1 = new BackgroundWorker(); t1.DoWork += t1Work;
            finishController = new BackgroundWorker(); finishController.DoWork += finishControllerWork;*/
        }

        private void timer2Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            posG2();
        }

        private void timer1Event(object sender, System.Timers.ElapsedEventArgs e)
        {
            posG1();
        }

        //EVENTI DEI BACKGROUND WORKER
/*
        private void finishControllerWork(object sender, DoWorkEventArgs e)
        {
            FinishController();
        }

        private void t1Work(object sender, DoWorkEventArgs e)
        {
            posG1();
        }

        private void t2Work(object sender, DoWorkEventArgs e)
        {
            posG2();
        }*/


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
        {/*
            int currentVPS = 0;

            DateTime prevDateTime = DateTime.Now;
            double msPrimaDiFrame = 1000.0 / VPS;
            DateTime now;*/

            //while (true)
            //{
               /* now = DateTime.Now;

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
                    {*/
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

                            int cosaFare = new AI().Process(dim);
                            Thickness mG1 = G1.Margin;

                            switch (cosaFare)
                            {
                                case (int)CosaFare.Destra:
                                    mG1.Right -= VelocitaG1;
                                    mG1.Left += VelocitaG1;
                                    break;
                                case (int)CosaFare.Sinsita:
                                    mG1.Left -= VelocitaG1;
                                    mG1.Right += VelocitaG1;
                                    break;
                                case (int)CosaFare.Su:
                                    mG1.Top -= VelocitaG1;
                                    mG1.Bottom += VelocitaG1;
                                    break;
                                case (int)CosaFare.Giu:
                                    mG1.Bottom -= VelocitaG1;
                                    mG1.Top += VelocitaG1;
                                    break;
                            }

                            G1.Margin = mG1;
                            Debug.WriteLine(G1.Margin);
                        }));
            /*
                        currentVPS++;
                    } 
                        
                }

                prevDateTime = now;
            }*/
        }

        private void posG2()
        {/*
            int currentVPS = 0;

            DateTime prevDateTime = DateTime.Now;
            double msPrimaDiFrame = 1000.0 / VPS;
            DateTime now;

            //while (t2.IsAlive)
            //{
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
                    {*/
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
                            int cosaFare = new AI().Process(dim);
                            Thickness mG2 = G2.Margin;

                            switch (cosaFare)
                            {
                                case (int)CosaFare.Destra:
                                    mG2.Right -= VelocitaG1;
                                    mG2.Left += VelocitaG1;
                                    break;
                                case (int)CosaFare.Sinsita:
                                    mG2.Left -= VelocitaG1;
                                    mG2.Right += VelocitaG1;
                                    break;
                                case (int)CosaFare.Su:
                                    mG2.Top -= VelocitaG1;
                                    mG2.Bottom += VelocitaG1;
                                    break;
                                case (int)CosaFare.Giu:
                                    mG2.Bottom -= VelocitaG1;
                                    mG2.Top += VelocitaG1;
                                    break;
                            }

                            G2.Margin = mG2;
                            Debug.WriteLine(G2.Margin);
                        }));
            /*
                        currentVPS++;
                    }
                }

                prevDateTime = now;
            //}*/
        }

        private void FinishController()
        {
            int currentVPS = 0;

            DateTime prevDateTime = DateTime.Now;
            double msPrimaDiFrame = 1000.0 / VPS;
            DateTime now;

            //while (finishController.IsAlive)
            //{
                Thread.Sleep(100);

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
                    }

                    prevDateTime = now;
                }
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {/*
            t1.Start();
            t2.Start();
            finishController.Start();*/
            //t1.RunWorkerAsync();
            //t2.RunWorkerAsync();
            //finishController.RunWorkerAsync();

            //t1 = new Thread(new ThreadStart(posG1));
            //t2 = new Thread(new ThreadStart(posG2));

            timer1.Start();
            //timer2.Start();
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {/*
            t1.Start();
            t2.Start();
            finishController.Start();*/
        }
    }
}
