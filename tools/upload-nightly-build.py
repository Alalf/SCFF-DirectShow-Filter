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
