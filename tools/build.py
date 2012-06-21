# SCFF-DirectShow-Filter Build Script
#======================================================================

import subprocess
import sys

# 32bit版dsfのビルド
try:
    retcode = subprocess.call('build-x86.bat')
    if retcode < 0:
        print >>sys.stderr, "[x86-build] FAILED!", -retcode
        sys.exit()
    else:
        print >>sys.stderr, "[x86-build] SUCCESS!", retcode
except OSError, e:
    print >>sys.stderr, "[x86-build] Execution failed:", e
    sys.exit()

# 64bit版dsfのビルド
try:
    retcode = subprocess.call('build-amd64.bat')
    if retcode < 0:
        print >>sys.stderr, "[amd64-build] FAILED!", -retcode
        sys.exit()
    else:
        print >>sys.stderr, "[amd64-build] SUCCESS!", retcode
except OSError, e:
    print >>sys.stderr, "[amd64-build] Execution failed:", e
    sys.exit()

# appのビルド
try:
    retcode = subprocess.call('build-app.bat')
    if retcode < 0:
        print >>sys.stderr, "[app-build] FAILED!", -retcode
        sys.exit()
    else:
        print >>sys.stderr, "[app-build] SUCCESS!", retcode
except OSError, e:
    print >>sys.stderr, "[app-build] Execution failed:", e
    sys.exit()

# 
