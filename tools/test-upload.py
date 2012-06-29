
import requests
import json

kUserName = 'Alalf'
kPassword = raw_input('GitHub Password: ')
kAuth = (kUserName, kPassword)
kDownloads = 'https://api.github.com/repos/Alalf/SCFF-DirectShow-Filter/downloads'
kFilename = 'test.zip'
kFilesize = 118
kFilepath = 'test.zip'

# create download
headers = {'content-type':'application/json', 'accept':'application/json'}
payload = {'name':kFilename, 'size':kFilesize}
response = requests.post(kDownloads, auth=kAuth, headers=headers, data=json.dumps(payload))

data = response.json

print json.dumps(data, indent=2)

# upload
payload = []
payload.append(('key',data['path']))
payload.append(('acl',data['acl']))
payload.append(('success_action_status',201))
payload.append(('Filename',data['name']))
payload.append(('AWSAccessKeyId',data['accesskeyid']))
payload.append(('Policy',data['policy']))
payload.append(('Signature',data['signature']))
payload.append(('Content-Type',data['mime_type']))
files = {'file': (kFilename, open(kFilepath, 'rb'))}
response = requests.post(data['s3_url'], data=payload, files=files)

print response.content
