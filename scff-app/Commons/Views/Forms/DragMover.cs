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
        /// <summary>リサイズとする領域のサイズ</summary>
        private const int Border = 16;
        /// <summary>最小サイズ</summary>
        private const int MinSize = Border * 2;

        private Control target;
        private bool resizable;
        private int mode;
        private Point startMouseLocation;
        private Point lastMouseLocation;
        private Rectangle startTargetRect;
        private Point virtualLocation;
        private Size virtualSize;

        public DragMover(Control target, bool resizable)
        {
            this.target = target;
            this.resizable = resizable;
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
            var location = e.Location;
            mode = GetArea(location);
            startMouseLocation = location;
            lastMouseLocation = location;
            startTargetRect = new Rectangle(target.Location, target.Size);
            virtualLocation = target.Location;
            virtualSize = target.Size;
        }

        private void target_MouseMove(object sender, MouseEventArgs e)
        {
            var location = e.Location;
            if (mode == 0)
            {
                target.Cursor = AreaToCursor(GetArea(location));
            }
            else
            {
                UpdateVirtualRect(location);

                // UpdateRealRect()
                // 真ん中
                if (mode == 5)
                {
                    target.Location = virtualLocation;
                }
                else
                {
                    int newLocationX = virtualLocation.X;
                    int newLocationY = virtualLocation.Y;
                    int newSizeWidth = virtualSize.Width;
                    int newSizeHeight = virtualSize.Height;
                    // 左
                    if (mode == 7 || mode == 4 || mode == 1)
                    {
                    }
                    // 上
                    if (mode == 7 || mode == 8 || mode == 9)
                    {
                        // TODO(progre): 実装
                        /*
                        if (virtualSize.Height < MinSize)
                        {
                            newSizeHeight = MinSize;
                            newLocationY = startTargetRect.Y + startTargetRect.Height - MinSize+1;
                        }
                         * */
                    }
                    // 下
                    if (mode == 1 || mode == 2 || mode == 3)
                    {
                        newSizeHeight = virtualSize.Height > MinSize ? virtualSize.Height : MinSize;
                    }
                    // 右
                    if (mode == 9 || mode == 6 || mode == 3)
                    {
                        newSizeWidth = virtualSize.Width > MinSize ? virtualSize.Width : MinSize;
                    }
                    target.Location = new Point(newLocationX, newLocationY);
                    target.Size = new Size(newSizeWidth, newSizeHeight);
                }
                if (false)
                {

                    int newLocationX;
                    int newLocationY;
                    if (virtualSize.Width > MinSize)
                        newLocationX = virtualLocation.X;
                    else
                        newLocationX = startTargetRect.Left + startTargetRect.Width - MinSize;
                    if (virtualSize.Height > MinSize)
                        newLocationY = virtualLocation.Y;
                    else
                        newLocationY = startTargetRect.Top + startTargetRect.Height - MinSize;

                    target.Location = new Point(
                            newLocationX,
                            newLocationY);
                    target.Size = new Size(
                            virtualSize.Width > MinSize ? virtualSize.Width : MinSize,
                            virtualSize.Height > MinSize ? virtualSize.Height : MinSize);
                }
                lastMouseLocation = location;
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
                virtualX += location.X - startMouseLocation.X;
                virtualY += location.Y - startMouseLocation.Y;
            }
            else
            {
                // 左
                if (mode == 7 || mode == 4 || mode == 1)
                {
                    virtualX += location.X - startMouseLocation.X;
                    virtualWidth -= location.X - startMouseLocation.X;
                }
                // 上
                if (mode == 7 || mode == 8 || mode == 9)
                {
                    virtualY += location.Y - startMouseLocation.Y;
                    virtualHeight -= location.Y - startMouseLocation.Y;
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
        }

        /// <summary>
        /// locationがターゲットコントロールの上下左右斜めのどの領域にあるか
        /// </summary>
        private int GetArea(Point location)
        {
            var onLeftBorder = location.X < Border;
            var onTopBorder = location.Y < Border;
            var onRightBorder = location.X > target.Width - Border;
            var onBottomBorder = location.Y > target.Height - Border;
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
    }
}
