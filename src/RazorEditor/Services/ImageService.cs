using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace RazorEditor.Services
{
    public class ImageService
    {
        /// <summary>
        /// Creates 64base encoded string for passed text, so it can be embedded into email template preview
        /// </summary>
        public string GetEncodedImage(string text, Color textColor, FontStyle fontStyle)
        {
            using (var measureImage = new Bitmap(1, 1))
            {
                using (Graphics measureGraphic = Graphics.FromImage(measureImage))
                {
                    var drawFont   = new Font("Arial, sans-serif", 10, fontStyle);
                    var stringSize = measureGraphic.MeasureString(text, drawFont);

                    using (var drawImage = new Bitmap((int)Math.Round(stringSize.Width, 0), (int)Math.Round(stringSize.Height, 0) + 6))
                    {
                        using (var drawGraphics = Graphics.FromImage(drawImage))
                        {
                            drawGraphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                            var drawBrush = new SolidBrush(textColor);
                            var stringPonit = new PointF(0, 3);

                            drawGraphics.DrawString(text, drawFont, drawBrush, stringPonit);

                            using (var ms = new MemoryStream())
                            {
                                drawImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                return Convert.ToBase64String(ms.ToArray());
                            }
                        }
                    }
                }

            }
        } 
    }
}