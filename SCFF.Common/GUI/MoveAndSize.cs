// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
//
// SCFF DSF is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCFF DSF is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SCFF DSF.  If not, see <http://www.gnu.org/licenses/>.

/// @file SCFF.Common/GUI/MoveAndSize.cs
/// @copydoc SCFF::Common::GUI::MoveAndSize

namespace SCFF.Common.GUI {

using System.Diagnostics;
using SCFF.Common.Profile;

/// 現在のマウス座標とオフセットを指定してCurrentレイアウト要素を移動・拡大縮小
/// @warning このクラスの中でProfileへの書き込みは行わない
public class MoveAndSize {
  //===================================================================
  // 動作
  //===================================================================

  /// 開始
  public void Start(ILayoutElementView target, HitModes hitMode,
                    RelativePoint mousePoint, SnapGuide snapGuide) {
    this.Target = target;
    this.HitMode = hitMode;
    this.MouseOffset = new RelativeMouseOffset(this.Target, mousePoint);
    this.OriginalLTRB = new RelativeLTRB(this.Target.BoundRelativeLeft,
                                         this.Target.BoundRelativeTop,
                                         this.Target.BoundRelativeRight,
                                         this.Target.BoundRelativeBottom);
    this.SnapGuide = snapGuide;
  }

  /// 処理
  public RelativeLTRB Do(RelativePoint mousePoint, bool keepAspectRatio) {
    Debug.Assert(this.IsRunning);

    // 現在のマウスポイントに合わせて処理を実行
    if (this.HitMode == HitModes.Move) {
      return this.Move(mousePoint);
    } else if (keepAspectRatio &&
               (this.HitMode == HitModes.SizeNE || this.HitMode == HitModes.SizeNW ||
                this.HitMode == HitModes.SizeSE || this.HitMode == HitModes.SizeSW)) {
      return this.SizeWithKeepAspectRatio(mousePoint);
    } else {
      return this.Size(mousePoint);
    }
  }

  /// 終了
  public void End() {
    this.Target = null;
    this.HitMode = HitModes.Neutral;
    this.MouseOffset = null;
    this.OriginalLTRB = null;
    this.SnapGuide = null;
  }

  //===================================================================
  // 移動
  //===================================================================

  /// レイアウト要素を移動する
  public RelativeLTRB Move(RelativePoint mousePoint) {
    var width = this.Target.BoundRelativeWidth;
    var height = this.Target.BoundRelativeHeight;

    // 初期値
    var nextLeft = mousePoint.X - this.MouseOffset.Left;
    var nextTop = mousePoint.Y - this.MouseOffset.Top;
    var nextRight = nextLeft + width;
    var nextBottom = nextTop + height;

    // スナップガイドによる補正(優先度: 左上右下)
    if (this.UseSnapGuide) {
      // Left
      var isLeftSnapped = this.SnapGuide.TryHorizontalSnap(ref nextLeft);
      if (isLeftSnapped) {
        nextRight = nextLeft + width;
      }
      // Top
      var isTopSnapped = this.SnapGuide.TryVerticalSnap(ref nextTop);
      if (isTopSnapped) {
        nextBottom = nextTop + height;
      }
      // Right (LeftにSnapされていたら無視する
      if (!isLeftSnapped) {
        var isRightSnapped = this.SnapGuide.TryHorizontalSnap(ref nextRight);
        if (isRightSnapped) {
          nextLeft = nextRight - width;
        }
      }
      // Bottom (TopにSnapされていたら無視する
      if (!isTopSnapped) {
        var isBottomSnapped = this.SnapGuide.TryVerticalSnap(ref nextBottom);
        if (isBottomSnapped) {
          nextTop = nextBottom - height;
        }
      }
    }

    // 一気に補正
    if (nextLeft < 0.0 && 1.0 < nextRight) {
      nextLeft = 0.0;
      nextRight = 1.0;
      Debug.WriteLine("Width is too wide for move", "MoveAndSize.Move");
    } else if (nextLeft < 0.0) {
      nextLeft = 0.0;
      nextRight = nextLeft + width;
    } else if (nextRight > 1.0) {
      nextRight = 1.0;
      nextLeft = nextRight - width;
    } 
    if (nextTop < 0.0 && 1.0 < nextBottom) {
      nextTop = 0.0;
      nextBottom = 1.0;
      Debug.WriteLine("Height is too tall for move", "MoveAndSize.Move");
    } else if (nextTop < 0.0) {
      nextTop = 0.0;
      nextBottom = nextTop + height;
    } else if (nextBottom > 1.0) {
      nextBottom = 1.0;
      nextTop = nextBottom - height;
    }

    return new RelativeLTRB(nextLeft, nextTop, nextRight, nextBottom);
  }

  //===================================================================
  // 拡大縮小
  //===================================================================

  /// レイアウト要素を比率を維持したまま拡大縮小する
  public RelativeLTRB SizeWithKeepAspectRatio(RelativePoint mousePoint) {
    Debug.Assert(this.HitMode == HitModes.SizeNW || this.HitMode == HitModes.SizeNE ||
                 this.HitMode == HitModes.SizeSW || this.HitMode == HitModes.SizeSE);

    // 初期値
    var nextLeft = this.Target.BoundRelativeLeft;
    var nextTop = this.Target.BoundRelativeTop;
    var nextRight = this.Target.BoundRelativeRight;
    var nextBottom = this.Target.BoundRelativeBottom;

    // Top/Bottom
    switch (this.HitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeNE: {
        // Top
        var tryNextTop = mousePoint.Y - this.MouseOffset.Top;
        if (this.UseSnapGuide) this.SnapGuide.TryVerticalSnap(ref tryNextTop);
        if (tryNextTop < 0.0) tryNextTop = 0.0;
        var topUpperBound = nextBottom - Constants.MinimumBoundRelativeSize;
        if (tryNextTop > topUpperBound) tryNextTop = topUpperBound;
        nextTop = tryNextTop;
        break;
      }
      case HitModes.SizeSW:
      case HitModes.SizeSE: {
        // Bottom
        var tryNextBottom = mousePoint.Y - this.MouseOffset.Bottom;
        if (this.UseSnapGuide) this.SnapGuide.TryVerticalSnap(ref tryNextBottom);
        if (tryNextBottom > 1.0) tryNextBottom = 1.0;
        var bottomLowerBound = nextTop + Constants.MinimumBoundRelativeSize;
        if (tryNextBottom < bottomLowerBound) tryNextBottom = bottomLowerBound;
        nextBottom = tryNextBottom;
        break;
      }
    }

    // Left/Right
    switch (this.HitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeSW: {
        // Left
        var tryNextLeft = nextRight - ((nextBottom - nextTop) * this.OriginalAspectRatio);
        if (this.UseSnapGuide && this.SnapGuide.TryHorizontalSnap(ref tryNextLeft)) {
          if (this.HitMode == HitModes.SizeNW) {
            nextTop = nextBottom - ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          }
        }
        if (tryNextLeft < 0.0) {
          tryNextLeft = 0.0;
          if (this.HitMode == HitModes.SizeNW) {
            nextTop = nextBottom - ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          }
        }
        var leftUpperBound = nextRight - Constants.MinimumBoundRelativeSize;
        if (tryNextLeft > leftUpperBound) tryNextLeft = leftUpperBound;
        nextLeft = tryNextLeft;
        break;
      }
      case HitModes.SizeNE:
      case HitModes.SizeSE: {
        // Right
        var tryNextRight = ((nextBottom - nextTop) * this.OriginalAspectRatio) + nextLeft;
        if (this.UseSnapGuide && this.SnapGuide.TryHorizontalSnap(ref tryNextRight)) {
          if (this.HitMode == HitModes.SizeNE) {
            nextTop = nextBottom - ((tryNextRight - nextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((tryNextRight - nextLeft) / this.OriginalAspectRatio);
          }
        }
        if (tryNextRight > 1.0) {
          tryNextRight = 1.0;
          if (this.HitMode == HitModes.SizeNE) {
            nextTop = nextBottom - ((tryNextRight - nextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((tryNextRight - nextLeft) / this.OriginalAspectRatio);
          }
        }
        var rightLowerBound = nextLeft + Constants.MinimumBoundRelativeSize;
        if (tryNextRight < rightLowerBound) tryNextRight = rightLowerBound;
        nextRight = tryNextRight;
        break;
      }
    }

    return new RelativeLTRB(nextLeft, nextTop, nextRight, nextBottom);
  }

  /// レイアウト要素を拡大縮小する
  public RelativeLTRB Size(RelativePoint mousePoint) {
    Debug.Assert(this.HitMode != HitModes.Move);

    // 初期値
    var nextLeft = this.Target.BoundRelativeLeft;
    var nextTop = this.Target.BoundRelativeTop;
    var nextRight = this.Target.BoundRelativeRight;
    var nextBottom = this.Target.BoundRelativeBottom;

    // Top/Bottom
    switch (this.HitMode) {
      case HitModes.SizeN:
      case HitModes.SizeNW:
      case HitModes.SizeNE: {
        // Top
        var tryNextTop = mousePoint.Y - this.MouseOffset.Top;
        if (this.UseSnapGuide) this.SnapGuide.TryVerticalSnap(ref tryNextTop);
        if (tryNextTop < 0.0) tryNextTop = 0.0;
        var topUpperBound = nextBottom - Constants.MinimumBoundRelativeSize;
        if (tryNextTop > topUpperBound) tryNextTop = topUpperBound;
        nextTop = tryNextTop;
        break;
      }
      case HitModes.SizeS:
      case HitModes.SizeSW:
      case HitModes.SizeSE: {
        // Bottom
        var tryNextBottom = mousePoint.Y - this.MouseOffset.Bottom;
        if (this.UseSnapGuide) this.SnapGuide.TryVerticalSnap(ref tryNextBottom);
        if (tryNextBottom > 1.0) tryNextBottom = 1.0;
        var bottomLowerBound = nextTop + Constants.MinimumBoundRelativeSize;
        if (tryNextBottom < bottomLowerBound) tryNextBottom = bottomLowerBound;
        nextBottom = tryNextBottom;
        break;
      }
    }

    // Left/Right
    switch (this.HitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeW:
      case HitModes.SizeSW: {
        // Left
        var tryNextLeft = mousePoint.X - this.MouseOffset.Left;
        if (this.UseSnapGuide) this.SnapGuide.TryHorizontalSnap(ref tryNextLeft);
        if (tryNextLeft < 0.0) tryNextLeft = 0.0;
        var leftUpperBound = nextRight - Constants.MinimumBoundRelativeSize;
        if (tryNextLeft > leftUpperBound) tryNextLeft = leftUpperBound;
        nextLeft = tryNextLeft;
        break;
      }
      case HitModes.SizeNE:
      case HitModes.SizeE:
      case HitModes.SizeSE: {
        // Right
        var tryNextRight = mousePoint.X - this.MouseOffset.Right;
        if (this.UseSnapGuide) this.SnapGuide.TryHorizontalSnap(ref tryNextRight);
        if (tryNextRight > 1.0) tryNextRight = 1.0;
        var rightLowerBound = nextLeft + Constants.MinimumBoundRelativeSize;
        if (tryNextRight < rightLowerBound) tryNextRight = rightLowerBound;
        nextRight = tryNextRight;
        break;
      }
    }

    return new RelativeLTRB(nextLeft, nextTop, nextRight, nextBottom);
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// 動作中か
  public bool IsRunning { get { return (this.HitMode != HitModes.Neutral); } }

  //-------------------------------------------------------------------

  /// 動作開始時のアスペクト比
  private double OriginalAspectRatio {
    get {
      return (this.OriginalLTRB.Right - this.OriginalLTRB.Left) /
             (this.OriginalLTRB.Bottom - this.OriginalLTRB.Top);
    }
  }
  /// スナップガイドを使用するか
  private bool UseSnapGuide { get { return this.SnapGuide != null; } }

  //-------------------------------------------------------------------

  /// 移動・拡大縮小対象のレイアウト要素
  private ILayoutElementView Target { get; set; }

  /// 動作開始時のヒットテストの結果
  private HitModes HitMode { get; set; }

  /// 動作開始時のマウス座標とLeft/Right/Top/BottomのOffset
  private RelativeMouseOffset MouseOffset { get; set; }

  /// 動作開始時のLTRB
  public RelativeLTRB OriginalLTRB { get; set; }

  /// スナップガイド
  private SnapGuide SnapGuide { get; set; }
}
}   // namespace SCFF.Common.GUI
