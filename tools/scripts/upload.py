# upload.py
#======================================================================

# config

# TMP_DIR
# ARCHIVES
# UPLOAD_COMMAND
# UPLOAD_OPTIONS
# AUTH
# DOWNLOADS_URL

#----------------------------------------------------------------------

def init():
    from sys import stderr
    from os import makedirs
    from shutil import rmtree

    print >>stderr, 'init:'

    rmtree(TMP_DIR, True)
    makedirs(TMP_DIR)

#----------------------------------------------------------------------

def upload():
    from sys import stderr
    from requests import get
    from requests import post
    from requests import delete
    from subprocess import call
    from glob import glob
    from json import dumps
    from os.path import split
    from os.path import getsize

    print >>stderr, 'upload:'

    # common
    headers = {'content-type':'application/json', 'accept':'application/json'}

    # get_downloads
    print >>stderr, '\t[get_downloads]'
    response = get(DOWNLOADS_URL, headers=headers)
    data = response.json

    # TODO(me): 古くなったファイルは削除する
    for i in data:
        for archive in glob(ARCHIVES):
            filename = split(archive)[1]
            if i['name'] == filename:
                print >>stderr, '\t[delete] %s(%s)' % (filename, i['id'])
                url = DOWNLOADS_URL + '/' + str(i['id'])
                delete(url, auth=AUTH, headers=headers)

    for archive in glob(ARCHIVES):
        # common
        dist_dir = split(archive)[0]
        filename = split(archive)[1]
        filesize = getsize(archive)

        # create_downloads_resource
        print >>stderr, '\t[create_downloads_resource] ' + filename
        payload = {'name':filename, 'size':filesize}
        response = post(DOWNLOADS_URL, auth=AUTH, headers=headers, data=dumps(payload))
        data = response.json

        # upload_file_to_s3
        print >>stderr, '\t[upload_file_to_s3] ' + filename

        params = [
            ('key', data['path']),
            ('acl', data['acl']),
            ('success_action_status', 201),
            ('Filename', data['name']),
            ('AWSAccessKeyId', data['accesskeyid']),
            ('Policy', data['policy']),
            ('Signature', data['signature']),
            ('Content-Type', data['mime_type']),
            ('file', '@' + archive)]

        command = '"%s" %s' % (UPLOAD_COMMAND, UPLOAD_OPTIONS)
        for k, v in params:
            command += ' -F"' + k +'=' + str(v) + '"'
        command += ' ' + data['s3_url']

        call(command)
