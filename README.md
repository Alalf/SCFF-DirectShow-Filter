# SCFF DirectShow Filter Ver 0.1.0 (2012/5/14)

SCFF directShow Filter��Windows�p�X�N���[���L���v�`���v���O����(DirectShow�t�B���^)�ł��B
ffmpeg��Windows Media Encoder�̉f�����͂Ƃ��Ďg���邱�Ƃ�z�肵�Ă��܂��B



## �K�v�����
* (�J�����̂��ߒ������ł�)
* Windows XP SP3
* ��ʂ̐F��: 32bit True Color

## �œK�����
* Windows 7
* CPU: Intel Sandy Bridge/Ivy Bridge
  * Lucid Virtu (MVP) I-Mode��ON�ɂ����ꍇ�A�ō��̃p�t�H�[�}���X�����҂ł��܂��B



## �C���X�g�[�����@
1. �ȉ��̃����^�C�����C���X�g�[�����Ă�������:
  * ����:
    * http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=e5ad0459-cbcc-4b4f-97b6-fb17111cf544
  * 32bit OS�̏ꍇ:
    * http://www.microsoft.com/downloads/ja-jp/details.aspx?familyid=c32f406a-f8fc-4164-b6eb-5328b8578f03
  * 64bit OS�̏ꍇ:
    * http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=C68CCBB6-75EF-4C9D-A326-879EAB4FCDF8

2. install-*.bat�����s���Ă�������
  * *******************************************************************
    * �d�v�I                                                          *
    *******************************************************************
    �C���X�g�[�����scff-*.ax���ړ��������ꍇ�͍ēxinstall-*.bat�����s���Ă��������B



## �g�p���@
1. �L���v�`���\�t�g�ŁuSCFF DirectShow Filter�v��I�����܂��B

2. *******************************************************************
   * �d�v�I                                                          *
   *******************************************************************
   SCFF DirectShow Filter�͎�荞�݃T�C�Y�ƃt���[�����[�g��
   �o�̓T�C�Y�Ɏ����I�ɍ��킹���܂��B
   �e��G���R�[�_�ŁA�܂��͏o�̓T�C�Y�A�t���[�����[�g��ݒ肵�Ă��������B

3. �v���r���[��ʂ��m�F���A���S�������ɕ\������邱�Ƃ��m�F���Ă��������B

4. scff-app.exe�����s���Ă��������B

5. ����̃v���Z�X���j���[����ړI�̃v���Z�X��I�����A
   ��荞�ݐݒ��AApply�{�^���������Ă��������B
   (Apply�{�^������Auto�`�F�b�N�{�b�N�X�������ƁA
    �ꕔ�ݒ肪�ύX�㎩����Apply����܂�)

6. ��͂��낢��G���Ċo���Ă��������B



## �o�[�W�����A�b�v���@
1. �G���R�[�_�����GUI�N���C�A���g(scff-app.exe)�����s����Ă��Ȃ����Ƃ��m�F���ĉ������B

2. �m�F��A�t�@�C�����㏑�����Ă��������B



## �A���C���X�g�[�����@
1. �G���R�[�_�����GUI�N���C�A���g(scff-app.exe)�����s����Ă��Ȃ����Ƃ��m�F���ĉ������B

2. uninstall-*.bat�����s���Ă��������B

3. �t�H���_�E�t�@�C�����폜���Ă��������B
   * ���W�X�g���͎g�p���Ă��܂���̂ŁA���ꂾ���Ŋ��S�ɃA���C���X�g�[�����\�ł��B



## ����
*  *******************************************************************
   * �d�v�I                                                          *
   *******************************************************************
   ��荞�ݎ��ɖ�肪��������ƁA�����I�Ƀ��S���\������܂��B
   �����Ă��̏ꍇ��GUI�N���C�A���g�Őݒ��ς���Apply���Ȃ����ƒ���܂��B

* ���ݔ������Ă�����Ƃ��āA�傫�Ȏ�荞�ݗ̈�(1920x1050)��
  32x32���x�܂ŏ������k�����悤�Ƃ���Ǝ�荞�݂Ɏ��s���܂��B

* ���xApply���Ă�����Ȃ��ꍇ�́A�v���Z�X���j���[��Refresh����
  �G���R�[�_���N�����Ă��邩�ǂ����m�F���Ă��������B

* ����ł�����Ȃ��ꍇ��SCFF DirectShow Filter�ɑΉ����Ă��Ȃ����̉\��������܂��B
  * �ȉ���Web�y�[�W��"Issue"�Ɋ��̏ڍׂ���������ł���������Ώ�����܂��B
  * https://github.com/Alalf/SCFF-DirectShow-Filter



## �J���Ҍ���: �r���h+���p���@

1. http://ffmpeg.zeranoe.com/builds/ ����Shared�r���h�y��Dev�r���h���擾����
   * ext/ffmpeg/amd64��64bit�ł��Aext/ffmpeg/x86��32bit�ł�W�J����
   * Shared��Dev�������f�B���N�g���ɓW�J���邱�Ɓi���̃t�@�C�����㏑������邪���Ȃ��j
   * ext/ffmpeg/amd64/README.txt, ext/ffmpeg/x86/README.txt�����݂���悤�Ɋm�F���邱��
2. (�d�v�I) ext/ffmpeg/amd64/include/libavutil/pixdesc.h�����
   ext/ffmpeg/x86/include/libavutil/pixdesc.h�̈ȉ��̕���:
> extern const AVPixFmtDescriptor av_pix_fmt_descriptors[];
   ��
> extern __declspec(dllimport) const AVPixFmtDescriptor av_pix_fmt_descriptors[];
   �̂悤�ɏ��������Ă��������B�ꉞtools/pixdesc.patch���Y�t���Ă���܂��B
3. scff-dsf.sln�\�����[�V�������J���A�S�Ẵr���h���ʂ邱�Ƃ��m�F
   * Microsoft Visual C++ 2010 Express Edition + Windows SDK 7.1�Ŋm�F�ς�
   * �K�v�Ȃ�΂������̃v���W�F�N�g�ݒ�����������邱��
4. tools/copy-binaries.bat�����s����dist�f�B���N�g����dll�Ȃǂ��R�s�[
5. tools/install-debug.bat��tools/install-release.bat�����s
6. �e��G���R�[�_���N�����t�B���^���F������Ă��邩�`�F�b�N
7. scff-app.sln�\�����[�V�������J���A�S�Ẵr���h���ʂ邱�Ƃ��m�F
   * Microsoft Visual C# 2010 Express Edition�Ŋm�F�ς�
8. dist/Debug��dist/Release�ɂ���w���p�[�A�v���P�[�V�������N������荞�ݐݒ���s��
9. �iscff-dsf�̃f�o�b�O�o�[�W�����𗘗p����ꍇ:�j
   * �v���W�F�N�g�ݒ肩�烍�[�J��Windows�f�o�b�K�[��I��
   * �R�}���h��WME/KTE/FME�Ȃǂ�I������΃f�o�b�O������Ȃǂ����邱�Ƃ��o����B



## �J���Ҍ���: �u�J���ɎQ���������I�v

* ���݁ASCFF DirectShow Filter��GitHub��ŊJ�����i�߂��Ă��܂��B
  * https://github.com/Alalf/SCFF-DirectShow-Filter

* �p�b�`���쐬�������ꍇ��R�[�h��ǉ��������ꍇ�A�܂�Google C++�X�^�C���K�C�h����ǂ��Ă��������B
  * Google C++�X�^�C���K�C�h ���{���
    * http://www.textdrop.net/google-styleguide-ja/cppguide.xml
  * ���̃K�C�h�͒P���Ɍ��ߎ��ł͂Ȃ��A�o�O�����炷���߂ɖ��ɗ��e�N�j�b�N�������炩�܂܂�Ă���悤�ł��B

* scff-dsf�ɂ�doxygen�R�����g�����Ă���܂�
  * Doxygen(http://www.stack.nl/~dimitri/doxygen/index.html)
    * �v���O�����̑S�̓I�ȍ\����c���������ꍇ�͂��З��p���Ă݂Ă��������B



## �e��G���R�[�_�[�Ή����

### Windows Media Encoder etc.
* YUV420P(I420)�ɉ�����RGB 32(RGB0),YUV422(UYVY)�t�H�[�}�b�g�o�͂����p�\�ł��B
  * �i�b��Ή��Ȃ̂�YUV422(UYVY)�͂��Ȃ�@�\����������܂��j
  *  WME�̃v���p�e�B > ���� > �r�f�I > �s�N�Z���̌`������uRGB 32�v�uUYVY�v��I�����邱�Ƃŗ��p�ł��܂��B

### KoToEncoder(KTE)
* KTE��YUV420P�o�͂𗘗p����ꍇ�Ascff-dsf/base/constants.h�̈ȉ��̕����F
> // #define FOR_KOTOENCODER
  ��
> #define FOR_KOTOENCODER
  �̂悤�ɏ��������Ă��������B
* KTE��RGB32�o�͂��T�|�[�g�����t�B���^�𗘗p����ꍇ�A�t�B���^�̏o�͂�RGB32�ɌŒ肷��悤�ł��B
  * ���������ʂ͑����܂����AYUV422�����p�\�ł��B
    * �ڂ����̓\�����[�V������������"FOR_KOTOENCODER"�Ō������Ă݂Ă��������B
* KoToEncoder�̃v���r���[�@�\��I420�o�͗��p���ɂ��܂������Ȃ��悤�ł��B
  * �o�̓T�C�Y��ݒ肵������KTE���ċN������΃v���r���[���\�������悤�ɂȂ�܂��B



## �֘A����\�t�g�E�F�A�A�\�[�X�R�[�h�ɂ���

* DirectShow base classes - efines class hierarchy for streams architecture.
  * Copyright (c) 1992-2001 Microsoft Corporation.  All rights reserved.
* ISO C9x  compliant inttypes.h for Microsoft Visual Studio
  * Copyright (c) 2006 Alexander Chemeris

* ffmpeg�v���W�F�N�g(http://ffmpeg.org)
  * ���p���Ă���LGPL���C�u����:
    * libavutil: a library containing functions for simplifying programming, including random number generators, data structures, mathematics routines, core multimedia utilities, and much more.
    * libavcodec: a library containing decoders and encoders for audio/video codecs.
    * libswscale: a library performing highly optimized image scaling and color space/pixel format conversion operations.
    * libavfilter: a library containing media filters.
    * libavformat: a library containing demuxers and muxers for multimedia container formats.
    * libswresample: a library performing highly optimized audio resampling, rematrixing and sample format conversion operations.
  * ���p���Ă���LGPL���C�Z���X�̃\�[�X�R�[�h
    * libavutil/colorspace.h
    * libavfilter/drawutils.c
    * libavfilter/drawutils.h



## ����
* SCFF DirectShow Filter��"�t���[�\�t�g�E�F�A"�ł��B
* ��҂͖{�\�t�g�E�F�A�Ɋւ����؂̋`���i�T�|�[�g�A�P�v�I�A�b�v�f�[�g�j�������܂���B
* �܂��A�{�\�t�g�E�F�A�̎g�p�ɂ�萶�������ړI�A�ԐړI���Q�Ɉ�؂̐ӔC�������܂���B
* �{�\�t�g�E�F�A�̗��p�ɂ��Ă�LICENSE(LGPLv.3�̏ڍ�)���Q�Ƃ��Ă��������B

https://github.com/Alalf/SCFF-DirectShow-Filter
Copyright (C) 2012 Alalf