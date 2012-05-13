using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace scff_app.Commons.Views.Forms
{
    public class DragMover : IDisposable
    {
        /// <remarks>
        /// 座標系がレイアウトフォームとプレビュー領域に分散しないように注意する
        /// マウスのlocationはプレビュー領域の座標系なのでGetTargetsLocationメソッドで変換する
        /// </remarks>

        /// <summary>リサイズとする領域のサイズ</summary>
        private const int kBorder = 16;
        /// <summary>最小サイズ</summary>
        private const int kMinSize = kBorder * 2;

        private Control target_;
        private bool resizable_;
        private int mode_;
        private Point last_mouse_location_;
        private Rectangle start_target_rect_;
        private Point virtual_location_;
        private Size virtual_size_;

        public DragMover(Control target)
        {
            this.target_ = target;
            target.MouseDown += target_MouseDown;
            target.MouseMove += target_MouseMove;
            target.MouseUp += target_MouseUp;
        }

        #region IDisposable メンバー

        public void Dispose()
        {
            target_.MouseDown -= target_MouseDown;
            target_.MouseMove -= target_MouseMove;
            target_.MouseUp -= target_MouseUp;
        }

        #endregion

        private void target_MouseDown(object sender, MouseEventArgs e)
        {
            var previewsLocation = e.Location;
            mode_ = GetArea(previewsLocation);
            last_mouse_location_ = GetTargetsLocation(previewsLocation);
            start_target_rect_ = new Rectangle(target_.Location, target_.Size);
            virtual_location_ = target_.Location;
            virtual_size_ = target_.Size;
        }

        private void target_MouseMove(object sender, MouseEventArgs e)
        {
            var previewsLocation = e.Location;
            if (mode_ == 0)
            {
                target_.Cursor = AreaToCursor(GetArea(previewsLocation));
            }
            else
            {
                var location = GetTargetsLocation(previewsLocation);
                UpdateVirtualRect(location);

                var rect = GetFixedRect(virtual_location_, virtual_size_, mode_);
                target_.Size = rect.Size;
                target_.Location = rect.Location;
            }
        }

        private void target_MouseUp(object sender, MouseEventArgs e)
        {
            mode_ = 0;
        }

        private void UpdateVirtualRect(Point location)
        {
            int virtualX = virtual_location_.X;
            int virtualY = virtual_location_.Y;
            int virtualWidth = virtual_size_.Width;
            int virtualHeight = virtual_size_.Height;
            // 真ん中
            if (mode_ == 5)
            {
                virtualX += location.X - last_mouse_location_.X;
                virtualY += location.Y - last_mouse_location_.Y;
            }
            else
            {
                // 左
                if (mode_ == 7 || mode_ == 4 || mode_ == 1)
                {
                    virtualX += location.X - last_mouse_location_.X;
                    virtualWidth -= location.X - last_mouse_location_.X;
                }
                // 上
                if (mode_ == 7 || mode_ == 8 || mode_ == 9)
                {
                    virtualY += location.Y - last_mouse_location_.Y;
                    virtualHeight -= location.Y - last_mouse_location_.Y;
                }
                // 下
                if (mode_ == 1 || mode_ == 2 || mode_ == 3)
                {
                    virtualHeight += location.Y - last_mouse_location_.Y;
                }
                // 右
                if (mode_ == 9 || mode_ == 6 || mode_ == 3)
                {
                    virtualWidth += location.X - last_mouse_location_.X;
                }
            }
            virtual_location_ = new Point(virtualX, virtualY);
            virtual_size_ = new Size(virtualWidth, virtualHeight);

            last_mouse_location_ = location;
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
                    if (size.Width < kMinSize)
                    {
                        newSizeWidth = kMinSize;
                        newLocationX = start_target_rect_.X + start_target_rect_.Width - kMinSize;
                    }
                }
                // 上
                if (mode == 7 || mode == 8 || mode == 9)
                {
                    if (size.Height < kMinSize)
                    {
                        newSizeHeight = kMinSize;
                        newLocationY = start_target_rect_.Y + start_target_rect_.Height - kMinSize;
                    }
                }
                // 下
                if (mode == 1 || mode == 2 || mode == 3)
                {
                    if (size.Height < kMinSize)
                    {
                        newSizeHeight = kMinSize;
                    }
                }
                // 右
                if (mode == 9 || mode == 6 || mode == 3)
                {
                    if (size.Width < kMinSize)
                    {
                        newSizeWidth = kMinSize;
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
            var onLeftBorder = previewsLocation.X < kBorder;
            var onTopBorder = previewsLocation.Y < kBorder;
            var onRightBorder = previewsLocation.X > target_.Width - kBorder;
            var onBottomBorder = previewsLocation.Y > target_.Height - kBorder;
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
            return new Point(target_.Left + previewsLocation.X, target_.Top + previewsLocation.Y);
        }
    }
}
