using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Maleficus.CustomControls
{
    /// <summary>
    /// Helper class for captcha images generation.
    /// </summary>
    public static class CaptchaGenerator
    {
        /// <summary>
        /// Generates captcha image.
        /// </summary>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="imageFormat">Format of image.</param>
        /// <param name="captchaText">Captcha text.</param>
        /// <param name="captchaKeyGenerator">Captcha text generator. May be null. In this case default generator is used.</param>
        /// <returns>Stream with captcha image.</returns>
        public static MemoryStream Generate(int width, int height, ImageFormat imageFormat, out string captchaText, Func<string> captchaKeyGenerator)
        {
            MemoryStream buffer = new MemoryStream(width * height * 4);

            Brush backgroundBrush = new SolidBrush(Color.Black);
            Brush foregroundBrush = new SolidBrush(Color.White);
            Font textFont = new Font("Georgia", 10, FontStyle.Strikeout);
            
            Random m = new Random();

            if (captchaKeyGenerator == null)
            {                
                captchaText = m.Next(10000).ToString();
            }
            else
            {
                captchaText = captchaKeyGenerator();
            }            

            double angle = m.NextDouble() * 360;
            angle -= 180;
            angle *= 0.1;

            using(Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                SizeF textSize = g.MeasureString(captchaText, textFont);

                float vRatio = height / textSize.Height;
                float hRatio = width / textSize.Width;
                float ratio = Math.Min(vRatio, hRatio);

                PointF origin = new PointF
                {
                    X = width / 2f - textSize.Width / 2f * ratio,
                    Y = height / 2f - textSize.Height / 2f * ratio
                };

                g.FillRectangle(backgroundBrush, g.ClipBounds);

                g.TranslateTransform(origin.X, origin.Y);
                g.ScaleTransform(ratio, ratio);
                g.TranslateTransform(textSize.Width / 2, textSize.Height / 2);
                g.RotateTransform((float)angle);
                g.TranslateTransform(-textSize.Width / 2, -textSize.Height / 2);

                g.DrawString(captchaText, textFont, foregroundBrush, 0, 0);

                bmp.Save(buffer, imageFormat);
            }

            return buffer;
        }
    }
}