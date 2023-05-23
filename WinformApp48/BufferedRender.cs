using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinformApp48
{
    public class BufferedRender : IDisposable
    {
        BufferedGraphicsContext GraphicManager;

        BufferedGraphics ManagedBackBuffer;
        public Control Current { get;private set; }

        BufferedRender() {
            GraphicManager = BufferedGraphicsManager.Current;
        }
        public BufferedRender(Control control) : this()
        {
            Current = control;
        }

        public void InitDefault()
        {
            GraphicManager.MaximumBuffer = new Size(Current.Width + 1, Current.Height + 1);
            ManagedBackBuffer?.Dispose();
            if (Current.ClientSize == Size.Empty)
            {
                ManagedBackBuffer = GraphicManager.Allocate(Current.CreateGraphics(), new Rectangle(Point.Empty, new Size(1, 1)));
            }
            else
            {
                ManagedBackBuffer = GraphicManager.Allocate(Current.CreateGraphics(), Current.ClientRectangle);
            }
        }

        public void Render(Action<Graphics> actDraw, Graphics g)
        {
            ManagedBackBuffer.Graphics.SetClip(new Region(Current.GetVisibleRect()), CombineMode.Replace);
            actDraw?.Invoke(ManagedBackBuffer.Graphics);
            ManagedBackBuffer?.Render(g);
        }

        public void Dispose()
        {
            ManagedBackBuffer?.Dispose();
        }
    }
}
