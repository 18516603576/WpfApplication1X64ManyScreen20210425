using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.util
{
    public class FrameworkElementUtil
    {

        /*
         * 获取其下的某个名称的控件
         */
        public static FrameworkElement getByName(Canvas canvas, string name)
        {
            FrameworkElement result = null;
            foreach (FrameworkElement child in canvas.Children)
            {
                if (child.Name == name)
                {
                    result = child;
                    break;
                }
            }
            return result;

        }

        /*
         * 获取其下的某个名称的控件
         */
        public static FrameworkElement getByName(Grid grid, string name)
        {
            FrameworkElement result = null;
            foreach (FrameworkElement child in grid.Children)
            {
                if (child.Name == name)
                {
                    result = child;
                    break;
                }
            }
            return result;

        }



        /*
         * 获取其下的某个名称的控MenuItem
         */
        public static MenuItem getByName(ItemCollection items, string name)
        {
            MenuItem result = null;
            foreach (MenuItem child in items)
            {
                if (child.Name == name)
                {
                    result = child;
                    return result;
                }
                else
                {
                    result = getByName(child.Items, name);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        /*
        * 启用所有MenuItem
        */
        public static void enableAllMenuItem(ItemCollection items)
        {

            foreach (MenuItem child in items)
            {

                child.IsEnabled = true;

                enableAllMenuItem(child.Items);


            }

        }






        /*
         * 获取子控件
         * 
         * @param obj 父控件
         * 
         * @param T 要获取控件类型
         * 
         * @param name 要获取的控件名称
         */
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }
    }
}
