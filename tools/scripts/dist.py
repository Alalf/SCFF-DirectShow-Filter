# dist.py
#======================================================================

# config

# TMP_DIR
# DIST_DIR
# DLLS_32BIT_DIR
# DLLS_64BIT_DIR
# FILES
# DLLS_32BIT
# DLLS_64BIT
# ARCHIVE_COMMAND = BIN_DIR + '\\7zr.exe'
# ARCHIVE_OPTIONS = 'a'

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
    
    makedirs(DIST_DIR)
    for path in FILES:
        for f in glob(path):
            copy(f, DIST_DIR)
    
    makedirs(DLLS_32BIT_DIR)    
    for path in DLLS_32BIT:
        for f in glob(path):
            copy(f, DLLS_32BIT_DIR)

    makedirs(DLLS_64BIT_DIR)
    for path in DLLS_64BIT:
        for f in glob(path):
            copy(f, DLLS_64BIT_DIR)

#----------------------------------------------------------------------

def make_archive():
    from sys import stderr
    from datetime import datetime
    from subprocess import call

    print >>stderr, 'make_archive:'
    
    today = datetime.today()
    timestamp = today.strftime('%Y%m%d-%H%M%S')

    archive = TMP_DIR + '\\%s-%s' % (BASENAME_DIR, timestamp)
    command = '"%s" %s "%s" "%s"' % (ARCHIVE_COMMAND, ARCHIVE_OPTIONS, archive + '.7z', DIST_DIR)
    call(command)