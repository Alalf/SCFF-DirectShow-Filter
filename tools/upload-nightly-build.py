#----------------------------------------------------------------------
# アーカイブ作成
#----------------------------------------------------------------------
import shutil
import datetime
import os.path

# 定数
k32bitDir = 'SCFF-DirectShow-Filter-x86'
k64bitDir = 'SCFF-DirectShow-Filter-amd64'

kToday = datetime.datetime.today()
kTimestamp = kToday.strftime("%Y%m%d-%H%M%S")

# パッケージ作成
path_32bit = shutil.make_archive('..\\package\\' + k32bitDir + "-" + kTimestamp, 'zip', '..\\package\\' + k32bitDir)
path_64bit = shutil.make_archive('..\\package\\' + k64bitDir + "-" + kTimestamp, 'zip', '..\\package\\' + k64bitDir)

# ファイル名とサイズを取得
filename_32bit = os.path.split(path_32bit)[1]
filesize_32bit = os.path.getsize(path_32bit)
filename_64bit = os.path.split(path_64bit)[1]
filesize_64bit = os.path.getsize(path_64bit)

print filename_32bit
print filesize_32bit
print filename_64bit
print filesize_64bit

#----------------------------------------------------------------------
# GitHubにアップロード
#----------------------------------------------------------------------

import requests
import json
import time
import calendar

# 定数

kUserName = 'Alalf'
kPassword = raw_input('GitHub Password: ')
kAuth = (kUserName, kPassword)

kDownloads = 'https://api.github.com/repos/Alalf/SCFF-DirectShow-Filter/downloads'

# ダウンロードリスト取得
#response = requests.get(kDownloads, auth=kAuth)
#if response.ok:
#	data = response.json
#	print json.dumps(data, indent=2)
#	# あとはもうdataから直接値を取り出せる
#	# data[0]["url"] etc.

# 参考:
# http://docs.python-requests.org/en/latest/user/quickstart/#more-complicated-post-requests

# アップロード準備
headers = {'content-type':'application/json', 'accept':'application/json'}
payload = {'name':filename_32bit, 'size':filesize_32bit}
response = requests.post(kDownloads, auth=kAuth, headers=headers, data=json.dumps(payload))
if not response.ok:
	print "Create download error!"
	quit()
data = response.json
print json.dumps(data, indent=2)

# Expiresを計算
expires = calendar.timegm(time.strptime(data['expirationdate'],'%Y-%m-%dT%H:%M:%S.000Z'))

# 実際にファイルをアップロード
payload = [('key',data['path']), ('acl',data['acl']), ('success_action_status',201), ('Filename',data['name']), ('AWSAccessKeyId',data['accesskeyid']), ('Policy',data['policy']), ('Signature',data['signature']), ('Expires',expires), ('Content-Type',data['mime_type'])]
#print payload
files = {'file': (filename_32bit, open(path_32bit, 'rb'))}
response = requests.post(data['s3_url'], data=payload, files=files)
print response.content

if not response.ok:
	print "Create download error!"
	quit()

print 'ALL OK'