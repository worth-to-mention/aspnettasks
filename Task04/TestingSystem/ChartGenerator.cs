using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestingSystem
{
    public struct HSVColor
    {
        public double H;
        public double S;
        public double V;
        public int A;

        public RGBColor ToRGB()
        {
            double chroma = S * V;
            double hDash = H / 60.0;
            double x = chroma * (1.0 - Math.Abs((hDash % 2) - 1.0));
            double r = 0.0;
            double g = 0.0;
            double b = 0.0;
            if (hDash < 1.0)
            {
                r = chroma;
                g = x;
            }
            else if (hDash < 2.0)
            {
                r = x;
                g = chroma;
            }
            else if (hDash < 3.0)
            {
                g = chroma;
                b = x;
            }
            else if (hDash < 4)
            {
                g = x;
                b = chroma;
            }
            else if (hDash < 5.0)
            {
                r = x;
                b = chroma;
            }
            else if (hDash <= 6.0)
            {
                r = chroma;
                b = x;
            }
            double min = V - chroma;

            RGBColor rgbColor = new RGBColor();
            rgbColor.R = (int)((min + r) * 255);
            rgbColor.G = (int)((min + g) * 255);
            rgbColor.B = (int)((min + b) * 255);
            rgbColor.A = A;

            return rgbColor;
        }
    }
    public struct RGBColor
    {
        public int R;
        public int G;
        public int B;
        public int A;
    }

    public static class ChartGenerator
    {
        private const double precision = 0.000000001;
        public static MemoryStream CreateChart(int width, int height, List<Tuple<double, string>> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Count == 0)
                throw new ArgumentException("Number of elements can not be zero.", "data");

            int count = data.Count;
            double sum = 0;
            data.Sort((x, y) =>
            {
                double tmp = x.Item1 - y.Item1;
                if (tmp < 0)
                {
                    return -1;
                }
                return tmp < precision ? 0 : 1;
            });
            sum = data.Sum(el => el.Item1);

            int diameter = Math.Min(width, height);
            int markerSize = 10;
            int legendWidth = width - diameter - markerSize;

            SizeF legendSize = new SizeF(legendWidth, height);
            RectangleF legendLayout = new RectangleF(
                new PointF(diameter, 0), legendSize);

            MemoryStream buffer = new MemoryStream(width * height);

            using(Bitmap bmp = new Bitmap(width, height))
            using(Graphics g = Graphics.FromImage(bmp))
            {
                Brush backgroundBrush = new SolidBrush(Color.FromArgb(0x66, 0x66, 0x66));
                Font f = new Font("Georgia", 12);
                

                PointF origin = new PointF
                {
                    X = width / 2f,
                    Y = height / 2f
                };
                float startAngle = 0;
                float deltaAngle = 0;

                HSVColor color = new HSVColor
                {
                    H = 0.0,
                    S = .75,
                    V = .95,
                    A = 255
                };

                float currentLegendHeight = 0;
                foreach(var el in data)
                {
                    deltaAngle = (float)(el.Item1 / sum * 360);

                    RGBColor rgbColor = color.ToRGB();
                    Brush b = new SolidBrush(Color.FromArgb(rgbColor.A, rgbColor.R, rgbColor.G, rgbColor.B));

                    g.FillPie(b, 0, 0, diameter, diameter, startAngle, deltaAngle);

                    SizeF strSize = g.MeasureString(el.Item2, f, legendSize);
                    RectangleF layout = new RectangleF(
                        new PointF(legendLayout.X, currentLegendHeight), strSize);
                    RectangleF markerLayout = new RectangleF(
                        new PointF(width - markerSize, currentLegendHeight)
                        , new SizeF(markerSize, markerSize));

                    g.DrawString(el.Item2, f, backgroundBrush, layout);
                    g.FillRectangle(b, markerLayout);
                                        
                    startAngle += deltaAngle;
                    color.H = startAngle - deltaAngle / 2.0;
                    currentLegendHeight += strSize.Height;
                }
                bmp.Save(buffer, ImageFormat.Png);
            }
            return buffer;
        }
    }
}