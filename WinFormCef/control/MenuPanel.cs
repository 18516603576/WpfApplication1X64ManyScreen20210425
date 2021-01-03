using System.Drawing;
using System.Windows.Forms;

namespace WinFormCef.control
{
    public partial class MenuPanel : Panel
    {

        private readonly Color _BorderColor = Color.FromArgb(182, 180, 182);
        public MenuPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重写OnPaint方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics,
                            ClientRectangle,
                            _BorderColor,
                            0,
                            ButtonBorderStyle.Solid,
                            _BorderColor,
                            1,
                            ButtonBorderStyle.Solid,
                           _BorderColor,
                            0,
                            ButtonBorderStyle.Solid,
                            _BorderColor,
                            0,
                            ButtonBorderStyle.Solid);
        }

    }
}
