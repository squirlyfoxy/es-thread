﻿using System;
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
        public int DiversiDaNero { get; set; }

        public AreaBitmap(Pixel[,] px, int nNeri, int nDiversiNero)
        {
            this.px = px;
            Neri = nNeri;
            DiversiDaNero = nDiversiNero;
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
            int nDiversiNero = (area[0].DiversiDaNero) + (area[1].DiversiDaNero);

            return new int[] { nDiversiNero, nNeri };
        }
    }

    public enum CosaFare
    {
        Sinistra, Destra, Su, Giu
    }

    public class AI
    {
        public AI() { }

        /// <summary>
        /// Codice AI gen1
        /// </summary>
        public float[] Process(Pixel[,] dim, MainWindow mw)
        {
            List<AreaBitmap> Area = GetPixelArea(dim);

            float[] toReturn = new float[2];
            float velBase = mw.VelocitaG1;

            //CALCOLA IL NUMERO DEI PIXEL BIANCHI E NERI, FAI LA MEDIA PER LATO E DECIDI DOVE ANDARE
            
            ///
            /// pos 0 = media quantità bianchi; pos 1 = media quantità neri
            ///

            int[] pixelLAlto = new int[2];
            int[] pixelLBasso = new int[2];
            int[] pixelLSinistra = new int[2];
            int[] pixelLDestra = new int[2];

            int nDiversiNeroTot;
            int pixelTotali = dim.GetLength(0) * dim.GetLength(1);

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

                if (pixelLAlto[0] > pixelLAlto[1]) //Numero dei diversi da nero maggiore del numero dei neri
                    alto = false;
                else //Numero neri maggiore dei bianchi
                    alto = true;
            }

            void CalcolaPixelLatoBasso()
            {
                Thread.Sleep(1); //Per la sincronizzazzione
                AreaBitmap[] area = { Area[1], Area[3] };

                pixelLBasso = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLBasso[0] > pixelLBasso[1]) //Numero dei diversi da nero maggiore del numero dei neri
                    basso = false;
                else //Numero neri maggiore dei bianchi
                    basso = true;
            }

            void CalcolaPixelLatoSinistra()
            {
                Thread.Sleep(2); //Per la sincronizzazzione
                AreaBitmap[] area = { Area[0], Area[1] };

                pixelLSinistra = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLSinistra[0] > pixelLSinistra[1]) //Numero dei diversi da nero maggiore del numero dei neri
                    sinistra = false;
                else //Numero neri maggiore dei bianchi
                    sinistra = true;
            }

            void CalcolaPixelLatoDestra()
            {
                Thread.Sleep(3); //Per la sincronizzazzione
                AreaBitmap[] area = { Area[2], Area[3] };

                pixelLDestra = new MediaObjectCalculator().CalcolaQPixel(area);

                if (pixelLDestra[0] > pixelLDestra[1]) //Numero dei diversi da nero maggiore del numero dei neri
                    destra = false;
                else //Numero neri maggiore dei bianchi
                    destra = true;
            }

            t1.Abort(); t2.Abort(); t3.Abort(); t4.Abort();
            t1.Join(); t2.Join(); t3.Join(); t4.Join();

            //Per debug
            System.Diagnostics.Debug.WriteLine("Destra: " + destra + " Sinistra: " + sinistra + " Su: " + alto + " Basso: " + basso);

            //RAGIONARE SU COME MUOVERE L'IMMAGINE SE TUTTO è VERO
            nDiversiNeroTot = pixelLAlto[0] + pixelLBasso[0] + pixelLDestra[0] + pixelLSinistra[0];

            ///
            /// Ragionamento v.1
            /// 
            /// Se sta solo una variabile a vero mi muovo verso quel lato
            /// Se sono tutti veri mi muovo o seguendo il lato con numero maggiore di pixel neri
            /// Se sono veri in 2 o 3 mi muovo verso il lato con numero maggiore di pixel neri
            /// 
            /// Ragionamento v.2
            /// Maggiore è il numero di pixel diversi dal nero, più la velocità della moto aumenta in modo da dare un effetto "inseguito" e "scappo"
            ///

            velBase =  (nDiversiNeroTot * (velBase) / pixelTotali);
            System.Diagnostics.Debug.WriteLine(velBase);
            toReturn[1] = velBase;

            //TODO: OTTIMIZZARE I TEMPI DEL BLOCCO DI ISTRUZIONI SOTTOSTANTE

            //SERVIRANNO MOLTI IF, NON LA COSA MIGLIORE CHE SI POTREBBE FARE MA PER INIZIARE...

            //E' LA COSA PIù FATTA MALE CHE POTEVO FARE

            //BLOCCO ISTRUZIONI SOLO 1 VARIABILE VERA
            if (alto == true && basso == false && sinistra == false && destra == false)
            {
                toReturn[0] = (int)CosaFare.Su;
            }
            else if (alto == false && basso == true && sinistra == false && destra == false)
            {
                toReturn[0] = (int)CosaFare.Giu;
            }
            else if (alto == false && basso == false && sinistra == true && destra == false)
            {
                toReturn[0] = (int)CosaFare.Sinistra;
            }
            else if (alto == false && basso == false && sinistra == false && destra == true)
            {
                toReturn[0] = (int)CosaFare.Destra;
            }
            else
            {
                int nNeriAlto = pixelLAlto[1];
                int nNeriBasso = pixelLBasso[1];
                int nNeriSinistra = pixelLSinistra[1];
                int nNeriDestra = pixelLDestra[1];

                //SE SONO TUTTI VERI
                if (alto == true && basso == true && sinistra == true && destra == true)
                {
                    //COSA PEGGIORE CHE SI POTEVA INVENTARE
                    if(nNeriAlto >= nNeriBasso && nNeriAlto >= nNeriSinistra && nNeriAlto >= nNeriDestra)
                    {
                        toReturn[0] = (int)CosaFare.Su;
                    } else if(nNeriBasso >= nNeriAlto && nNeriBasso >= nNeriSinistra && nNeriBasso >= nNeriDestra)
                    {
                        toReturn[0] = (int)CosaFare.Giu;
                    } else if(nNeriSinistra >= nNeriAlto && nNeriSinistra >= nNeriBasso && nNeriSinistra >= nNeriDestra)
                    {
                        toReturn[0] = (int)CosaFare.Sinistra;
                    } else if(nNeriDestra >= nNeriAlto && nNeriDestra >= nNeriBasso && nNeriDestra >= nNeriSinistra)
                    {
                        toReturn[0] = (int)CosaFare.Destra;
                    }
                }

                //BLOCCO DI ISTRUZIONI PIù DISPENDIOSO DI TEMPO IN ASSOLUTO
                if(alto == true && basso == true && sinistra == false && destra == false)   //2 VARIABILI A TRUE
                {
                    if(nNeriAlto >= nNeriBasso)
                    {
                        toReturn[0] = (int)CosaFare.Su;
                    }
                    else
                    {
                        toReturn[0] = (int)CosaFare.Giu;
                    }
                }
                else if (alto == true && basso == false && sinistra == true && destra == false)
                {
                    if (nNeriAlto >= nNeriSinistra)
                    {
                        toReturn[0] = (int)CosaFare.Su;
                    }
                    else
                    {
                        toReturn[0] = (int)CosaFare.Sinistra;
                    }
                }
                else if (alto == true && basso == false && sinistra == false && destra == true)
                {
                    if (nNeriAlto >= nNeriDestra)
                    {
                        toReturn[0] = (int)CosaFare.Su;
                    }
                    else
                    {
                        toReturn[0] = (int)CosaFare.Destra;
                    }
                }
                else if (alto == false && basso == true && sinistra == true && destra == false)
                {
                    if (nNeriBasso >= nNeriSinistra)
                    {
                        toReturn[0] = (int)CosaFare.Giu;
                    }
                    else
                    {
                        toReturn[0] = (int)CosaFare.Sinistra;
                    }
                }
                else if (alto == false && basso == true && sinistra == false && destra == true)
                {
                    if (nNeriBasso >= nNeriDestra)
                    {
                        toReturn[0] = (int)CosaFare.Giu;
                    }
                    else
                    {
                        toReturn[0] = (int)CosaFare.Destra;
                    }
                } else if(alto == false && basso == false && sinistra == true && destra == true)
                {
                    if (nNeriSinistra >= nNeriDestra)
                    {
                        toReturn[0] = (int)CosaFare.Sinistra;
                    }
                    else
                    {
                        toReturn[0] = (int)CosaFare.Destra;
                    }
                }
                else
                {
                    //3 VARIABILI A TRUE
                    if(alto == true && basso == true && sinistra == true && destra == false)
                    {
                        if(nNeriAlto >= nNeriBasso && nNeriAlto >= nNeriSinistra)
                        {
                            toReturn[0] = (int)CosaFare.Su;
                        } else if(nNeriBasso >= nNeriAlto && nNeriBasso >= nNeriSinistra)
                        {
                            toReturn[0] = (int)CosaFare.Giu;
                        } else
                        {
                            toReturn[0] = (int)CosaFare.Sinistra;
                        }
                    } else if(alto == true && basso == true && sinistra == false && destra == true)
                    {
                        if (nNeriAlto >= nNeriBasso && nNeriAlto >= nNeriDestra)
                        {
                            toReturn[0] = (int)CosaFare.Su;
                        }
                        else if (nNeriBasso >= nNeriAlto && nNeriBasso >= nNeriDestra)
                        {
                            toReturn[0] = (int)CosaFare.Giu;
                        }
                        else
                        {
                            toReturn[0] = (int)CosaFare.Destra;
                        }
                    } else if(alto == true && basso == false && sinistra == true && destra == true)
                    {
                        if (nNeriAlto >= nNeriSinistra && nNeriAlto >= nNeriDestra)
                        {
                            toReturn[0] = (int)CosaFare.Su;
                        }
                        else if (nNeriSinistra >= nNeriAlto && nNeriSinistra >= nNeriDestra)
                        {
                            toReturn[0] = (int)CosaFare.Sinistra;
                        }
                        else
                        {
                            toReturn[0] = (int)CosaFare.Destra;
                        }
                    } else
                    {
                        if(nNeriBasso >= nNeriSinistra && nNeriBasso >= nNeriDestra)
                        {
                            toReturn[0] = (int)CosaFare.Giu;
                        } else if(nNeriSinistra >= nNeriBasso && nNeriSinistra >= nNeriDestra)
                        {
                            toReturn[0] = (int)CosaFare.Sinistra;
                        } else
                        {
                            toReturn[0] = (int)CosaFare.Destra;
                        }
                    }
                }
            }

            return toReturn;
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

                            if (c.R == Color.Black.R && c.G == Color.Black.G && c.B == Color.Black.B)
                                n++;
                            else
                                if(c.R != 120 && c.G != 120 && c.B != 120) //Escludi il grigio che è il colore della linea di partenza
                                    b++;
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
