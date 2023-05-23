#define UseBufferedRender_

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinformApp48
{
    public class DateDisplay : Control
    {
        Timer timer;
#if UseBufferedRender
        BufferedRender bufferedRender = null;
#endif

        Font font = SystemFonts.DefaultFont;

        public DateDisplay()
        {
            timer = new Timer();
            timer.Interval = 300;
            timer.Enabled = true;
            timer.Tick += (s, e) =>
            {
                this.Invalidate();
            };

#if UseBufferedRender
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            bufferedRender = new BufferedRender(this);

            bufferedRender?.InitDefault();
#endif
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

#if UseBufferedRender
            bufferedRender?.Dispose();
#endif

            font?.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

#if UseBufferedRender
            bufferedRender?.InitDefault();
#endif

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
#if UseBufferedRender
                bufferedRender?.Render(DrawReal, e.Graphics);
#else
                DrawReal(e.Graphics);
#endif
            }
            catch (Exception Exp)
            {
                Debug.WriteLine(Exp.Message);
            }
        }

        Color GetColorByDay(DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Saturday ? Color.Blue : (dt.DayOfWeek == DayOfWeek.Sunday ? Color.Red : Color.Black);
        }
        private void DrawReal(Graphics g)
        {
            g.SetClip(new Region(this.GetVisibleRect()), CombineMode.Replace);
            g.Clear(Parent.BackColor);

            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now.AddYears(1);

            int iPadding = 4;
            int iDateGap = 5;
            int iTotDays = (dtEnd - dtStart).Days;

            DateTime dtCurrent = DateTime.Now;
            Color dayColor = Color.Black;
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);

            int top1 = 50;
            int top2 = 53;
            int bottom = 60;
            int left = 0;

            g.DrawString($"Now : {DateTime.Now.ToString("mm:ss.ffff")}", font, Brushes.Black, new PointF(1, 1));

            for (int k = 0; k <= iTotDays; k++)
            {
                dtCurrent = dtStart.AddDays(k);
                dayColor = GetColorByDay(dtCurrent);
                left = iPadding + k * iDateGap;

                if (dtCurrent.Day % 5 == 0)
                {
                    p1 = new Point(left, top1);
                    p2 = new Point(left, bottom);
                    g.DrawLine(new Pen(dayColor), p1, p2);

                    g.DrawString(dtCurrent.Day.ToString(), font, new SolidBrush(dayColor), new PointF(left, top1 - 20));
                }
                else
                {
                    p1 = new Point(left, top2);
                    p2 = new Point(left, bottom);
                    g.DrawLine(new Pen(dayColor), p1, p2);
                }

                if (dtCurrent.Day == 1)
                {
                    g.DrawLine(Pens.DarkGreen, new PointF(left, 15), new PointF(left, bottom));
                    g.DrawString($"{dtCurrent.Year}-{dtCurrent.Month:00}", font, Brushes.Black, new PointF(left, 15));
                }
            }
        }
    }
}