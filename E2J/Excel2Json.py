#!/usr/bin/env python

import os
import sys
import xlrd
import json
import shutil

reload(sys)
sys.setdefaultencoding('utf-8')

# excel dir
WORK_DIR = './'
EXCEL_DIR = './Excel/'
META_DIR = './Meta/'

# export dir
JSON_DIR = '../U/Assets/Resources/Json/'
CSHARP_DIR = '../U/Assets/Scripts/Project/E2JAutoGen/'

# file path
TABLELIST_PATH = 'E2JTableList.xlsx'

# global
ALL_XLSX = {}  # key: xlsx name, value: file path
ALL_SHEET = {} # key: sheet name, value: sheet item

# config
EXCEL_HEADLINE = 3
GEN_CODE = 1   # 1 for generate code, 0 for not

# class
class Sheet:
	def __init__(self, name, keyType):
		self.name = name
		self.keyType = keyType
		self.columns = {}
		self.datas = {}

# function
def ConvertKeyType(type):
	if type == 'Int':
		return 'int'
	elif type == 'String':
		return 'string'
	elif type == 'Float':
		return 'float'
	else:
		return ''

def GetCellValue(cell, type):
	if cell == None or cell.value == None or cell.ctype not in (1, 2, 3):
		return ''
	if type == 'Int':
		return int(cell.value)
	elif type == 'String':
		if cell.ctype == 1:
			return str(cell.value)
		else:
			return str(int(float(cell.value)))
	elif type == 'Float':
		return float(cell.value)
	else:
		return ''

def RemoveAllJson():
	if(os.path.exists(META_DIR)):
		shutil.rmtree(META_DIR)
	os.makedirs(META_DIR)
	for root, dirs, files in os.walk(JSON_DIR):
		for file in files:
			path = os.path.join(root, file)
			if file.endswith('.meta'):
				shutil.copy2(path, os.path.join(META_DIR, file))
			else:
				os.remove(path)

def RemoveAllCSharp():
	if not os.path.exists(CSHARP_DIR):
		os.makedirs(CSHARP_DIR)
	if GEN_CODE == 0:
		return
	for root, dirs, files in os.walk(CSHARP_DIR):
		for file in files:
			if not file.endswith('.meta'):
				os.remove(os.path.join(root, file))

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

def LoadAllSheetFromTableList(path):
	global ALL_SHEET
	ALL_SHEET.clear()
	workBook = xlrd.open_workbook(os.path.join(WORK_DIR, path))
	workSheets = workBook.sheets()
	for st in workSheets:
		if st.name == os.path.splitext(path)[0]:
			for rowIndex in range(st.nrows):
				if rowIndex < EXCEL_HEADLINE:
					continue
				sheetName = GetCellValue(st.cell(rowIndex, 0), 'String')
				keyType = GetCellValue(st.cell(rowIndex, 1), 'Int')
				if not ALL_SHEET.has_key(sheetName):
					ALL_SHEET[sheetName] = Sheet(sheetName, keyType)

# main
if __name__ == '__main__':
	global GEN_CODE
	if len(sys.argv) >= 2:
		GEN_CODE = int(sys.argv[1])
		LoadAllXLSX(EXCEL_DIR)
		LoadAllSheetFromTableList(TABLELIST_PATH)
		#RemoveAllJson()
		#RemoveAllCSharp()
	print('Excel2Json Done!')