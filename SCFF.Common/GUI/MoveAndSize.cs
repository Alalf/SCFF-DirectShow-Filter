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

using System;
using System.Diagnostics;
using SCFF.Common.Profile;

/// 現在のマウス座標とオフセットを指定してCurrentレイアウト要素を移動・拡大縮小
/// @warning このクラスの中でProfileへの書き込みは行わない
public class MoveAndSize {
  //===================================================================
  // 定数
  //===================================================================

  /// 100ミリ秒間隔でコントロールに更新を伝える
  private const int frameLength = 100;

  //===================================================================
  // コンストラクタ
  //===================================================================

  public MoveAndSize() {
    this.Clear();
  }

  private void Clear() {
    this.target               = null;
    this.hitMode              = HitModes.Neutral;
    this.mouseOffset          = null;
    this.originalLTRB         = null;
    this.snapGuide            = null;
    this.lastUpdateControl    = 0;

    this.MousePoint           = null;
    this.ShouldUpdateControl  = false;
  }

  //===================================================================
  // 動作
  //===================================================================

  /// 開始
  public void Start(ILayoutElementView target, HitModes hitMode,
                    RelativePoint mousePoint, SnapGuide snapGuide) {
    this.target       = target;
    this.hitMode      = hitMode;
    this.mouseOffset  = new RelativeMouseOffset(target, mousePoint);
    this.originalLTRB = new RelativeLTRB(target.BoundRelativeLeft,
                                         target.BoundRelativeTop,
                                         target.BoundRelativeRight,
                                         target.BoundRelativeBottom);
    this.snapGuide            = snapGuide;
    this.lastUpdateControl    = Environment.TickCount;

    this.MousePoint           = mousePoint;
    this.ShouldUpdateControl  = false;
  }

  /// 処理
  public RelativeLTRB Do(bool keepAspectRatio) {
    Debug.Assert(this.IsRunning);

    var now = Environment.TickCount;
    if (this.lastUpdateControl + MoveAndSize.frameLength < now) {
      // 現在時刻が次回更新時刻を過ぎた場合は更新が必要
      this.ShouldUpdateControl = true;
      this.lastUpdateControl = now;
    }

    // 現在のマウスポイントに合わせて処理を実行
    if (this.hitMode == HitModes.Move) {
      return this.Move();
    } else if (keepAspectRatio &&
               (this.hitMode == HitModes.SizeNE || this.hitMode == HitModes.SizeNW ||
                this.hitMode == HitModes.SizeSE || this.hitMode == HitModes.SizeSW)) {
      return this.SizeWithKeepAspectRatio();
    } else {
      return this.Size();
    }
  }

  /// 終了
  public void End() {
    this.Clear();
  }

  //===================================================================
  // 移動
  //===================================================================

  /// レイアウト要素を移動する
  /// @return 移動後のLTRB
  public RelativeLTRB Move() {
    var width = this.target.BoundRelativeWidth;
    var height = this.target.BoundRelativeHeight;

    // 初期値
    var nextLeft = this.MousePoint.X - this.mouseOffset.Left;
    var nextTop = this.MousePoint.Y - this.mouseOffset.Top;
    var nextRight = nextLeft + width;
    var nextBottom = nextTop + height;

    // スナップガイドによる補正(優先度: 左上右下)
    if (this.UseSnapGuide) {
      // Left
      var isLeftSnapped = this.snapGuide.TryHorizontalSnap(ref nextLeft);
      if (isLeftSnapped) {
        nextRight = nextLeft + width;
      }
      // Top
      var isTopSnapped = this.snapGuide.TryVerticalSnap(ref nextTop);
      if (isTopSnapped) {
        nextBottom = nextTop + height;
      }
      // Right (LeftにSnapされていたら無視する
      if (!isLeftSnapped) {
        var isRightSnapped = this.snapGuide.TryHorizontalSnap(ref nextRight);
        if (isRightSnapped) {
          nextLeft = nextRight - width;
        }
      }
      // Bottom (TopにSnapされていたら無視する
      if (!isTopSnapped) {
        var isBottomSnapped = this.snapGuide.TryVerticalSnap(ref nextBottom);
        if (isBottomSnapped) {
          nextTop = nextBottom - height;
        }
      }
    }

    // 一気に補正
    /// @attention 浮動小数点数の比較
    if (nextLeft < 0.0 && 1.0 < nextRight) {
      nextLeft = 0.0;
      nextRight = 1.0;
      Debug.WriteLine("Width is too wide for move", "MoveAndSize.Move");
    } else if (nextLeft < 0.0) {
      nextLeft = 0.0;
      nextRight = nextLeft + width;
    } else if (1.0 < nextRight) {
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
    } else if (1.0 < nextBottom) {
      nextBottom = 1.0;
      nextTop = nextBottom - height;
    }

    return new RelativeLTRB(nextLeft, nextTop, nextRight, nextBottom);
  }

  //===================================================================
  // 拡大縮小
  //===================================================================

  /// レイアウト要素を比率を維持したまま拡大縮小する
  /// @return 拡大縮小後のLTRB
  public RelativeLTRB SizeWithKeepAspectRatio() {
    Debug.Assert(this.hitMode == HitModes.SizeNW || this.hitMode == HitModes.SizeNE ||
                 this.hitMode == HitModes.SizeSW || this.hitMode == HitModes.SizeSE);

    // 初期値
    var nextLeft = this.target.BoundRelativeLeft;
    var nextTop = this.target.BoundRelativeTop;
    var nextRight = this.target.BoundRelativeRight;
    var nextBottom = this.target.BoundRelativeBottom;

    // Top/Bottom
    /// @attention 浮動小数点数の比較
    switch (this.hitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeNE: {
        // Top
        var tryNextTop = this.MousePoint.Y - this.mouseOffset.Top;
        if (this.UseSnapGuide) this.snapGuide.TryVerticalSnap(ref tryNextTop);
        if (tryNextTop < 0.0) tryNextTop = 0.0;
        var topUpperBound = nextBottom - Constants.MinimumBoundRelativeSize;
        if (topUpperBound < tryNextTop) tryNextTop = topUpperBound;
        nextTop = tryNextTop;
        break;
      }
      case HitModes.SizeSW:
      case HitModes.SizeSE: {
        // Bottom
        var tryNextBottom = this.MousePoint.Y - this.mouseOffset.Bottom;
        if (this.UseSnapGuide) this.snapGuide.TryVerticalSnap(ref tryNextBottom);
        if (1.0 < tryNextBottom) tryNextBottom = 1.0;
        var bottomLowerBound = nextTop + Constants.MinimumBoundRelativeSize;
        if (tryNextBottom < bottomLowerBound) tryNextBottom = bottomLowerBound;
        nextBottom = tryNextBottom;
        break;
      }
    }

    // Left/Right
    /// @attention 浮動小数点数の比較
    switch (this.hitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeSW: {
        // Left
        var tryNextLeft = nextRight - ((nextBottom - nextTop) * this.OriginalAspectRatio);
        if (this.UseSnapGuide && this.snapGuide.TryHorizontalSnap(ref tryNextLeft)) {
          if (this.hitMode == HitModes.SizeNW) {
            nextTop = nextBottom - ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          }
        }
        if (tryNextLeft < 0.0) {
          tryNextLeft = 0.0;
          if (this.hitMode == HitModes.SizeNW) {
            nextTop = nextBottom - ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((nextRight - tryNextLeft) / this.OriginalAspectRatio);
          }
        }
        var leftUpperBound = nextRight - Constants.MinimumBoundRelativeSize;
        if (leftUpperBound < tryNextLeft) tryNextLeft = leftUpperBound;
        nextLeft = tryNextLeft;
        break;
      }
      case HitModes.SizeNE:
      case HitModes.SizeSE: {
        // Right
        var tryNextRight = ((nextBottom - nextTop) * this.OriginalAspectRatio) + nextLeft;
        if (this.UseSnapGuide && this.snapGuide.TryHorizontalSnap(ref tryNextRight)) {
          if (this.hitMode == HitModes.SizeNE) {
            nextTop = nextBottom - ((tryNextRight - nextLeft) / this.OriginalAspectRatio);
          } else {
            nextBottom = nextTop + ((tryNextRight - nextLeft) / this.OriginalAspectRatio);
          }
        }
        if (1.0 < tryNextRight) {
          tryNextRight = 1.0;
          if (this.hitMode == HitModes.SizeNE) {
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
  /// @return 拡大縮小後のLTRB
  public RelativeLTRB Size() {
    Debug.Assert(this.hitMode != HitModes.Move);

    // 初期値
    var nextLeft = this.target.BoundRelativeLeft;
    var nextTop = this.target.BoundRelativeTop;
    var nextRight = this.target.BoundRelativeRight;
    var nextBottom = this.target.BoundRelativeBottom;

    // Top/Bottom
    /// @attention 浮動小数点数の比較
    switch (this.hitMode) {
      case HitModes.SizeN:
      case HitModes.SizeNW:
      case HitModes.SizeNE: {
        // Top
        var tryNextTop = this.MousePoint.Y - this.mouseOffset.Top;
        if (this.UseSnapGuide) this.snapGuide.TryVerticalSnap(ref tryNextTop);
        if (tryNextTop < 0.0) tryNextTop = 0.0;
        var topUpperBound = nextBottom - Constants.MinimumBoundRelativeSize;
        if (topUpperBound < tryNextTop) tryNextTop = topUpperBound;
        nextTop = tryNextTop;
        break;
      }
      case HitModes.SizeS:
      case HitModes.SizeSW:
      case HitModes.SizeSE: {
        // Bottom
        var tryNextBottom = this.MousePoint.Y - this.mouseOffset.Bottom;
        if (this.UseSnapGuide) this.snapGuide.TryVerticalSnap(ref tryNextBottom);
        if (1.0 < tryNextBottom) tryNextBottom = 1.0;
        var bottomLowerBound = nextTop + Constants.MinimumBoundRelativeSize;
        if (tryNextBottom < bottomLowerBound) tryNextBottom = bottomLowerBound;
        nextBottom = tryNextBottom;
        break;
      }
    }

    // Left/Right
    /// @attention 浮動小数点数の比較
    switch (this.hitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeW:
      case HitModes.SizeSW: {
        // Left
        var tryNextLeft = this.MousePoint.X - this.mouseOffset.Left;
        if (this.UseSnapGuide) this.snapGuide.TryHorizontalSnap(ref tryNextLeft);
        if (tryNextLeft < 0.0) tryNextLeft = 0.0;
        var leftUpperBound = nextRight - Constants.MinimumBoundRelativeSize;
        if (leftUpperBound < tryNextLeft) tryNextLeft = leftUpperBound;
        nextLeft = tryNextLeft;
        break;
      }
      case HitModes.SizeNE:
      case HitModes.SizeE:
      case HitModes.SizeSE: {
        // Right
        var tryNextRight = this.MousePoint.X - this.mouseOffset.Right;
        if (this.UseSnapGuide) this.snapGuide.TryHorizontalSnap(ref tryNextRight);
        if (1.0 < tryNextRight) tryNextRight = 1.0;
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
  public bool IsRunning { get { return this.hitMode != HitModes.Neutral; } }

  /// 動作中のマウス座標
  public RelativePoint MousePoint { get; set; }

  /// コントロールを更新するべきか
  public bool ShouldUpdateControl { get; set; }

  //-------------------------------------------------------------------
  // private
  //-------------------------------------------------------------------

  /// 動作開始時のアスペクト比
  private double OriginalAspectRatio {
    get {
      return (this.originalLTRB.Right - this.originalLTRB.Left) /
             (this.originalLTRB.Bottom - this.originalLTRB.Top);
    }
  }
  /// スナップガイドを使用するか
  private bool UseSnapGuide { get { return this.snapGuide != null; } }

  //===================================================================
  // フィールド
  //===================================================================

  /// 移動・拡大縮小対象のレイアウト要素
  private ILayoutElementView target;

  /// 動作開始時のヒットテストの結果
  private HitModes hitMode;

  /// 動作開始時のマウス座標とLeft/Right/Top/BottomのOffset
  private RelativeMouseOffset mouseOffset;

  /// 動作開始時のLTRB
  private RelativeLTRB originalLTRB;

  /// スナップガイド
  private SnapGuide snapGuide;

  /// タイムスタンプ(ミリ秒単位)
  private int lastUpdateControl;
}
}   // namespace SCFF.Common.GUI
