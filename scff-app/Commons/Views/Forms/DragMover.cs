using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScffApp.Commons.Views.Forms
{
    public class DragMover : IDisposable
    {
        /// <remarks>
        /// 座標系がレイアウトフォームとプレビュー領域に分散しないように注意する
        /// マウスのlocationはプレビュー領域の座標系なのでGetTargetsLocationメソッドで変換する
        /// </remarks>

        /// <summary>リサイズとする領域のサイズ</summary>
        private const int Border = 16;
        /// <summary>最小サイズ</summary>
        private const int MinSize = Border * 2;

        private Control target;
        private bool resizable;
        private int mode;
        private Point lastMouseLocation;
        private Rectangle startTargetRect;
        private Point virtualLocation;
        private Size virtualSize;

        public DragMover(Control target)
        {
            this.target = target;
            target.MouseDown += target_MouseDown;
            target.MouseMove += target_MouseMove;
            target.MouseUp += target_MouseUp;
        }

        #region IDisposable メンバー

        public void Dispose()
        {
            target.MouseDown -= target_MouseDown;
            target.MouseMove -= target_MouseMove;
            target.MouseUp -= target_MouseUp;
        }

        #endregion

        private void target_MouseDown(object sender, MouseEventArgs e)
        {
            var previewsLocation = e.Location;
            mode = GetArea(previewsLocation);
            lastMouseLocation = GetTargetsLocation(previewsLocation);
            startTargetRect = new Rectangle(target.Location, target.Size);
            virtualLocation = target.Location;
            virtualSize = target.Size;
        }

        private void target_MouseMove(object sender, MouseEventArgs e)
        {
            var previewsLocation = e.Location;
            if (mode == 0)
            {
                target.Cursor = AreaToCursor(GetArea(previewsLocation));
            }
            else
            {
                var location = GetTargetsLocation(previewsLocation);
                UpdateVirtualRect(location);

                var rect = GetFixedRect(virtualLocation, virtualSize, mode);
                target.Size = rect.Size;
                target.Location = rect.Location;
            }
        }

        private void target_MouseUp(object sender, MouseEventArgs e)
        {
            mode = 0;
        }

        private void UpdateVirtualRect(Point location)
        {
            int virtualX = virtualLocation.X;
            int virtualY = virtualLocation.Y;
            int virtualWidth = virtualSize.Width;
            int virtualHeight = virtualSize.Height;
            // 真ん中
            if (mode == 5)
            {
                virtualX += location.X - lastMouseLocation.X;
                virtualY += location.Y - lastMouseLocation.Y;
            }
            else
            {
                // 左
                if (mode == 7 || mode == 4 || mode == 1)
                {
                    virtualX += location.X - lastMouseLocation.X;
                    virtualWidth -= location.X - lastMouseLocation.X;
                }
                // 上
                if (mode == 7 || mode == 8 || mode == 9)
                {
                    virtualY += location.Y - lastMouseLocation.Y;
                    virtualHeight -= location.Y - lastMouseLocation.Y;
                }
                // 下
                if (mode == 1 || mode == 2 || mode == 3)
                {
                    virtualHeight += location.Y - lastMouseLocation.Y;
                }
                // 右
                if (mode == 9 || mode == 6 || mode == 3)
                {
                    virtualWidth += location.X - lastMouseLocation.X;
                }
            }
            virtualLocation = new Point(virtualX, virtualY);
            virtualSize = new Size(virtualWidth, virtualHeight);

            lastMouseLocation = location;
        }

        private Rectangle GetFixedRect(Point location, Size size, int mode)
        {
            int newLocationX = location.X;
            int newLocationY = location.Y;
            int newSizeWidth = size.Width;
            int newSizeHeight = size.Height;
            // 真ん中以外
            if (mode != 5)
            {
                // 左
                if (mode == 7 || mode == 4 || mode == 1)
                {
                    if (size.Width < MinSize)
                    {
                        newSizeWidth = MinSize;
                        newLocationX = startTargetRect.X + startTargetRect.Width - MinSize;
                    }
                }
                // 上
                if (mode == 7 || mode == 8 || mode == 9)
                {
                    if (size.Height < MinSize)
                    {
                        newSizeHeight = MinSize;
                        newLocationY = startTargetRect.Y + startTargetRect.Height - MinSize;
                    }
                }
                // 下
                if (mode == 1 || mode == 2 || mode == 3)
                {
                    if (size.Height < MinSize)
                    {
                        newSizeHeight = MinSize;
                    }
                }
                // 右
                if (mode == 9 || mode == 6 || mode == 3)
                {
                    if (size.Width < MinSize)
                    {
                        newSizeWidth = MinSize;
                    }
                }
            }
            return new Rectangle(newLocationX, newLocationY, newSizeWidth, newSizeHeight);
        }

        /// <summary>
        /// locationがターゲットコントロールの上下左右斜めのどの領域にあるか
        /// </summary>
        private int GetArea(Point previewsLocation)
        {
            var onLeftBorder = previewsLocation.X < Border;
            var onTopBorder = previewsLocation.Y < Border;
            var onRightBorder = previewsLocation.X > target.Width - Border;
            var onBottomBorder = previewsLocation.Y > target.Height - Border;
            if (onLeftBorder && onTopBorder)
                return 7;
            if (onTopBorder && onRightBorder)
                return 9;
            if (onLeftBorder && onBottomBorder)
                return 1;
            if (onBottomBorder && onRightBorder)
                return 3;
            if (onLeftBorder)
                return 4;
            if (onTopBorder)
                return 8;
            if (onRightBorder)
                return 6;
            if (onBottomBorder)
                return 2;

            // リサイズ領域ではない場合は移動モード
            return 5;
        }

        private Cursor AreaToCursor(int area)
        {
            switch (area)
            {
                case 7:
                case 3:
                    return Cursors.SizeNWSE;
                case 1:
                case 9:
                    return Cursors.SizeNESW;
                case 8:
                case 2:
                    return Cursors.SizeNS;
                case 4:
                case 6:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.SizeAll;
                default:
                    return Cursors.Default;
            }
        }

        private Point GetTargetsLocation(Point previewsLocation)
        {
            return new Point(target.Left + previewsLocation.X, target.Top + previewsLocation.Y);
        }
    }
}
