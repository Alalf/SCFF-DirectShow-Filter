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

/// @file SCFF.Common.Tests/SandBox.cs
/// @copybrief SCFF::Common::Tests::SandBoxTest

namespace SCFF.Common.Tests {

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

interface IReader {
  int Value { get; }
}
interface IWriter {
  int Value { set; }
}
class FullAccess : IReader, IWriter {
  public int Value { get; set; }
}

// 継承によるインタフェースの実装

class Parent {
  public int Value { get; set; }
}

class Child : Parent, IReader, IWriter {

}

/// C#の機能調査用
[TestClass]
public class SandBoxTest {
  [TestMethod]
  public void TestRWInterfaces() {
    var obj = new FullAccess();
    obj.Value = 1;
    var test1 = obj.Value;
    Assert.AreEqual(test1, 1);
    var writable = obj as IWriter;
    writable.Value = 2;
    var readable = obj as IReader;
    var test2 = readable.Value;
    Assert.AreEqual(test2, 2);
    var child = new Child();
    child.Value = 3;
    var test3 = child.Value;
    Assert.AreEqual(test3, 3);
  }
}
}   // namespace SCFF.Common.Tests
