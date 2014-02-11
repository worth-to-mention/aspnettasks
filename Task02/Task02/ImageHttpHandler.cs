using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;


namespace Task02
{
    public class ImageHttpHandler : IHttpHandler
    {
        private int imageWidth = 200;
        private int imageHeight = 100;
        private int maxValue = 100000;

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            using(Bitmap bmp = new Bitmap(imageWidth, imageHeight))
            using(Graphics g = Graphics.FromImage(bmp))
            {
                Brush blackBrush = new SolidBrush(Color.Black);
                Brush whiteBrush = new SolidBrush(Color.White);

                g.FillRectangle(blackBrush, g.ClipBounds);

                Font f = new Font("Georgia", 10, FontStyle.Strikeout);

                Random m = new Random();
                Index.randomNumber = m.Next(maxValue);

                double angle = m.NextDouble() * 360;
                angle -= 180;
                angle *= 0.1;

                string tmpStr = Index.randomNumber.ToString();
                SizeF textSize = g.MeasureString(tmpStr, f); 
     

                float vRatio = imageHeight / textSize.Height;
                float hRatio = imageWidth / textSize.Width;
                float ratio = Math.Min(vRatio, hRatio);
                PointF origin = new PointF
                {
                    X = imageWidth / 2f - textSize.Width / 2f * ratio,
                    Y = imageHeight / 2f - textSize.Height / 2f * ratio
                };

                g.TranslateTransform(origin.X, origin.Y);
                g.ScaleTransform(ratio, ratio);
                g.TranslateTransform(textSize.Width / 2, textSize.Height / 2);
                g.RotateTransform((float)angle);
                g.TranslateTransform(-textSize.Width / 2, -textSize.Height / 2);
                

                g.DrawString(tmpStr, f, whiteBrush, 0, 0);
                                
                bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
                
            }
        }
    }
}