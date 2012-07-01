# dist.py
#======================================================================

# config

# TMP_DIR

#----------------------------------------------------------------------

def init():
    from sys import stderr
    from os import makedirs
    from shutil import rmtree

    print >>stderr, 'init:'
    
    rmtree(TMP_DIR, True)
    makedirs(TMP_DIR)

#----------------------------------------------------------------------

def make_dist():
    from sys import stderr
    from os import makedirs
    from glob import glob
    from shutil import copy

    print >>stderr, 'make_dist:'
    
    makedirs(DIST_32BIT_DIR)
    for path in FILES_32BIT:
        for f in glob(path):
            copy(f, DIST_32BIT_DIR)

    makedirs(DIST_64BIT_DIR)
    for path in FILES_64BIT:
        for f in glob(path):
            copy(f, DIST_64BIT_DIR)

#----------------------------------------------------------------------

def make_archive():
    from sys import stderr
    from datetime import datetime
    from subprocess import call

    print >>stderr, 'make_archive:'
    
    today = datetime.today()
    timestamp = today.strftime('%Y%m%d-%H%M%S')

    archive_32bit = TMP_DIR + '\\%s-%s' % (BASENAME_32BIT_DIR, timestamp)
    archive_64bit = TMP_DIR + '\\%s-%s' % (BASENAME_64BIT_DIR, timestamp)
    
    command_32bit = '"%s" %s "%s" "%s"' % (ARCHIVE_COMMAND, ARCHIVE_OPTIONS, archive_32bit + '.7z', DIST_32BIT_DIR)
    command_64bit = '"%s" %s "%s" "%s"' % (ARCHIVE_COMMAND, ARCHIVE_OPTIONS, archive_64bit + '.7z', DIST_64BIT_DIR)
    call(command_32bit)
    call(command_64bit)