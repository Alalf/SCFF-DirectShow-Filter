#pragma once

namespace scffappnet {

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

/// @brief デスクトップでクリッピング領域を簡単に指定するためのフォーム
public ref class AreaSelectForm : public System::Windows::Forms::Form {

private: System::ComponentModel::Container ^components;
private: System::Windows::Forms::Label^  label1;

#pragma region Windows Form Designer generated code
/// <summary>
/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
/// コード エディターで変更しないでください。
/// </summary>
void InitializeComponent(void)
{
  this->label1 = (gcnew System::Windows::Forms::Label());
  this->SuspendLayout();
  // 
  // label1
  // 
  this->label1->AutoSize = true;
  this->label1->Location = System::Drawing::Point(12, 9);
  this->label1->Name = L"label1";
  this->label1->Size = System::Drawing::Size(117, 12);
  this->label1->TabIndex = 0;
  this->label1->Text = L"Double-click to Apply";
  // 
  // AreaSelectForm
  // 
  this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
  this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
  this->BackColor = System::Drawing::Color::FromArgb(static_cast<System::Int32>(static_cast<System::Byte>(255)), static_cast<System::Int32>(static_cast<System::Byte>(128)), 
    static_cast<System::Int32>(static_cast<System::Byte>(0)));
  this->ClientSize = System::Drawing::Size(320, 240);
  this->Controls->Add(this->label1);
  this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::None;
  this->Name = L"AreaSelectForm";
  this->Opacity = 0.5;
  this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Show;
  this->StartPosition = System::Windows::Forms::FormStartPosition::Manual;
  this->Text = L"AreaSelectForm";
  this->FormClosed += gcnew System::Windows::Forms::FormClosedEventHandler(this, &AreaSelectForm::AreaSelectForm_FormClosed);
  this->DoubleClick += gcnew System::EventHandler(this, &AreaSelectForm::AreaSelectForm_DoubleClick);
  this->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &AreaSelectForm::AreaSelectForm_MouseDown);
  this->MouseMove += gcnew System::Windows::Forms::MouseEventHandler(this, &AreaSelectForm::AreaSelectForm_MouseMove);
  this->MouseUp += gcnew System::Windows::Forms::MouseEventHandler(this, &AreaSelectForm::AreaSelectForm_MouseUp);
  this->ResumeLayout(false);
  this->PerformLayout();

}
#pragma endregion

 public:
  AreaSelectForm(void)
      : resizing(false),
        last(0,0) {
    InitializeComponent();
    //
    //TODO: ここにコンストラクター コードを追加します
    //
    clipping_x = 0;
    clipping_y = 0;
    clipping_width = 32;
    clipping_height = 32;
  }

 protected:
  /// <summary>
  /// 使用中のリソースをすべてクリーンアップします。
  /// </summary>
  ~AreaSelectForm() {
    if (components) {
      delete components;
    }
  }
 public:
  property int clipping_x;
  property int clipping_y;
  property int clipping_width;
  property int clipping_height;

 private:
  bool resizing;
  Point last;
  Point moving_start;

private: System::Void AreaSelectForm_DoubleClick(System::Object^  sender, System::EventArgs^  e) {
    this->Close();
  }
private: System::Void AreaSelectForm_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
    this->resizing = true;
    this->last = e->Location;
    this->moving_start = e->Location;
  }
private: System::Void AreaSelectForm_MouseUp(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
    this->resizing = false;
  }
private: System::Void AreaSelectForm_MouseMove(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
    if (resizing) {
      int w = this->Size.Width;
      int h = this->Size.Height;

      if (this->Cursor->Equals(Cursors::SizeNWSE)) {
        System::Drawing::Size my_size(w + (e->Location.X - last.X), h + (e->Location.Y - last.Y));
        if (my_size.Width > 32 && my_size.Height > 32) {
          this->Size = my_size;
        }
      } else if (this->Cursor->Equals(Cursors::SizeWE)) {
        System::Drawing::Size my_size(w + (e->Location.X - last.X), h);
        if (my_size.Width > 32 && my_size.Height > 32) {
          this->Size = my_size;
        }
      } else if (this->Cursor->Equals(Cursors::SizeNS)) {
        System::Drawing::Size my_size(w, h + (e->Location.Y - last.Y));
        if (my_size.Width > 32 && my_size.Height > 32) {
          this->Size = my_size;
        }
      } else if (this->Cursor->Equals(Cursors::SizeAll)) {
        this->Left += e->Location.X - moving_start.X;
        this->Top += e->Location.Y - moving_start.Y;
      }

      this->last = e->Location;
    } else {
      bool resize_x = e->X > (this->Width - 16);
      bool resize_y = e->Y > (this->Height - 16);

      if (resize_x && resize_y) {
        this->Cursor = Cursors::SizeNWSE;
      } else if (resize_x) {
        this->Cursor = Cursors::SizeWE;
      } else if (resize_y) {
        this->Cursor = Cursors::SizeNS;
      } else {
        this->Cursor = Cursors::SizeAll;
      }
    }
  }
private: System::Void AreaSelectForm_FormClosed(System::Object^  sender, System::Windows::Forms::FormClosedEventArgs^  e) {
    Point origin(0,0);
    clipping_x = PointToScreen(origin).X;
    clipping_y = PointToScreen(origin).Y;
    clipping_width = this->Size.Width;
    clipping_height = this->Size.Height;
  }
};
}
