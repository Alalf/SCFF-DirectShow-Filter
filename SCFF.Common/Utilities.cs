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

/// @file SCFF.Common/Utilities.cs
/// @copydoc SCFF::Common::Utilities

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using SCFF.Common.Ext;

/// SCFF.Commonモジュール共通で利用する機能
public static class Utilities {
  //===================================================================
  // ファイルパスの短縮
  //===================================================================

  /// 短縮した後の長さ
  private static int CalcShortPathLength(string pathRoot,
      List<string> directoryList, bool shortened, string fileName) {
    var length = pathRoot.Length;
    if (shortened) length += "...\\".Length;
    foreach (var i in directoryList) {
      if (i == string.Empty) continue;
      length += i.Length + 1;   // '\\'.Length = 1
    }
    length += fileName.Length;
    return length;
  }

  /// 短縮パスを文字列に直す
  private static string BuildShortPath(string pathRoot,
      List<string> directoryList, bool shortened, string fileName) {
    var builder = new StringBuilder();
    builder.Append(pathRoot);
    if (shortened) builder.Append("...\\");
    foreach (var i in directoryList) {
      if (i == string.Empty) continue;
      builder.Append(i);
      builder.Append('\\');
    }
    builder.Append(fileName);
    return builder.ToString();
  }

  /// 最大文字数(目安)を指定して短縮パスを得る
  public static string GetShortPath(string path, int length) {
    var fileName = Path.GetFileName(path);
    var pathRoot = Path.GetPathRoot(path);
    var directoryPath = Path.GetDirectoryName(path).Substring(pathRoot.Length);
    var directoryList = new List<string>(directoryPath.Split('\\'));

    // 短縮が必要ない場合はそのまま文字列にして返す
    var currentLength = Utilities.CalcShortPathLength(pathRoot, directoryList, false, fileName);
    if (currentLength <= length) {
      // 正常な結果なので文字列に戻して返す
      return Utilities.BuildShortPath(pathRoot, directoryList, false, fileName);
    }

    // 短縮開始
    do {
      directoryList.RemoveAt(0);
      var shortenedLength = Utilities.CalcShortPathLength(pathRoot, directoryList, true, fileName);
      if (shortenedLength <= length) {
        return Utilities.BuildShortPath(pathRoot, directoryList, true, fileName);
      }
    } while (directoryList.Count > 0);

    return Utilities.BuildShortPath(pathRoot, directoryList, true, fileName);
  }

  //===================================================================
  // ファイル入出力用の機能
  //===================================================================

  /// Optionの読み込み・保存先/Profileのデフォルト出力先を取得する
  /// @todo(me) 実装。ただし、書き込み可能かどうかの判断が可能かはわからない
  public static string GetDefaultFilePath {
    get {
      // 末尾が\\で終わること
      var applicationPath = Assembly.GetEntryAssembly().Location;
      return Path.GetDirectoryName(applicationPath) + @"\";
    }
  }

  /// INIファイル(セクション機能は無し)読み込み
  public static bool LoadDictionaryFromINIFile(string path,
      out Dictionary<string,string> labelToRawData) {
    labelToRawData = null;

    // テキストファイルの読み込み
    var lines = new List<string>();
    try {
      using (var reader = new StreamReader(path)) {
        while (!reader.EndOfStream) {
          lines.Add(reader.ReadLine());
        }
      }
    } catch (Exception) {
      Debug.WriteLine("Cannot read file", "LoadDictionaryFromINIFile");
      return false;
    }

    // ディクショナリの作成
    labelToRawData = new Dictionary<string,string>();

    // 読み込んだデータを*=*でSplit
    var separator = new char[1] {'='};
    foreach (var line in lines) {
      if (line.Length == 0 || line[0] == ';' || line[0] == '[') {
        // 空行、コメント行、セクション記述行は読み飛ばす
        continue;
      }

      var splitIndex = line.IndexOf('=');
      if (splitIndex == -1) {
        // '='が見つからなければ読みとばす
        continue;
      } else if (splitIndex == line.Length - 1) {
        // 空文字列なので読み飛ばす
        continue;
      }
      var label = line.Substring(0, splitIndex).Trim();
      var rawData = line.Substring(splitIndex+1);
      labelToRawData.Add(label, rawData);
    }

    return true;;
  }

  /// ディクショナリからintを読み込む
  public static bool TryGetInt(this Dictionary<string,string> labelToRawData,
                               string label, out int parsedData) {
    parsedData = default(int);
    if (!labelToRawData.ContainsKey(label)) return false;
    return int.TryParse(labelToRawData[label], out parsedData);
  }

  /// ディクショナリからdoubleを読み込む
  public static bool TryGetDouble(this Dictionary<string,string> labelToRawData,
                                  string label, out double parsedData) {
    parsedData = default(double);
    if (!labelToRawData.ContainsKey(label)) return false;
    return double.TryParse(labelToRawData[label], out parsedData);
  }

  /// ディクショナリからfloatを読み込む
  public static bool TryGetFloat(this Dictionary<string,string> labelToRawData,
                                 string label, out float parsedData) {
    parsedData = default(float);
    if (!labelToRawData.ContainsKey(label)) return false;
    return float.TryParse(labelToRawData[label], out parsedData);
  }

  /// ディクショナリからboolを読み込む
  public static bool TryGetBool(this Dictionary<string,string> labelToRawData,
                                string label, out bool parsedData) {
    parsedData = default(bool);
    if (!labelToRawData.ContainsKey(label)) return false;
    return bool.TryParse(labelToRawData[label], out parsedData);
  }

  /// ディクショナリからUIntPtrを読み込む
  public static bool TryGetUIntPtr(this Dictionary<string,string> labelToRawData,
                                   string label, out UIntPtr parsedData) {
    parsedData = default(UIntPtr);
    if (!labelToRawData.ContainsKey(label)) return false;
    UInt64 uint64Data;
    if (UInt64.TryParse(labelToRawData[label], out uint64Data)) {
      parsedData = (UIntPtr)uint64Data;
      return true;
    }
    return false;
  }

  /// ディクショナリからEnum<T>を読み込む
  public static bool TryGetEnum<TEnum>(this Dictionary<string,string> labelToRawData,
                                       string label, out TEnum parsedData) where TEnum : struct { 
    parsedData = default(TEnum);
    Debug.Assert(typeof(TEnum).IsEnum);
    if (!labelToRawData.ContainsKey(label)) return false;
    return Enum.TryParse<TEnum>(labelToRawData[label], out parsedData);
  }

  //===================================================================
  // WindowType/Window別の機能
  //===================================================================

  /// Windowハンドルが正常かどうか
  public static bool IsWindowValid(WindowTypes windowType, UIntPtr window) {
    switch(windowType) {
      case WindowTypes.Normal: {
        return (window != UIntPtr.Zero && User32.IsWindow(window));
      }
      case WindowTypes.DesktopListView:
      case WindowTypes.Desktop: {
        return true;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// WindowのClient座標系での領域を返す
  /// @warning windowType == Desktop時のみ、左上端がマイナス座標になる可能性がある
  /// @param windowType Windowタイプ
  /// @param window Windowハンドル
  /// @return WindowのClient座標系での領域(Desktopのみ左上端が(0,0)とは限らない)
  public static ClientRect GetWindowRect(WindowTypes windowType, UIntPtr window) {
    switch (windowType) {
      case WindowTypes.Normal: {
        Debug.Assert(Utilities.IsWindowValid(windowType, window),
                     "Invalid Window", "GetWindowRect");
        User32.RECT windowRect;
        User32.GetClientRect(window, out windowRect);
        return new ClientRect(0, 0, windowRect.Right, windowRect.Bottom);
      }
      case WindowTypes.DesktopListView: {
        return new ClientRect(0, 0,
            User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
      }
      case WindowTypes.Desktop: {
        return new ClientRect(
            User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// Client座標系の座標をWindowタイプ別にScreen座標系に変換する
  public static ScreenPoint ClientToScreen(WindowTypes windowType, UIntPtr window, int clientX, int clientY) {
    switch (windowType) {
      case WindowTypes.Normal: {
        Debug.Assert(Utilities.IsWindowValid(windowType, window),
                     "Invalid Window", "ClientToScreen");
        User32.POINT windowPoint = new User32.POINT { X = clientX, Y = clientY };
        User32.ClientToScreen(window, ref windowPoint);
        return new ScreenPoint(windowPoint.X, windowPoint.Y);
      }
      case WindowTypes.DesktopListView: {
        //　仮想スクリーン座標なので補正を戻す
        return new ScreenPoint(clientX + User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
                               clientY + User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN));
      }
      case WindowTypes.Desktop: {
        // スクリーン座標系なのでそのまま返す
        return new ScreenPoint(clientX, clientY);
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// Windowタイプ別にScreen座標系での領域を返す
  public static ScreenRect GetWindowScreenRect(WindowTypes windowType, UIntPtr window) {
    var screenRect = Utilities.GetWindowRect(windowType, window);
    var screenPoint =
        Utilities.ClientToScreen(windowType, window, screenRect.X, screenRect.Y);
    return new ScreenRect(screenPoint.X, screenPoint.Y,
                          screenRect.Width, screenRect.Height);
  }

  //===================================================================
  // 特定のOSに依存しないDesktopListViewWindowの取得
  //===================================================================

  /// 特定のOSに依存しないDesktopListViewWindowの取得
  ///
  /// デスクトップのHWNDの階層関係は以下のとおり:
  /// - GetDesktopWindow()
  ///   - Progman (XP/Win7(No Aero)/Vista(No Aero))
  ///     - SHELLDLL_DefView (XP/Win7 No Aero/Vista No Aero?)
  ///       - Internet Exproler_Server (XP Active Desktop)
  ///       - SysListView32 (XP?/Win7 No Aero/Vista No Aero?)
  ///   - WorkerW[/WorkerW]* (Win7 Aero/Vista Aero?)
  ///     - SHELLDLL_DefView
  ///       - SysListView32
  ///   - EdgeUiInputWndClass (Win 8)
  ///
  /// パッと見る限り明らかに重いのはAero On時。EnumWindows必須。
  public static UIntPtr DesktopListViewWindow {
    get {
      UIntPtr progman = User32.FindWindowEx(UIntPtr.Zero,
          UIntPtr.Zero, "Progman", null);
      if (progman != UIntPtr.Zero) {
        // XP/Win7(No Aero)/Vista(No Aero)
        UIntPtr shellDLLDefView = User32.FindWindowEx(progman,
            UIntPtr.Zero, "SHELLDLL_DefView", null);
        if (shellDLLDefView != UIntPtr.Zero) {
          UIntPtr sysListView32 = User32.FindWindowEx(shellDLLDefView,
              UIntPtr.Zero, "SysListView32", null);
          if (sysListView32 != UIntPtr.Zero) {
            // XP(No ActiveDesktop)/Win7(No Aero)/Vista(No Aero)
            return sysListView32;
          } 
          UIntPtr internetExprolerServer = User32.FindWindowEx(shellDLLDefView,
              UIntPtr.Zero, "Internet Exproler_Server", null);
          if (internetExprolerServer != UIntPtr.Zero) {
            // XP(ActiveDesktop)
            return internetExprolerServer;
          }
        }
      }
      UIntPtr edgeUiInputWndClass = User32.FindWindowEx(UIntPtr.Zero,
          UIntPtr.Zero, "EdgeUiInputWndClass", null);
      if (edgeUiInputWndClass != UIntPtr.Zero) {
        // Win8
        return edgeUiInputWndClass;
      }
      enumerateWindowResult = UIntPtr.Zero;
      User32.EnumWindows(new User32.WNDENUMProc(EnumerateWindow), IntPtr.Zero);
      if (enumerateWindowResult != UIntPtr.Zero) {
        // Win7(Aero)/Vista(Aero)
        UIntPtr sysListView32 = User32.FindWindowEx(enumerateWindowResult,
            UIntPtr.Zero, "SysListView32", null);
        if (sysListView32 != UIntPtr.Zero) {
          return sysListView32;
        }
      }
      return User32.GetDesktopWindow();
    }
  }

  /// EnumerateWindowの結果を格納するポインタ
  private static UIntPtr enumerateWindowResult = UIntPtr.Zero;
  /// FindWindowExに渡されるWindow列挙関数
  private static bool EnumerateWindow(UIntPtr hWnd, IntPtr lParam) {
    StringBuilder className = new StringBuilder(256);
    User32.GetClassName(hWnd, className, 256);
    // "WorkerW"以外はスキップ
    if (className.ToString() != "WorkerW") return true;

    // "WorkerW" > "SHELLDLL_DefView"になってなければスキップ
    UIntPtr shellDLLDefView = User32.FindWindowEx(hWnd, UIntPtr.Zero, "SHELLDLL_DefView", null);
    if (shellDLLDefView == UIntPtr.Zero) return true;
      
    enumerateWindowResult = shellDLLDefView;
    return false;
  }
}
}   // namespace SCFF.Common
