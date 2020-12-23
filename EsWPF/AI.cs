using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace EsWPF
{
    //TODO: MUOVI IN UN ALTRO FILE
    public class AreaBitmap
    {
        public Pixel[,] px { get; set; }
        public int Neri { get; set; }
        public int Bianchi { get; set; }

        public AreaBitmap(Pixel[,] px, int nNeri, int nBianchi)
        {
            this.px = px;
            Neri = nNeri;
            Bianchi = nBianchi;
        }

        public AreaBitmap() { }
    }

    //LASCIA QUI
    public class MediaObjectCalculator
    {
        public MediaObjectCalculator() { }

        public int[] CalcolaQPixel(AreaBitmap[] area)
        {
            if (area.Length != 2)
                throw new Exception("Lunghezza non valida");

            int nNeri = (area[0].Neri) + (area[1].Neri);
            int nBianchi = (area[0].Bianchi) + (area[1].Bianchi);

            return new int[] { nBianchi, nNeri };
        }
    }

    public enum CosaFare
    {
        Sinsita, Destra, Su, Giu
    }

    public class AI
    {
        public AI() { }

        /// <summary>
        /// Codice AI gen1
        /// </summary>
        public int Process(Pixel[,] dim)
        {
            List<AreaBitmap> Area = GetPixelArea(dim);

            //CALCOLA IL NUMERO DEI PIXEL BIANCHI E NERI, FAI LA MEDIA PER LATO E DECIDI DOVE ANDARE
            
            ///
            /// pos 0 = media quantità bianchi; pos 1 = media quantità neri
            ///

            int[] pixelLAlto;
            int[] pixelLBasso;
            int[] pixelLSinistra;
            int[] pixelLDestra;

            //VARIABILI CHE RAPPRESENTANO I CONSIGLI DEI MOVIMENTI PRODOTTI DAI THREAD SOTTOSTANTI
            bool alto = false;
            bool basso = false;
            bool sinistra = false;
            bool destra = false;   

            //THREADS
            Thread t1 = new Thread(new ThreadStart(CalcolaPixelLatoAlto)); t1.Start();
            Thread t2 = new Thread(new ThreadStart(CalcolaPixelLatoBasso)); t2.Start();
            Thread t3 = new Thread(new ThreadStart(CalcolaPixelLatoSinistra)); t3.Start();
            Thread t4 = new Thread(new ThreadStart(CalcolaPixelLatoDestra)); t4.Start();

            void CalcolaPixelLatoAlto()
            {
                AreaBitmap[] area = { Area[0], Area[2]};

                pixelLAlto = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLAlto[0] > pixelLAlto[1]) //Numero dei bianchi maggiore del numero dei neri
                    alto = false;
                else //Numero neri maggiore dei bianchi
                    alto = true;
            }

            void CalcolaPixelLatoBasso()
            {
                Thread.Sleep(1); //Per la sincronizzazzione
                AreaBitmap[] area = { Area[1], Area[3] };

                pixelLBasso = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLBasso[0] > pixelLBasso[1]) //Numero dei bianchi maggiore del numero dei neri
                    basso = false;
                else //Numero neri maggiore dei bianchi
                    basso = true;
            }

            void CalcolaPixelLatoSinistra()
            {
                Thread.Sleep(2); //Per la sincronizzazzione
                AreaBitmap[] area = { Area[0], Area[1] };

                pixelLSinistra = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLSinistra[0] > pixelLSinistra[1]) //Numero dei bianchi maggiore del numero dei neri
                    sinistra = false;
                else //Numero neri maggiore dei bianchi
                    sinistra = true;
            }

            void CalcolaPixelLatoDestra()
            {
                Thread.Sleep(3); //Per la sincronizzazzione
                AreaBitmap[] area = { Area[2], Area[3] };

                pixelLDestra = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLDestra[0] > pixelLDestra[1]) //Numero dei bianchi maggiore del numero dei neri
                    destra = false;
                else //Numero neri maggiore dei bianchi
                    destra = true;
            }

            t1.Abort(); t2.Abort(); t3.Abort(); t4.Abort();
            t1.Join(); t2.Join(); t3.Join(); t4.Join();

            //Per debug
            System.Diagnostics.Debug.WriteLine("Destra: " + destra + " Sinistra: " + sinistra + " Su: " + alto + " Basso: " + basso);

            //TODO: RAGIONARE SU COME MUOVERE L'IMMAGINE SE TUTTO è VERO

            ///
            /// Ragionamento v.1
            /// 
            /// Se sta solo una variabile a vero mi muovo verso quel lato
            /// Se sono tutti veri mi muovo o seguendo il movimento precende o verso il lato con numero maggiore di pixel neri
            /// Se sono veri in 2 o 3 mi muovo verso il lato con numero maggiore di pixel neri
            ///

            int xy = 0;

            return xy;
        }

        /// <summary>
        /// Crea le 4 immagini
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        private List<AreaBitmap> GetPixelArea(Pixel[,] dim)
        {
            List<AreaBitmap> toReturn = new List<AreaBitmap>();            

            int w = dim.GetLength(0) / 2;
            int h = dim.GetLength(1) / 2;
            Bitmap[] imgarray = new Bitmap[4];
            Bitmap from = new Bitmap(w * 2, h * 2);

            for(int x = 0; x < from.Width; x++)
            {
                for(int y = 0; y < from.Height; y++)
                {
                    from.SetPixel(x, y, dim[x, y].Color);
                }
            }

            for (int x = 0; x < 2; x++)
            {
                ///
                /// 00 == alto-sinistra, 01 == basso-sinistra, 10 == alto-destra e 11 == basso-destra
                ///

                for (int y = 0; y < 2; y++)
                {
                    Pixel[,] pxToAdd = new Pixel[w, h];
                    int n = 0;
                    int b = 0;

                    var index = y * 2 + x;
                    imgarray[index] = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    var graphics = Graphics.FromImage(imgarray[index]);

                    graphics.DrawImage(from, new Rectangle(0, 0, w, h), new Rectangle(x * w, y * h, w, h), GraphicsUnit.Pixel);
                    graphics.Dispose();

                    for(int x1 = 0; x1 < imgarray[index].Width; x1++)
                    {
                        for (int y1 = 0; y1 < imgarray[index].Height; y1++)
                        {
                            pxToAdd[x1, y1] = new Pixel(
                                imgarray[index].GetPixel(x1, y1), x1, y1);

                            Color c = pxToAdd[x1, y1].Color;
                            //System.Diagnostics.Debug.WriteLine(c);

                            if (c.R == Color.White.R && c.G == Color.White.G && c.B == Color.White.B)
                                b++;
                            
                            if (c.R == Color.Black.R && c.G == Color.Black.G && c.B == Color.Black.B)
                                n++;
                        }
                    }

                    //imgarray[index].Save("ss.png");
                    toReturn.Add(new AreaBitmap(pxToAdd, n, b));
                }
            }

            return toReturn;
        }
    }
}
