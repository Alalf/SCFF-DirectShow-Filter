#pragma once

#include "scffappnet/data_source.h"

namespace scff_interprocess {
class Interprocess;
struct LayoutParameter;
}

namespace scffappnet {

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

/// @brief メインウィンドウ
public ref class Form1 : public System::Windows::Forms::Form {

private: System::ComponentModel::IContainer^  components;
private: System::Windows::Forms::Label^  kVersion;
private: System::Windows::Forms::Label^  kDirectShowFilter;
private: System::Windows::Forms::Label^  kSCFF;
private: System::Windows::Forms::MenuStrip^  main_menu;
private: System::Windows::Forms::ToolStripMenuItem^  tools_menu;
private: System::Windows::Forms::ToolStripMenuItem^  cts_item;
private: System::Windows::Forms::ToolStripMenuItem^  language_menu;
private: System::Windows::Forms::ToolStripMenuItem^  english_item;
private: System::Windows::Forms::ToolStripMenuItem^  external_item;
private: System::Windows::Forms::ToolStripMenuItem^  ctc_item;
private: System::Windows::Forms::Button^  process_refresh;
private: System::Windows::Forms::GroupBox^  kTarget;
private: System::Windows::Forms::Panel^  kGreenPanel;
private: System::Windows::Forms::GroupBox^  kWindow;
private: System::Windows::Forms::Label^  kCaption;
private: System::Windows::Forms::TextBox^  window_handle;
private: System::Windows::Forms::StatusStrip^  main_status;
private: System::Windows::Forms::ComboBox^  process_combo;
private: System::Windows::Forms::Button^  window_desktop;
private: System::Windows::Forms::Button^  splash;
private: System::Windows::Forms::Button^  window_draghere;
private: System::Windows::Forms::ToolStripSplitButton^  layout_status;
private: System::Windows::Forms::ToolStripMenuItem^  layout1_item;
private: System::Windows::Forms::ToolStripStatusLabel^  status_status;
private: System::Windows::Forms::GroupBox^  kArea;
private: System::Windows::Forms::Button^  area_add;
private: System::Windows::Forms::ComboBox^  area_clipping_combo;
private: System::Windows::Forms::Label^  kCross;
private: System::Windows::Forms::Label^  kSize;
private: System::Windows::Forms::Label^  kY;
private: System::Windows::Forms::Label^  kX;
private: System::Windows::Forms::ToolStripSeparator^  kToolsSeparator;
private: System::Windows::Forms::ToolStripMenuItem^  aero_on_item;
private: System::Windows::Forms::Button^  target_apply;
private: System::Windows::Forms::Button^  target_area_select;
private: System::Windows::Forms::GroupBox^  kOption;
private: System::Windows::Forms::Label^  kThreadNum;
private: System::Windows::Forms::CheckBox^  option_over_sampling;
private: System::Windows::Forms::CheckBox^  option_enable_enlargement;
private: System::Windows::Forms::CheckBox^  option_keep_aspect_ratio;
private: System::Windows::Forms::CheckBox^  option_show_layered_window;
private: System::Windows::Forms::CheckBox^  option_show_mouse_cursor;
private: System::Windows::Forms::ComboBox^  option_resize_method_combo;
private: System::Windows::Forms::Label^  kResizeMethod;
private: System::Windows::Forms::Button^  apply;
private: System::Windows::Forms::ToolStripMenuItem^  layout2_item;
private: System::Windows::Forms::ToolStripMenuItem^  layout2_add;
private: System::Windows::Forms::ToolStripMenuItem^  layout2_remove;
private: System::Windows::Forms::ToolStripMenuItem^  layout1_add;
private: System::Windows::Forms::ToolStripMenuItem^  layout1_remove;
private: System::Windows::Forms::NumericUpDown^  area_clipping_height;
private: System::Windows::Forms::NumericUpDown^  area_clipping_width;
private: System::Windows::Forms::NumericUpDown^  area_clipping_y;
private: System::Windows::Forms::CheckBox^  area_fit;
private: System::Windows::Forms::NumericUpDown^  option_thread_num;
private: System::Windows::Forms::NumericUpDown^  area_clipping_x;

#pragma region Windows Form Designer generated code
  /// <summary>
  /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
  /// コード エディターで変更しないでください。
  /// </summary>
  void InitializeComponent(void)
  {
    System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(Form1::typeid));
    this->main_menu = (gcnew System::Windows::Forms::MenuStrip());
    this->tools_menu = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->cts_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->ctc_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->kToolsSeparator = (gcnew System::Windows::Forms::ToolStripSeparator());
    this->aero_on_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->language_menu = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->english_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->external_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->process_refresh = (gcnew System::Windows::Forms::Button());
    this->kTarget = (gcnew System::Windows::Forms::GroupBox());
    this->target_apply = (gcnew System::Windows::Forms::Button());
    this->target_area_select = (gcnew System::Windows::Forms::Button());
    this->kArea = (gcnew System::Windows::Forms::GroupBox());
    this->area_fit = (gcnew System::Windows::Forms::CheckBox());
    this->area_clipping_height = (gcnew System::Windows::Forms::NumericUpDown());
    this->area_clipping_width = (gcnew System::Windows::Forms::NumericUpDown());
    this->area_clipping_y = (gcnew System::Windows::Forms::NumericUpDown());
    this->area_clipping_x = (gcnew System::Windows::Forms::NumericUpDown());
    this->area_add = (gcnew System::Windows::Forms::Button());
    this->area_clipping_combo = (gcnew System::Windows::Forms::ComboBox());
    this->kCross = (gcnew System::Windows::Forms::Label());
    this->kSize = (gcnew System::Windows::Forms::Label());
    this->kY = (gcnew System::Windows::Forms::Label());
    this->kX = (gcnew System::Windows::Forms::Label());
    this->kWindow = (gcnew System::Windows::Forms::GroupBox());
    this->window_desktop = (gcnew System::Windows::Forms::Button());
    this->window_draghere = (gcnew System::Windows::Forms::Button());
    this->window_handle = (gcnew System::Windows::Forms::TextBox());
    this->kCaption = (gcnew System::Windows::Forms::Label());
    this->splash = (gcnew System::Windows::Forms::Button());
    this->kGreenPanel = (gcnew System::Windows::Forms::Panel());
    this->kVersion = (gcnew System::Windows::Forms::Label());
    this->kDirectShowFilter = (gcnew System::Windows::Forms::Label());
    this->kSCFF = (gcnew System::Windows::Forms::Label());
    this->main_status = (gcnew System::Windows::Forms::StatusStrip());
    this->layout_status = (gcnew System::Windows::Forms::ToolStripSplitButton());
    this->layout2_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->layout2_add = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->layout2_remove = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->layout1_item = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->layout1_add = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->layout1_remove = (gcnew System::Windows::Forms::ToolStripMenuItem());
    this->status_status = (gcnew System::Windows::Forms::ToolStripStatusLabel());
    this->process_combo = (gcnew System::Windows::Forms::ComboBox());
    this->kOption = (gcnew System::Windows::Forms::GroupBox());
    this->option_thread_num = (gcnew System::Windows::Forms::NumericUpDown());
    this->option_resize_method_combo = (gcnew System::Windows::Forms::ComboBox());
    this->kResizeMethod = (gcnew System::Windows::Forms::Label());
    this->kThreadNum = (gcnew System::Windows::Forms::Label());
    this->option_over_sampling = (gcnew System::Windows::Forms::CheckBox());
    this->option_enable_enlargement = (gcnew System::Windows::Forms::CheckBox());
    this->option_keep_aspect_ratio = (gcnew System::Windows::Forms::CheckBox());
    this->option_show_layered_window = (gcnew System::Windows::Forms::CheckBox());
    this->option_show_mouse_cursor = (gcnew System::Windows::Forms::CheckBox());
    this->apply = (gcnew System::Windows::Forms::Button());
    this->main_menu->SuspendLayout();
    this->kTarget->SuspendLayout();
    this->kArea->SuspendLayout();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_height))->BeginInit();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_width))->BeginInit();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_y))->BeginInit();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_x))->BeginInit();
    this->kWindow->SuspendLayout();
    this->kGreenPanel->SuspendLayout();
    this->main_status->SuspendLayout();
    this->kOption->SuspendLayout();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->option_thread_num))->BeginInit();
    this->SuspendLayout();
    // 
    // main_menu
    // 
    this->main_menu->AutoSize = false;
    this->main_menu->GripMargin = System::Windows::Forms::Padding(2);
    this->main_menu->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->tools_menu, this->language_menu});
    this->main_menu->LayoutStyle = System::Windows::Forms::ToolStripLayoutStyle::Flow;
    this->main_menu->Location = System::Drawing::Point(0, 34);
    this->main_menu->Name = L"main_menu";
    this->main_menu->RenderMode = System::Windows::Forms::ToolStripRenderMode::System;
    this->main_menu->Size = System::Drawing::Size(292, 25);
    this->main_menu->TabIndex = 0;
    this->main_menu->Text = L"main_menu";
    // 
    // tools_menu
    // 
    this->tools_menu->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Text;
    this->tools_menu->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(4) {this->cts_item, 
      this->ctc_item, this->kToolsSeparator, this->aero_on_item});
    this->tools_menu->Name = L"tools_menu";
    this->tools_menu->Size = System::Drawing::Size(45, 16);
    this->tools_menu->Text = L"Tools";
    // 
    // cts_item
    // 
    this->cts_item->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Text;
    this->cts_item->Enabled = false;
    this->cts_item->Name = L"cts_item";
    this->cts_item->Size = System::Drawing::Size(251, 22);
    this->cts_item->Text = L"Convert To Screen Coordinates";
    // 
    // ctc_item
    // 
    this->ctc_item->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Text;
    this->ctc_item->Enabled = false;
    this->ctc_item->Name = L"ctc_item";
    this->ctc_item->Size = System::Drawing::Size(251, 22);
    this->ctc_item->Text = L"Convert to Client-area Coordinates";
    // 
    // kToolsSeparator
    // 
    this->kToolsSeparator->Name = L"kToolsSeparator";
    this->kToolsSeparator->Size = System::Drawing::Size(248, 6);
    // 
    // aero_on_item
    // 
    this->aero_on_item->Name = L"aero_on_item";
    this->aero_on_item->Size = System::Drawing::Size(251, 22);
    this->aero_on_item->Text = L"Force Windows Aero Enabled";
    this->aero_on_item->Click += gcnew System::EventHandler(this, &Form1::aero_on_item_Click);
    // 
    // language_menu
    // 
    this->language_menu->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Text;
    this->language_menu->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->english_item, 
      this->external_item});
    this->language_menu->Enabled = false;
    this->language_menu->Name = L"language_menu";
    this->language_menu->Size = System::Drawing::Size(65, 16);
    this->language_menu->Text = L"Language";
    // 
    // english_item
    // 
    this->english_item->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Text;
    this->english_item->Enabled = false;
    this->english_item->Name = L"english_item";
    this->english_item->Size = System::Drawing::Size(112, 22);
    this->english_item->Text = L"English";
    // 
    // external_item
    // 
    this->external_item->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Text;
    this->external_item->Enabled = false;
    this->external_item->Name = L"external_item";
    this->external_item->Size = System::Drawing::Size(112, 22);
    this->external_item->Text = L"External";
    // 
    // process_refresh
    // 
    this->process_refresh->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->process_refresh->Location = System::Drawing::Point(228, 66);
    this->process_refresh->Name = L"process_refresh";
    this->process_refresh->Size = System::Drawing::Size(60, 21);
    this->process_refresh->TabIndex = 2;
    this->process_refresh->Text = L"Refresh";
    this->process_refresh->UseVisualStyleBackColor = true;
    this->process_refresh->Click += gcnew System::EventHandler(this, &Form1::process_refresh_Click);
    // 
    // kTarget
    // 
    this->kTarget->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->kTarget->Controls->Add(this->target_apply);
    this->kTarget->Controls->Add(this->target_area_select);
    this->kTarget->Controls->Add(this->kArea);
    this->kTarget->Controls->Add(this->kWindow);
    this->kTarget->Location = System::Drawing::Point(7, 96);
    this->kTarget->Name = L"kTarget";
    this->kTarget->Size = System::Drawing::Size(281, 197);
    this->kTarget->TabIndex = 3;
    this->kTarget->TabStop = false;
    this->kTarget->Text = L"Target";
    // 
    // target_apply
    // 
    this->target_apply->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->target_apply->Enabled = false;
    this->target_apply->Location = System::Drawing::Point(150, 165);
    this->target_apply->Name = L"target_apply";
    this->target_apply->Size = System::Drawing::Size(125, 23);
    this->target_apply->TabIndex = 3;
    this->target_apply->Text = L"Apply";
    this->target_apply->UseVisualStyleBackColor = true;
    // 
    // target_area_select
    // 
    this->target_area_select->Location = System::Drawing::Point(9, 165);
    this->target_area_select->Name = L"target_area_select";
    this->target_area_select->Size = System::Drawing::Size(136, 23);
    this->target_area_select->TabIndex = 2;
    this->target_area_select->Text = L"Area Selection";
    this->target_area_select->UseVisualStyleBackColor = true;
    this->target_area_select->Click += gcnew System::EventHandler(this, &Form1::target_area_select_Click);
    // 
    // kArea
    // 
    this->kArea->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->kArea->Controls->Add(this->area_fit);
    this->kArea->Controls->Add(this->area_clipping_height);
    this->kArea->Controls->Add(this->area_clipping_width);
    this->kArea->Controls->Add(this->area_clipping_y);
    this->kArea->Controls->Add(this->area_clipping_x);
    this->kArea->Controls->Add(this->area_add);
    this->kArea->Controls->Add(this->area_clipping_combo);
    this->kArea->Controls->Add(this->kCross);
    this->kArea->Controls->Add(this->kSize);
    this->kArea->Controls->Add(this->kY);
    this->kArea->Controls->Add(this->kX);
    this->kArea->Location = System::Drawing::Point(6, 89);
    this->kArea->Name = L"kArea";
    this->kArea->Size = System::Drawing::Size(269, 70);
    this->kArea->TabIndex = 1;
    this->kArea->TabStop = false;
    this->kArea->Text = L"Area";
    // 
    // area_fit
    // 
    this->area_fit->AutoSize = true;
    this->area_fit->Location = System::Drawing::Point(19, 17);
    this->area_fit->Name = L"area_fit";
    this->area_fit->Size = System::Drawing::Size(38, 16);
    this->area_fit->TabIndex = 0;
    this->area_fit->Text = L"Fit";
    this->area_fit->UseVisualStyleBackColor = true;
    this->area_fit->CheckedChanged += gcnew System::EventHandler(this, &Form1::area_fit_CheckedChanged);
    // 
    // area_clipping_height
    // 
    this->area_clipping_height->Location = System::Drawing::Point(214, 41);
    this->area_clipping_height->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) {4096, 0, 0, 0});
    this->area_clipping_height->Minimum = System::Decimal(gcnew cli::array< System::Int32 >(4) {1, 0, 0, 0});
    this->area_clipping_height->Name = L"area_clipping_height";
    this->area_clipping_height->Size = System::Drawing::Size(50, 19);
    this->area_clipping_height->TabIndex = 6;
    this->area_clipping_height->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) {360, 0, 0, 0});
    this->area_clipping_height->ValueChanged += gcnew System::EventHandler(this, &Form1::area_clipping_height_ValueChanged);
    // 
    // area_clipping_width
    // 
    this->area_clipping_width->Location = System::Drawing::Point(151, 41);
    this->area_clipping_width->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) {4096, 0, 0, 0});
    this->area_clipping_width->Minimum = System::Decimal(gcnew cli::array< System::Int32 >(4) {1, 0, 0, 0});
    this->area_clipping_width->Name = L"area_clipping_width";
    this->area_clipping_width->Size = System::Drawing::Size(50, 19);
    this->area_clipping_width->TabIndex = 5;
    this->area_clipping_width->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) {640, 0, 0, 0});
    this->area_clipping_width->ValueChanged += gcnew System::EventHandler(this, &Form1::area_clipping_width_ValueChanged);
    // 
    // area_clipping_y
    // 
    this->area_clipping_y->Location = System::Drawing::Point(78, 41);
    this->area_clipping_y->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) {4096, 0, 0, 0});
    this->area_clipping_y->Name = L"area_clipping_y";
    this->area_clipping_y->Size = System::Drawing::Size(43, 19);
    this->area_clipping_y->TabIndex = 4;
    this->area_clipping_y->ValueChanged += gcnew System::EventHandler(this, &Form1::area_clipping_y_ValueChanged);
    // 
    // area_clipping_x
    // 
    this->area_clipping_x->Location = System::Drawing::Point(19, 41);
    this->area_clipping_x->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) {4096, 0, 0, 0});
    this->area_clipping_x->Name = L"area_clipping_x";
    this->area_clipping_x->Size = System::Drawing::Size(43, 19);
    this->area_clipping_x->TabIndex = 3;
    this->area_clipping_x->ValueChanged += gcnew System::EventHandler(this, &Form1::area_clipping_x_ValueChanged);
    // 
    // area_add
    // 
    this->area_add->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->area_add->Enabled = false;
    this->area_add->Location = System::Drawing::Point(229, 15);
    this->area_add->Name = L"area_add";
    this->area_add->Size = System::Drawing::Size(35, 20);
    this->area_add->TabIndex = 2;
    this->area_add->Text = L"Add";
    this->area_add->UseVisualStyleBackColor = true;
    // 
    // area_clipping_combo
    // 
    this->area_clipping_combo->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->area_clipping_combo->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
    this->area_clipping_combo->Enabled = false;
    this->area_clipping_combo->FormattingEnabled = true;
    this->area_clipping_combo->Location = System::Drawing::Point(78, 15);
    this->area_clipping_combo->Name = L"area_clipping_combo";
    this->area_clipping_combo->Size = System::Drawing::Size(145, 20);
    this->area_clipping_combo->TabIndex = 1;
    // 
    // kCross
    // 
    this->kCross->Location = System::Drawing::Point(202, 43);
    this->kCross->Name = L"kCross";
    this->kCross->Size = System::Drawing::Size(10, 14);
    this->kCross->TabIndex = 7;
    this->kCross->Text = L"x";
    // 
    // kSize
    // 
    this->kSize->Location = System::Drawing::Point(124, 44);
    this->kSize->Name = L"kSize";
    this->kSize->Size = System::Drawing::Size(26, 12);
    this->kSize->TabIndex = 5;
    this->kSize->Text = L"Size";
    // 
    // kY
    // 
    this->kY->Location = System::Drawing::Point(64, 44);
    this->kY->Name = L"kY";
    this->kY->Size = System::Drawing::Size(12, 12);
    this->kY->TabIndex = 3;
    this->kY->Text = L"Y";
    // 
    // kX
    // 
    this->kX->Location = System::Drawing::Point(5, 44);
    this->kX->Name = L"kX";
    this->kX->Size = System::Drawing::Size(12, 12);
    this->kX->TabIndex = 1;
    this->kX->Text = L"X";
    // 
    // kWindow
    // 
    this->kWindow->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->kWindow->Controls->Add(this->window_desktop);
    this->kWindow->Controls->Add(this->window_draghere);
    this->kWindow->Controls->Add(this->window_handle);
    this->kWindow->Controls->Add(this->kCaption);
    this->kWindow->Location = System::Drawing::Point(12, 13);
    this->kWindow->Name = L"kWindow";
    this->kWindow->Size = System::Drawing::Size(263, 73);
    this->kWindow->TabIndex = 0;
    this->kWindow->TabStop = false;
    this->kWindow->Text = L"Window";
    // 
    // window_desktop
    // 
    this->window_desktop->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->window_desktop->Location = System::Drawing::Point(185, 42);
    this->window_desktop->Name = L"window_desktop";
    this->window_desktop->Size = System::Drawing::Size(69, 23);
    this->window_desktop->TabIndex = 3;
    this->window_desktop->Text = L"Desktop";
    this->window_desktop->UseVisualStyleBackColor = true;
    this->window_desktop->Click += gcnew System::EventHandler(this, &Form1::window_desktop_Click);
    // 
    // window_draghere
    // 
    this->window_draghere->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->window_draghere->Location = System::Drawing::Point(7, 42);
    this->window_draghere->Name = L"window_draghere";
    this->window_draghere->Size = System::Drawing::Size(172, 23);
    this->window_draghere->TabIndex = 2;
    this->window_draghere->Text = L"Drag here.";
    this->window_draghere->UseVisualStyleBackColor = true;
    this->window_draghere->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::window_draghere_MouseDown);
    this->window_draghere->MouseUp += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::window_draghere_MouseUp);
    // 
    // window_handle
    // 
    this->window_handle->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->window_handle->Location = System::Drawing::Point(55, 16);
    this->window_handle->Name = L"window_handle";
    this->window_handle->ReadOnly = true;
    this->window_handle->Size = System::Drawing::Size(199, 19);
    this->window_handle->TabIndex = 1;
    this->window_handle->TabStop = false;
    // 
    // kCaption
    // 
    this->kCaption->Location = System::Drawing::Point(7, 19);
    this->kCaption->Name = L"kCaption";
    this->kCaption->Size = System::Drawing::Size(44, 12);
    this->kCaption->TabIndex = 0;
    this->kCaption->Text = L"Caption";
    // 
    // splash
    // 
    this->splash->Enabled = false;
    this->splash->Location = System::Drawing::Point(7, 404);
    this->splash->Name = L"splash";
    this->splash->Size = System::Drawing::Size(81, 23);
    this->splash->TabIndex = 5;
    this->splash->Text = L"Splash";
    this->splash->UseVisualStyleBackColor = true;
    this->splash->Click += gcnew System::EventHandler(this, &Form1::splash_Click);
    // 
    // kGreenPanel
    // 
    this->kGreenPanel->BackColor = System::Drawing::Color::FromArgb(static_cast<System::Int32>(static_cast<System::Byte>(0)), static_cast<System::Int32>(static_cast<System::Byte>(59)), 
      static_cast<System::Int32>(static_cast<System::Byte>(0)));
    this->kGreenPanel->Controls->Add(this->kVersion);
    this->kGreenPanel->Controls->Add(this->kDirectShowFilter);
    this->kGreenPanel->Controls->Add(this->kSCFF);
    this->kGreenPanel->Dock = System::Windows::Forms::DockStyle::Top;
    this->kGreenPanel->Location = System::Drawing::Point(0, 0);
    this->kGreenPanel->Name = L"kGreenPanel";
    this->kGreenPanel->Size = System::Drawing::Size(292, 34);
    this->kGreenPanel->TabIndex = 3;
    // 
    // kVersion
    // 
    this->kVersion->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->kVersion->Font = (gcnew System::Drawing::Font(L"Verdana", 8.25F, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
      static_cast<System::Byte>(0)));
    this->kVersion->ForeColor = System::Drawing::Color::White;
    this->kVersion->Location = System::Drawing::Point(222, 11);
    this->kVersion->Name = L"kVersion";
    this->kVersion->Size = System::Drawing::Size(65, 13);
    this->kVersion->TabIndex = 5;
    this->kVersion->Text = L"Ver.0.0.1";
    // 
    // kDirectShowFilter
    // 
    this->kDirectShowFilter->Font = (gcnew System::Drawing::Font(L"Verdana", 8.25F, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
      static_cast<System::Byte>(0)));
    this->kDirectShowFilter->ForeColor = System::Drawing::Color::FromArgb(static_cast<System::Int32>(static_cast<System::Byte>(184)), 
      static_cast<System::Int32>(static_cast<System::Byte>(184)), static_cast<System::Int32>(static_cast<System::Byte>(184)));
    this->kDirectShowFilter->Location = System::Drawing::Point(52, 11);
    this->kDirectShowFilter->Name = L"kDirectShowFilter";
    this->kDirectShowFilter->Size = System::Drawing::Size(119, 13);
    this->kDirectShowFilter->TabIndex = 4;
    this->kDirectShowFilter->Text = L"DirectShow Filter";
    // 
    // kSCFF
    // 
    this->kSCFF->Font = (gcnew System::Drawing::Font(L"Verdana", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
      static_cast<System::Byte>(0)));
    this->kSCFF->ForeColor = System::Drawing::Color::White;
    this->kSCFF->Location = System::Drawing::Point(5, 8);
    this->kSCFF->Name = L"kSCFF";
    this->kSCFF->Size = System::Drawing::Size(51, 18);
    this->kSCFF->TabIndex = 3;
    this->kSCFF->Text = L"SCFF";
    // 
    // main_status
    // 
    this->main_status->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->layout_status, this->status_status});
    this->main_status->Location = System::Drawing::Point(0, 451);
    this->main_status->Name = L"main_status";
    this->main_status->Size = System::Drawing::Size(292, 22);
    this->main_status->TabIndex = 4;
    this->main_status->Text = L"statusStrip1";
    // 
    // layout_status
    // 
    this->layout_status->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->layout2_item, 
      this->layout1_item});
    this->layout_status->Image = (cli::safe_cast<System::Drawing::Image^  >(resources->GetObject(L"layout_status.Image")));
    this->layout_status->ImageTransparentColor = System::Drawing::Color::Magenta;
    this->layout_status->Name = L"layout_status";
    this->layout_status->Size = System::Drawing::Size(71, 20);
    this->layout_status->Text = L"Layout";
    // 
    // layout2_item
    // 
    this->layout2_item->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->layout2_add, 
      this->layout2_remove});
    this->layout2_item->Enabled = false;
    this->layout2_item->Name = L"layout2_item";
    this->layout2_item->Size = System::Drawing::Size(114, 22);
    this->layout2_item->Text = L"Layout 2";
    // 
    // layout2_add
    // 
    this->layout2_add->Enabled = false;
    this->layout2_add->Name = L"layout2_add";
    this->layout2_add->Size = System::Drawing::Size(111, 22);
    this->layout2_add->Text = L"Add";
    // 
    // layout2_remove
    // 
    this->layout2_remove->Enabled = false;
    this->layout2_remove->Name = L"layout2_remove";
    this->layout2_remove->Size = System::Drawing::Size(111, 22);
    this->layout2_remove->Text = L"Remove";
    // 
    // layout1_item
    // 
    this->layout1_item->Checked = true;
    this->layout1_item->CheckState = System::Windows::Forms::CheckState::Checked;
    this->layout1_item->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {this->layout1_add, 
      this->layout1_remove});
    this->layout1_item->Name = L"layout1_item";
    this->layout1_item->Size = System::Drawing::Size(114, 22);
    this->layout1_item->Text = L"Layout 1";
    // 
    // layout1_add
    // 
    this->layout1_add->Enabled = false;
    this->layout1_add->Name = L"layout1_add";
    this->layout1_add->Size = System::Drawing::Size(111, 22);
    this->layout1_add->Text = L"Add";
    // 
    // layout1_remove
    // 
    this->layout1_remove->Enabled = false;
    this->layout1_remove->Name = L"layout1_remove";
    this->layout1_remove->Size = System::Drawing::Size(111, 22);
    this->layout1_remove->Text = L"Remove";
    // 
    // status_status
    // 
    this->status_status->Name = L"status_status";
    this->status_status->RightToLeft = System::Windows::Forms::RightToLeft::No;
    this->status_status->Size = System::Drawing::Size(206, 17);
    this->status_status->Spring = true;
    this->status_status->Text = L"Error Code: ";
    this->status_status->TextAlign = System::Drawing::ContentAlignment::MiddleRight;
    // 
    // process_combo
    // 
    this->process_combo->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->process_combo->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
    this->process_combo->FormattingEnabled = true;
    this->process_combo->Location = System::Drawing::Point(7, 67);
    this->process_combo->Name = L"process_combo";
    this->process_combo->Size = System::Drawing::Size(215, 20);
    this->process_combo->TabIndex = 1;
    this->process_combo->SelectedIndexChanged += gcnew System::EventHandler(this, &Form1::process_combo_SelectedIndexChanged);
    // 
    // kOption
    // 
    this->kOption->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->kOption->Controls->Add(this->option_thread_num);
    this->kOption->Controls->Add(this->option_resize_method_combo);
    this->kOption->Controls->Add(this->kResizeMethod);
    this->kOption->Controls->Add(this->kThreadNum);
    this->kOption->Controls->Add(this->option_over_sampling);
    this->kOption->Controls->Add(this->option_enable_enlargement);
    this->kOption->Controls->Add(this->option_keep_aspect_ratio);
    this->kOption->Controls->Add(this->option_show_layered_window);
    this->kOption->Controls->Add(this->option_show_mouse_cursor);
    this->kOption->Location = System::Drawing::Point(7, 294);
    this->kOption->Name = L"kOption";
    this->kOption->Size = System::Drawing::Size(281, 104);
    this->kOption->TabIndex = 4;
    this->kOption->TabStop = false;
    this->kOption->Text = L"Option";
    // 
    // option_thread_num
    // 
    this->option_thread_num->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->option_thread_num->Enabled = false;
    this->option_thread_num->Location = System::Drawing::Point(228, 28);
    this->option_thread_num->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) {8, 0, 0, 0});
    this->option_thread_num->Minimum = System::Decimal(gcnew cli::array< System::Int32 >(4) {1, 0, 0, 0});
    this->option_thread_num->Name = L"option_thread_num";
    this->option_thread_num->Size = System::Drawing::Size(47, 19);
    this->option_thread_num->TabIndex = 5;
    this->option_thread_num->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) {1, 0, 0, 0});
    this->option_thread_num->ValueChanged += gcnew System::EventHandler(this, &Form1::option_thread_num_ValueChanged);
    // 
    // option_resize_method_combo
    // 
    this->option_resize_method_combo->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->option_resize_method_combo->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
    this->option_resize_method_combo->FormattingEnabled = true;
    this->option_resize_method_combo->Location = System::Drawing::Point(157, 75);
    this->option_resize_method_combo->Name = L"option_resize_method_combo";
    this->option_resize_method_combo->Size = System::Drawing::Size(118, 20);
    this->option_resize_method_combo->TabIndex = 6;
    this->option_resize_method_combo->SelectedIndexChanged += gcnew System::EventHandler(this, &Form1::option_resize_method_combo_SelectedIndexChanged);
    // 
    // kResizeMethod
    // 
    this->kResizeMethod->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->kResizeMethod->Location = System::Drawing::Point(195, 60);
    this->kResizeMethod->Name = L"kResizeMethod";
    this->kResizeMethod->Size = System::Drawing::Size(80, 12);
    this->kResizeMethod->TabIndex = 7;
    this->kResizeMethod->Text = L"Resize Method";
    // 
    // kThreadNum
    // 
    this->kThreadNum->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
    this->kThreadNum->Enabled = false;
    this->kThreadNum->Location = System::Drawing::Point(155, 31);
    this->kThreadNum->Name = L"kThreadNum";
    this->kThreadNum->Size = System::Drawing::Size(67, 12);
    this->kThreadNum->TabIndex = 5;
    this->kThreadNum->Text = L"Thread Num";
    // 
    // option_over_sampling
    // 
    this->option_over_sampling->Enabled = false;
    this->option_over_sampling->Location = System::Drawing::Point(7, 79);
    this->option_over_sampling->Name = L"option_over_sampling";
    this->option_over_sampling->Size = System::Drawing::Size(100, 16);
    this->option_over_sampling->TabIndex = 4;
    this->option_over_sampling->Text = L"Over-Sampling";
    this->option_over_sampling->UseVisualStyleBackColor = true;
    this->option_over_sampling->CheckedChanged += gcnew System::EventHandler(this, &Form1::option_over_sampling_CheckedChanged);
    // 
    // option_enable_enlargement
    // 
    this->option_enable_enlargement->Location = System::Drawing::Point(7, 63);
    this->option_enable_enlargement->Name = L"option_enable_enlargement";
    this->option_enable_enlargement->Size = System::Drawing::Size(125, 16);
    this->option_enable_enlargement->TabIndex = 3;
    this->option_enable_enlargement->Text = L"Enable Enlargement";
    this->option_enable_enlargement->UseVisualStyleBackColor = true;
    this->option_enable_enlargement->CheckedChanged += gcnew System::EventHandler(this, &Form1::option_enable_enlargement_CheckedChanged);
    // 
    // option_keep_aspect_ratio
    // 
    this->option_keep_aspect_ratio->Location = System::Drawing::Point(7, 47);
    this->option_keep_aspect_ratio->Name = L"option_keep_aspect_ratio";
    this->option_keep_aspect_ratio->Size = System::Drawing::Size(120, 16);
    this->option_keep_aspect_ratio->TabIndex = 2;
    this->option_keep_aspect_ratio->Text = L"Keep Aspect Ratio";
    this->option_keep_aspect_ratio->UseVisualStyleBackColor = true;
    this->option_keep_aspect_ratio->CheckedChanged += gcnew System::EventHandler(this, &Form1::option_keep_aspect_ratio_CheckedChanged);
    // 
    // option_show_layered_window
    // 
    this->option_show_layered_window->Location = System::Drawing::Point(7, 31);
    this->option_show_layered_window->Name = L"option_show_layered_window";
    this->option_show_layered_window->Size = System::Drawing::Size(137, 16);
    this->option_show_layered_window->TabIndex = 1;
    this->option_show_layered_window->Text = L"Show Layered Window";
    this->option_show_layered_window->UseVisualStyleBackColor = true;
    this->option_show_layered_window->CheckedChanged += gcnew System::EventHandler(this, &Form1::option_show_layered_window_CheckedChanged);
    // 
    // option_show_mouse_cursor
    // 
    this->option_show_mouse_cursor->Location = System::Drawing::Point(7, 15);
    this->option_show_mouse_cursor->Name = L"option_show_mouse_cursor";
    this->option_show_mouse_cursor->Size = System::Drawing::Size(126, 16);
    this->option_show_mouse_cursor->TabIndex = 0;
    this->option_show_mouse_cursor->Text = L"Show Mouse Cursor";
    this->option_show_mouse_cursor->UseVisualStyleBackColor = true;
    this->option_show_mouse_cursor->CheckedChanged += gcnew System::EventHandler(this, &Form1::option_show_mouse_cursor_CheckedChanged);
    // 
    // apply
    // 
    this->apply->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
      | System::Windows::Forms::AnchorStyles::Right));
    this->apply->Enabled = false;
    this->apply->Location = System::Drawing::Point(152, 404);
    this->apply->Name = L"apply";
    this->apply->Size = System::Drawing::Size(136, 23);
    this->apply->TabIndex = 6;
    this->apply->Text = L"Apply";
    this->apply->UseVisualStyleBackColor = true;
    this->apply->Click += gcnew System::EventHandler(this, &Form1::apply_Click);
    // 
    // Form1
    // 
    this->AcceptButton = this->apply;
    this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
    this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
    this->ClientSize = System::Drawing::Size(292, 473);
    this->Controls->Add(this->apply);
    this->Controls->Add(this->splash);
    this->Controls->Add(this->kOption);
    this->Controls->Add(this->main_status);
    this->Controls->Add(this->main_menu);
    this->Controls->Add(this->kGreenPanel);
    this->Controls->Add(this->kTarget);
    this->Controls->Add(this->process_combo);
    this->Controls->Add(this->process_refresh);
    this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
    this->MainMenuStrip = this->main_menu;
    this->MaximizeBox = false;
    this->MaximumSize = System::Drawing::Size(800, 500);
    this->MinimumSize = System::Drawing::Size(300, 500);
    this->Name = L"Form1";
    this->Text = L"SCFF DirectShow Filter Ver.0.0.1";
    this->Shown += gcnew System::EventHandler(this, &Form1::Form1_Shown);
    this->main_menu->ResumeLayout(false);
    this->main_menu->PerformLayout();
    this->kTarget->ResumeLayout(false);
    this->kArea->ResumeLayout(false);
    this->kArea->PerformLayout();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_height))->EndInit();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_width))->EndInit();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_y))->EndInit();
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->area_clipping_x))->EndInit();
    this->kWindow->ResumeLayout(false);
    this->kWindow->PerformLayout();
    this->kGreenPanel->ResumeLayout(false);
    this->main_status->ResumeLayout(false);
    this->main_status->PerformLayout();
    this->kOption->ResumeLayout(false);
    (cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->option_thread_num))->EndInit();
    this->ResumeLayout(false);
    this->PerformLayout();

  }
#pragma endregion

  //-------------------------------------------------------------------
  // メソッド
  //-------------------------------------------------------------------
 public:
  /// @brief コンストラクタ
  Form1(void);

 protected: 
  /// @brief デストラクタ
  ~Form1();

 private:
  /// @brief 共有メモリからディレクトリを取得し、いろいろ処理
  void UpdateDirectory();
  /// @brief 共有メモリにNullLayoutリクエストを設定
  void SendNullLayoutRequest();
  /// @brief 共有メモリにNativeLayoutリクエストを設定
  void SendNativeLayoutRequest();

  //-------------------------------------------------------------------

  /// @brief ResizeMethod ComboBoxにデータソースを設定する
  void BuildResizeMethodCombobox();

  //-------------------------------------------------------------------

  /// @brief ウィンドウを指定する
  void SetWindow(HWND window_handle);
  /// @brief デスクトップを全画面で取り込む
  void DoCaptureDesktopWindow();
  /// @brief クリッピング領域をリセットする
  void ResetClippingRegion();

  /// @brief パラメータのValidate
  bool ValidateParameters();

  //-------------------------------------------------------------------

  /// @brief Dwmapi.dllを利用してAeroをOffに
  void DWMAPIOff();
  /// @brief 強制的にAeroのOn/Offを切り替える
  void DWMAPIFlip();
  /// @brief AeroをOffにしていたらOnに戻す
  void DWMAPIRestore();
  /// @brief Dwmapi.dllが利用可能かどうか
  bool can_use_dwmapi_dll_;
  /// @brief Aeroが起動時にONになっていたかどうか
  bool was_dwm_enabled_on_start_;

  //-------------------------------------------------------------------

  /// @brief プロセス間通信用オブジェクト
  scff_interprocess::Interprocess *interprocess_;
  /// @brief 現在編集中のレイアウト番号
  property int editing_layout_index_;
  /// @brief 現在編集中のレイアウトパラメータ
  scff_interprocess::LayoutParameter *layout_parameter_;

  //-------------------------------------------------------------------
  // イベントハンドラ
  //-------------------------------------------------------------------
private: System::Void aero_on_item_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void window_draghere_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e);
private: System::Void process_refresh_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void window_draghere_MouseUp(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e);
private: System::Void splash_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void apply_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void process_combo_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void area_fit_CheckedChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void window_desktop_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void area_clipping_x_ValueChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void area_clipping_y_ValueChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void area_clipping_width_ValueChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void area_clipping_height_ValueChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_show_mouse_cursor_CheckedChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_show_layered_window_CheckedChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_keep_aspect_ratio_CheckedChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_enable_enlargement_CheckedChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_over_sampling_CheckedChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_thread_num_ValueChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void option_resize_method_combo_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e);
private: System::Void target_area_select_Click(System::Object^  sender, System::EventArgs^  e);
private: System::Void Form1_Shown(System::Object^  sender, System::EventArgs^  e);
};
}   // namepspace
