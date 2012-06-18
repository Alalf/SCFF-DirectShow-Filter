import shutil

# 定数
k32bitDir = 'SCFF-x86'
k64bitDir = 'SCFF-amd64'

# パッケージ作成
shutil.make_archive('..\\package\\' + k32bitDir, 'zip', '..\\package\\' + k32bitDir)
shutil.make_archive('..\\package\\' + k64bitDir, 'zip', '..\\package\\' + k64bitDir)

# GitHubにアクセス
