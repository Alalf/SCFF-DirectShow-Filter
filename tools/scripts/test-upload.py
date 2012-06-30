import requests
import json
import subprocess

# main
print 'test-upload.py'

kUserName = 'Alalf'
kPassword = raw_input('GitHub Password: ')
kAuth = (kUserName, kPassword)
kDownloads = 'https://api.github.com/repos/Alalf/SCFF-DirectShow-Filter/downloads'
kFilename = 'test.zip'
kFilesize = 118
kFilepath = 'test.zip'

# get-downloads

print '[get-downloads]'
headers = {'content-type':'application/json', 'accept':'application/json'}
response = requests.get(kDownloads, headers=headers)
data = response.json
match = [x for x in data if x['name'] == 'test.zip']
print json.dumps(match, indent=2)
if len(match) == 1:
    file_id = match[0]['id']
    print "DELETING: " + str(file_id)
    requests.delete(kDownloads + '/' + str(file_id), auth=kAuth, headers=headers)

# create-downloads-resource
print '[create-downloads-resource]'

headers = {'content-type':'application/json', 'accept':'application/json'}
payload = {'name':kFilename, 'size':kFilesize}
response = requests.post(kDownloads, auth=kAuth, headers=headers, data=json.dumps(payload))
data = response.json
print json.dumps(data, indent=2)

# upload-file-to-s3
print '[upload-file-to-s3]'

curl_params = []
curl_params.append(('key',data['path']))
curl_params.append(('acl',data['acl']))
curl_params.append(('success_action_status',201))
curl_params.append(('Filename',data['name']))
curl_params.append(('AWSAccessKeyId',data['accesskeyid']))
curl_params.append(('Policy',data['policy']))
curl_params.append(('Signature',data['signature']))
curl_params.append(('Content-Type',data['mime_type']))
curl_params.append(('file','@'+kFilepath))

curl_command = 'curl.exe -v'
for k, v in curl_params:
    curl_command += ' -F"' + k +'=' + str(v) + '"'
curl_command += ' ' + data['s3_url']

subprocess.call(curl_command)
