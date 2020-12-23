using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public enum CosaFare
    {
        Sinsita, Destra, Su, Giu
    }

    public class AI
    {
        public static int Process(Pixel[,] dim)
        {
            List<AreaBitmap> Area = GetPixelArea(dim);

            //TODO: CALCOLA IL NUMERO DEI PIXEL BIANCHI E NERI, FAI LA MEDIA PER LATO E DECIDI DOVE ANDARE

            int xy = 0;

            return xy;
        }

        /// <summary>
        /// Crea le 4 immagini
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        private static List<AreaBitmap> GetPixelArea(Pixel[,] dim)
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
