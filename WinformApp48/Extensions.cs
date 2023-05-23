using System.Drawing;
using System.Windows.Forms;

namespace WinformApp48
{
    public static class Extensions
    {
        /// <summary>
        /// 실제로 보여지는 영역을 리턴
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetVisibleRect(this Control ctl)
        {
            Control ctlTop = ctl;
            var rectScreen = ctl.RectangleToScreen(ctl.ClientRectangle);
            while (ctl != null)
            {
                rectScreen = Rectangle.Intersect(rectScreen, ctl.RectangleToScreen(ctl.ClientRectangle));
                ctl = ctl.Parent;
            }
            return ctlTop.RectangleToClient(rectScreen);
        }
    }
}
