#!/usr/bin/env python

import os
import sys

reload(sys)
sys.setdefaultencoding('utf-8')

# dir
EXCEL_DIR = './'
JSON_DIR = '../U/Assets/Resources/Json/'
CSHARP_DIR = '../U/Assets/Scripts/Project/E2J/AutoGen/'

# global
ALL_XLSX = {} # key: xlsx name, value: file path

if __name__ == '__main__':
	if len(sys.argv) >= 2:
		bGenCode = int(sys.argv[1])

def LoadAllXLSX(dir):
	global ALL_XLSX
	ALL_XLSX.clear()
	for fp in os.listdir(dir):
		filePath = os.path.join(dir, fp)
		if os.path.isdir(filePath):
			continue
		fpName, fpExt = os.path.splitext(fp)
		if fpName.startswith('E2J') and fpExt == '.xlsx':
			if not ALL_XLSX.has_key(fpName):
				ALL_XLSX[fpName] = fp

def ConvertKeyType(type):
    if type == 'Int':
        return 'int'
    elif type == 'String':
        return 'string'
    elif type == 'Float':
        return 'float'
    return ''