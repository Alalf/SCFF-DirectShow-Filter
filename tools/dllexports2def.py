# dllexports2def.py
# from http://takumakei.blogspot.jp/2009/04/dlldef.html
#======================================================================

def dllexports2def(path):
    from sys import stderr
    from os.path import exists
    if not exists(path):
        print >>stderr, 'dllexports2def: %s is not found' % path
        return

    from os.path import basename, splitext
    dll_basename = basename(path)
    dll_name = dll_basename.split('.')[0]
    max_name_length = 0
    exports = []
    with open(path) as f:
        from re import compile
        re_func = compile(r"^\s+(\d+)\s+[0-9A-Fa-f]+\s+[0-9A-Fa-f]+\s+(.*)$")

        for line in f:
            result = re_func.match(line)
            if result == None:
                continue
            ordinal = result.group(1)
            name = result.group(2)
            exports.append([ordinal, name])
            if max_name_length < len(name):
                max_name_length = len(name)

    width = ((max_name_length + 7) / 8) * 8
    print 'LIBRARY "%s.dll"' % dll_name
    print
    print 'EXPORTS'
    for ordinal, name in exports:
        tabbing = '\t' * ((width + 7 - len(name)) / 8)
        print '\t%s%s@%s' % (name, tabbing, ordinal)
    print >>stderr, 'dllexports2def: SUCCESS'

#----------------------------------------------------------------------

# main()
if __name__=='__main__':
    from sys import stderr, argv
    print >>stderr, '=== dllexports2def.py ===\n'

    if len(argv) != 2:
        print >>stderr, 'Usage: python %s filename' % argv[0]
    else:
        dllexports2def(argv[1])
