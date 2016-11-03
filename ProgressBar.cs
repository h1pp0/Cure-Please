
namespace CurePlease
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class NewProgressBar : ProgressBar
    {
        public NewProgressBar ()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaintBackground (PaintEventArgs pevent)
        {
            // None... Helps control the flicker.
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            const int inset = 1; // A single inset value to control the sizing of the inner rect.

            using (Image offscreenImage = new Bitmap(this.Width, this.Height))
            {
                using (var offscreen = Graphics.FromImage(offscreenImage))
                {
                    var rect = new Rectangle(0, 0, this.Width, this.Height);

                    if (ProgressBarRenderer.IsSupported)
                        ProgressBarRenderer.DrawHorizontalBar(offscreen, rect);

                    rect.Inflate(new Size(-inset, -inset)); // Deflate inner rect.
                    rect.Width = (int)( rect.Width * ( (double)this.Value / this.Maximum ) );
                    if (rect.Width == 0) rect.Width = 1; // Can't draw rec with width of 0.

                    var brush = new LinearGradientBrush(rect, this.BackColor, this.ForeColor, LinearGradientMode.Vertical);
                    offscreen.FillRectangle(brush, inset, inset, rect.Width, rect.Height);

                    e.Graphics.DrawImage(offscreenImage, 0, 0);
                    offscreenImage.Dispose();
                }
            }
        }
    }

}
// Source: http://stackoverflow.com/questions/778678/how-to-change-the-color-of-progressbar-in-c-sharp-net-3-5