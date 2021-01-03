using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormCef.control
{
    public partial class ImageButton : Button
    {
        public ImageButton()
        {

            UseVisualStyleBackColor = true;
            FlatStyle = FlatStyle.Flat;

            ForeColor = Color.Transparent;
            BackColor = Color.Transparent;
            FlatAppearance.BorderSize = 0;//透明-去边线
            FlatAppearance.MouseOverBackColor = Color.Transparent;//透明-鼠标经过
            FlatAppearance.MouseDownBackColor = Color.Transparent;//透明-鼠标按下
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void ImageButton_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 取消捕获焦点后的聚焦框 
        /// </summary>
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }

    }
}
