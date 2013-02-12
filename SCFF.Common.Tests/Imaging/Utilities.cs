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

/// @file SCFF.Common.Tests/Imaging/Utilities.cs
/// @copybrief SCFF::Common::Tests::Imaging::Utilities

/// SCFF.Commonモジュールのユニットテスト
namespace SCFF.Common.Tests.Imaging {

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// SCFF.Common.Imaging.Utilitiesモジュールのユニットテスト
[TestClass]
public class UtilitiesTest {
  [TestMethod]
  public void TestSameSize() {
    int x, y, width, height;
    SCFF.Common.Imaging.Utilities.CalculateLayout(
        0, 0, 640, 480,
        640, 480,
        true, true,
        out x, out y, out width, out height);
    Assert.AreEqual(x, 0);
    Assert.AreEqual(y, 0);
    Assert.AreEqual(width, 640);
    Assert.AreEqual(height, 480);
  }

  [TestMethod]
  public void TestNearlyEqualSize() {
    int x, y, width, height;
    SCFF.Common.Imaging.Utilities.CalculateLayout(
        0, 0, 640, 480,
        639, 479,
        true, true,
        out x, out y, out width, out height);
    Assert.AreEqual(x, 0);
    Assert.AreEqual(y, 0);
    Assert.AreEqual(width, 640);
    Assert.AreEqual(height, 480);
  }

  [TestMethod]
  public void TestPillarBoxSize() {
    int x, y, width, height;
    SCFF.Common.Imaging.Utilities.CalculateLayout(
        0, 0, 640, 480,
        480, 640,
        true, true,
        out x, out y, out width, out height);
    Assert.AreEqual(x, 140);
    Assert.AreEqual(y, 0);
    Assert.AreEqual(width, 360);
    Assert.AreEqual(height, 480);
  }

  [TestMethod]
  public void TestIrrationalAspectSize() {
    int x, y, width, height;
    SCFF.Common.Imaging.Utilities.CalculateLayout(
        0, 0, 640, 480,
        638, 479,
        true, true,
        out x, out y, out width, out height);
    Assert.AreEqual(x, 0);
    Assert.AreEqual(y, 0);
    Assert.AreEqual(width, 640);
    Assert.AreEqual(height, 480);
  }
}
}   // namespace SCFF.Common.Tests.Imaging
