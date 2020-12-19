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
        Thread simulationThread;

        //TODO: DA POPOLARE
        Uri UriGicoatore1 = new Uri("");
        Uri UriGicoatore2 = new Uri("");

        public MainWindow()
        {
            //Thread
            simulationThread = new Thread(new ThreadStart(SimulationLoop));

            InitializeComponent();
        }

        //Momento precedente
        private DateTime prevDateTime;

        //FPS
        public int CurrentFPS { get; private set; }
        public const double FPS_TO_BLOCK = 60.0; //FPS fissi

        private void SimulationLoop()
        {
            double msPrimaDiFrame = 1000.0 / FPS_TO_BLOCK;
            Debug.WriteLine(msPrimaDiFrame);

            prevDateTime = DateTime.Now;

            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                DateTime ora;

                //Loop simulazione
                while (simulationThread.IsAlive)
                {
                    ora = DateTime.Now;

                    //Controlla se è passato un secondo
                    if(ora.Second >= prevDateTime.Second + 1)
                    {
                        //Stampa gli FPS
                        Debug.WriteLine("FPS: " + CurrentFPS);

                        CurrentFPS = 0;
                        prevDateTime = ora;
                    }
                    else
                    {
                        //Controlla se sono passati "msPrimaDiFrame" millisecondi
                        if (ora.Millisecond >= prevDateTime.Millisecond + msPrimaDiFrame)
                        {
                            //Frame


                            CurrentFPS++;
                            prevDateTime = ora;
                        }
                    }
                }
            }));
        }
    }
}
