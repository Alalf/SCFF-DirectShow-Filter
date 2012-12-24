
namespace SCFF.GUI.Controls {

/// ユーザコントロール実装時に必要なメソッドをまとめたインタフェース
///
/// DataBindingを使うならこんなインタフェースはいらないのだが・・・
interface IProfileEditor {
  void UpdateByProfile();
  void AttachChangedEventHandlers();
  void DetachChangedEventHandlers();
}
}
