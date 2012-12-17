// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

namespace SCFF.Common {

public class Options {
  public enum Key {
    // Recent File Menu
    RecentFilePath1,
    RecentFilePath2,
    RecentFilePath3,
    RecentFilePath4,
    RecentFilePath5,

    // FFmpeg FrontEnd Feature
    FFmpegPath,
    FFmpegArguments,

    // Main Window
    MainWindowLeft,
    MainWindowTop,
    MainWindowWidth,
    MainWindowHeight,
    MainWindowState,

    // Expander
    AreaExpanderIsExpanded,
    OptionsExpanderIsExpanded,
    ResizeMethodExpanderIsExpanded,
    LayoutExpanderIsExpanded,

    // SCFF Feature
    AutoApply,
    LayoutPreview,
    LayoutBorder,
    LayoutSnap,

    // Options Menu
    CompactView,
    ForceAeroOn,
    RestoreLastProfile
  }

  public enum WindowState {
    Normal,
    Minimized,
    Maximized
  }

  public void Load() {
    
  }

  public void Save() {
    
  }

  private const string OptionsFilePath = "SCFF.GUI.options";
Microsoft Help Viewer 2.0
  private string[] recentFilePath = new string[5] {
      string.Empty, string.Empty, string.Empty, string.Empty, string.Empty};
  private string ffmpegPath = string.Empty;
  private string ffmpegArguments = string.Empty;
  private double mainWindowLeft = 0.0;
  private double mainWindowRight = 0.0;
  private double mainWindowWidth = Defaults.MainWindowWidth;
  private double mainWindowHeight = Defaults.MainWindowHeight;
  private WindowState mainWindowState = WindowState.Normal;
  private bool areaExpanderIsExpanded = true;
  private bool optionsExpanderIsExpanded = true;
  private bool resizeMethodExpanderIsExpanded = true;
  private bool layoutExpanderIsExpanded = true;
  private bool autoApply = true;
  private bool layoutPreview = true;
  private bool layoutBorder = true;
  private bool layoutSnap = true;
  private bool compactView = false;
  private bool forceAeroOn = false;
  private bool restoreLastProfile = true;
}
}
