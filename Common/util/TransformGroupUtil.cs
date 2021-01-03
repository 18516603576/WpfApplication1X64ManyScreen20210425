using System.Windows.Media;

namespace Common.util
{
    public class TransformGroupUtil
    {
        /*
         * 获取位移对象
         */
        public static TranslateTransform GetTranslateTransform(TransformGroup group)
        {
            foreach (Transform child in group.Children)
            {
                if (child.GetType().Name == "TranslateTransform")
                {
                    return (TranslateTransform)child;
                }
            }

            TranslateTransform translateTransform = new TranslateTransform();
            group.Children.Add(translateTransform);

            return translateTransform;

        }


        /*
         * 获取平移对象
         */
        public static RotateTransform GetRotateTransform(TransformGroup group)
        {
            foreach (Transform child in group.Children)
            {
                if (child.GetType().Name == "RotateTransform")
                {
                    return (RotateTransform)child;
                }
            }

            RotateTransform rotateTransform = new RotateTransform();
            group.Children.Add(rotateTransform);

            return rotateTransform;
        }
        /*
         * 获取缩放对象
         */
        public static ScaleTransform GetScaleTransform(TransformGroup group)
        {
            foreach (Transform child in group.Children)
            {
                if (child.GetType().Name == "ScaleTransform")
                {
                    return (ScaleTransform)child;
                }
            }

            ScaleTransform scaleTransform = new ScaleTransform();
            group.Children.Add(scaleTransform);

            return scaleTransform;
        }

        /*
        * 获取倾斜对象
        */
        public static SkewTransform GetSkewTransform(TransformGroup group)
        {
            foreach (Transform child in group.Children)
            {
                if (child.GetType().Name == "SkewTransform")
                {
                    return (SkewTransform)child;
                }
            }

            SkewTransform skewTransform = new SkewTransform();
            group.Children.Add(skewTransform);

            return skewTransform;
        }
    }
}
