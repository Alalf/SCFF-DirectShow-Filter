import shutil
import datetime

# 定数
k32bitDir = 'SCFF-x86'
k64bitDir = 'SCFF-amd64'

kToday = datetime.datetime.today()
kTimestamp = kToday.strftime("%Y%m%d-%H%M%S")

# パッケージ作成
shutil.make_archive('..\\package\\' + k32bitDir + "-" + kTimestamp, 'zip', '..\\package\\' + k32bitDir)
shutil.make_archive('..\\package\\' + k64bitDir + "-" + kTimestamp, 'zip', '..\\package\\' + k64bitDir)

# GitHubにアクセス
#
# GitHub API v3勉強の記録
# ---------------------------------------------------------------------
# 1. パスワードを毎回打ち込むのがめんどくさければOAuth2 Tokenを作る
#   curl -u "Alalf" -i -H "Accept: application/json" -H "Content-Type: application/json" -X POST -d "{\"scopes\":[\"SCFF-DirectShow-Filter\"],\"note\":\"upload-nightly-build.py\"}" https://api.github.com/authorizations
# 返ってきたメッセージのTokenを使えば以降URLにパスワード入力なしでアクセス可能:
#   https://api.github.com?access_token=TOKEN
# 言うまでも無いが、このTokenはパスワードと同じぐらい注意して取り扱うこと。
# どうみてもアクセス制限が全く無い！なおTokenを忘れたときは:
#   curl -u "Alalf" -i https://api.github.com/authorizations
#
# 2. Downloadsのリストを取ってきてみる
#   curl -u "Alalf" https://api.github.com/repos/Alalf/SCFF-DirectShow-Filter/downloads
# のちのち、必要の無くなったビルドを消していく予定
#
# 3. まずはリソースを作成する
#   curl -u "Alalf" -i -H "Accept: application/json" -H "Content-Type: application/json" -X POST -d "{\"name\":\"test.zip\",\"size\":118}" https://api.github.com/repos/Alalf/SCFF-DirectShow-Filter/downloads
# このレスポンスの内容から、アップロードに必要な情報を抜き出す
#
# 4. 内容をアップロードする
#   curl -u "Alalf" -i -H "Accept: application/json" -H "Content-Type: application/json" -F "key=downloads/Alalf/SCFF-DirectShow-Filter/test.zip" -F "acl=public-read" -F "success_action_status=201" -F "Filename=test.zip" -F "AWSAccessKeyId=XXX" -F "Policy=XXX" -F "Signature=XXX" -F "Content-Type=application/zip" -F "file=@D:\\Private\\Desktop\\test.zip" https://github.s3.amazonaws.com/
# こんな感じかな？まだ動かないので要調査
# それにしてもめんどくさいなー