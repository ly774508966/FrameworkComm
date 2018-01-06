#!/usr/bin/env python

import os
import sys
import xlrd
import json
import shutil

reload(sys)
sys.setdefaultencoding('utf-8')

# config
PROJECT_NAME = 'Project'
TABLELIST_NAME = 'E2JTableList.xlsx'
HASHHELPER_NAME = 'E2JHashHelper.cs'
EXCEL_HEADLINE = 3

# excel dir
EXCEL_DIR = './Excel/'

# export dir
JSON_DIR = '../U/Assets/Resources/Json/'
CSHARP_DIR = '../U/Assets/Scripts/Project/E2JAutoGen/'

# global
ALL_XLSX = {}  # key: xlsx name, value: file path
ALL_SHEET = {} # key: sheet name, value: sheet item

# class
class SheetColumn:
	def __init__(self):
		self.name = ''
		self.type = ''

	def SetName(self, name):
		self.name = name

	def SetType(self, type):
		self.type = type

class Sheet:
	def __init__(self, name, keyType):
		self.name = name
		self.keyType = keyType
		self.columns = {}
		self.datas = {}

	def GetOrCreateColumn(self, colIndex):
		if self.columns.has_key(colIndex):
			return self.columns[colIndex]
		else:
			sheetColumn = SheetColumn()
			self.columns[colIndex] = sheetColumn
			return sheetColumn

# function
def BKDRHash(string):
	seed = 131
	hash = 0
	for char in string:
		hash = (hash * seed + ord(char)) & 0x7FFFFFFF
	return hash & 0x7FFFFFFF

def ConvertKeyType(type):
	if type == 'Int':
		return 'int'
	elif type == 'String':
		return 'string'
	elif type == 'Float':
		return 'float'
	else:
		return ''

def ConvertTypeLoad(type, key):
	if type == 'int':
		return '            %s = E2JConvertHelper.O2I(ht["%s"]);\n' % (key, key)
	elif type == 'string':
		return '            %s = E2JConvertHelper.O2STrim(ht["%s"]);\n' % (key, key)
	elif type == 'float':
		return '            %s = E2JConvertHelper.O2F(ht["%s"]);\n' % (key, key)
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
			iVal = int(float(cell.value))
			fVal = float(cell.value)
			if fVal > (float)(iVal):
				str_value = str((float(cell.value))) + ''
			else:
				str_value = str(int(float(cell.value))) + ''
			return str(int(float(str_value)))
	elif type == 'Float':
		return float(cell.value)
	else:
		return ''

def RemoveAllJson():
	if not os.path.exists(JSON_DIR):
		os.makedirs(JSON_DIR)
	for root, dirs, files in os.walk(JSON_DIR):
		for file in files:
			if not file.endswith('.meta'):
				os.remove(os.path.join(root, file))

def RemoveAllCSharp(genCode):
	if not os.path.exists(CSHARP_DIR):
		os.makedirs(CSHARP_DIR)
	if genCode == 0:
		return
	for root, dirs, files in os.walk(CSHARP_DIR):
		for file in files:
			if not file.endswith('.meta'):
				os.remove(os.path.join(root, file))

def LoadAllXLSX():
	global ALL_XLSX
	ALL_XLSX.clear()
	for fp in os.listdir(EXCEL_DIR):
		if os.path.isdir(fp):
			continue
		fpName, fpExt = os.path.splitext(fp)
		if fpName.startswith('E2J') and fpExt == '.xlsx':
			if not ALL_XLSX.has_key(fpName):
				ALL_XLSX[fpName] = fp

def LoadAllSheetFromTableList():
	global ALL_SHEET
	ALL_SHEET.clear()
	workBook = xlrd.open_workbook(os.path.join(EXCEL_DIR, TABLELIST_NAME))
	workSheets = workBook.sheets()
	for st in workSheets:
		if st.name == os.path.splitext(TABLELIST_NAME)[0]:
			for rowIndex in range(st.nrows):
				if rowIndex < EXCEL_HEADLINE:
					continue
				sheetName = GetCellValue(st.cell(rowIndex, 0), 'String')
				keyType = GetCellValue(st.cell(rowIndex, 1), 'Int')
				if not ALL_SHEET.has_key(sheetName):
					ALL_SHEET[sheetName] = Sheet(sheetName, keyType)

def ParseOneSheetFromXLSX(xlsxPath):
	workBook = xlrd.open_workbook(os.path.join(EXCEL_DIR, xlsxPath))
	workSheets = workBook.sheets()
	for st in workSheets:
		sheetName = st.name
		if sheetName.startswith('E2J') and ALL_SHEET.has_key(sheetName):
			sheetItem = ALL_SHEET[sheetName]
			for rowIndex in range(st.nrows):
				if rowIndex == 0:
					for colIndex in range(st.ncols):
						keyType = GetCellValue(st.cell(rowIndex, colIndex), 'String')
						if ConvertKeyType(keyType) != '':
							sheetColumn = sheetItem.GetOrCreateColumn(colIndex)
							sheetColumn.SetType(keyType)
				elif rowIndex == 1:
					for colIndex in range(st.ncols):
						colName = GetCellValue(st.cell(rowIndex, colIndex), 'String')
						if colName != '':
							sheetColumn = sheetItem.GetOrCreateColumn(colIndex)
							sheetColumn.SetName(colName)
				elif rowIndex >= EXCEL_HEADLINE:
					rowKey = None
					rowValues = {}
					curColIndex = 0
					for index, column in sheetItem.columns.iteritems():
						cellValue = None
						if column.type != '':
							cellValue = GetCellValue(st.cell(rowIndex, curColIndex), column.type)
							rowValues[column.name] = cellValue
						if curColIndex == 0:
							rowKey = cellValue
						curColIndex += 1
					sheetItem.datas[rowKey] = rowValues

def ParseAllSheetFromXLSX():
	for xlsxName, xlsxPath in ALL_XLSX.iteritems():
		ParseOneSheetFromXLSX(xlsxPath)

def ExportSheetCSharp(sheetItem):
	code = \
'''//////////////////////////////////////////////////////////////////////////
/// This is an auto-generated script, please do not modify it manually ///
//////////////////////////////////////////////////////////////////////////

using System.Text;
using System.Collections;
using System.Collections.Generic;
using Framework;
'''
	code += \
'''
namespace %s
{
    public sealed class %s : E2JLoader
    {
''' % (PROJECT_NAME, sheetItem.name)
	code += '        public static readonly string E2JName = "%s";\n\n' % sheetItem.name
	for columnIndex, columnItem in sheetItem.columns.iteritems():
		columnType = ConvertKeyType(columnItem.type)
		if columnType == '':
			continue
		code += '        public %s %s { get; private set; }\n' % (columnType, columnItem.name)
	code += \
'''
        public void Load(Hashtable ht)
        {
'''
	for columnIndex, columnItem in sheetItem.columns.iteritems():
		columnType = ConvertKeyType(columnItem.type)
		if columnType == '':
			continue
		code += ConvertTypeLoad(columnType, columnItem.name)
	code += '        }\n'
	if sheetItem.keyType == 0:
		code += \
'''
        public static %s GetElement(int elementKey)
        {
            return E2JManager.instance.GetElementInt(E2JName, elementKey) as %s;
        }

        public static Dictionary<int, E2JLoader> GetElementTable()
        {
            return E2JManager.instance.GetTableInt(E2JName);
        }
''' % (sheetItem.name, sheetItem.name)
	elif sheetItem.keyType == 1:
		code += \
'''
        public static %s GetElement(string elementKey)
        {
            return E2JManager.instance.GetElementString(E2JName, elementKey) as %s;
        }

        public static Dictionary<string, E2JLoader> GetElementTable()
        {
            return E2JManager.instance.GetTableString(E2JName);
        }
''' % (sheetItem.name, sheetItem.name)
	code += \
'''
        public %s Clone()
        {
            %s clone = new %s();
''' % (sheetItem.name, sheetItem.name, sheetItem.name)
	for columnIndex, columnItem in sheetItem.columns.iteritems():
		columnType = ConvertKeyType(columnItem.type)
		if columnType == '':
			continue
		code += '            clone.%s = %s;\n' % (columnItem.name, columnItem.name)
	code += \
'''            return clone;
        }
'''
	code += \
'''
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(E2JName).Append("->");
            sb.AppendLine();
'''
	for columnIndex, columnItem in sheetItem.columns.iteritems():
		columnType = ConvertKeyType(columnItem.type)
		if columnType == '':
			continue
		code += '            sb.Append("%s: ").Append(%s);\n            sb.AppendLine();\n' % (columnItem.name, columnItem.name)
	code += \
'''            return sb.ToString();
        }
    }
}
'''
	csFileName = os.path.join(CSHARP_DIR, sheetItem.name + '.cs')
	if os.path.exists(csFileName):
		os.remove(csFileName)
	csFile = file(csFileName, 'w')
	csFile.writelines(code)
	csFile.close()

def ExportSheetCSharpHashHelper():
	code = \
'''//////////////////////////////////////////////////////////////////////////
/// This is an auto-generated script, please do not modify it manually ///
//////////////////////////////////////////////////////////////////////////

using Framework;
'''
	code += \
'''
namespace %s
{
    public static class E2JHashHelper
    {
        public static E2JLoader CreateLoaderByHash(uint hash)
        {
            E2JLoader loader = null;

            switch (hash)
            {
''' % PROJECT_NAME
	for sheetName, sheetItem in ALL_SHEET.iteritems():
		code += \
'''                case %s:
                    {
                        loader = new %s();
                    }
                    break;
''' % (BKDRHash(sheetName), sheetName)
	code += \
'''            }

            return loader;
        }
    }
}
'''
	hsFileName = os.path.join(CSHARP_DIR, HASHHELPER_NAME)
	if os.path.exists(hsFileName):
		os.remove(hsFileName)
	hsFile = file(hsFileName, 'w')
	hsFile.writelines(code)
	hsFile.close()

def ExportSheetJson(sheetItem):
	if len(sheetItem.datas) == 0:
		return
	if not os.path.exists(JSON_DIR):
		os.mkdir(JSON_DIR)
	jsonFile = file(os.path.join(JSON_DIR, "%s.txt" % (sheetItem.name)), 'w')
	json.dump(sheetItem.datas, jsonFile, indent = 2, sort_keys = True, ensure_ascii = False)
	jsonFile.close()

def ExportOneSheetFromCache(sheetName, genCode):
	if ALL_SHEET.has_key(sheetName):
		sheetItem = ALL_SHEET[sheetName]
		if genCode == 1:
			ExportSheetCSharp(sheetItem)
		ExportSheetJson(sheetItem)
		ExportSheetCSharpHashHelper()

def ExportAllSheetFromCache(genCode):
	for sheetName, sheetItem in ALL_SHEET.iteritems():
		if genCode == 1:
			ExportSheetCSharp(sheetItem)
		ExportSheetJson(sheetItem)
	ExportSheetCSharpHashHelper()

# main
if __name__ == '__main__':
	if len(sys.argv) >= 2:
		genCode = int(sys.argv[1])
		LoadAllXLSX()
		LoadAllSheetFromTableList()
		ParseAllSheetFromXLSX()
		RemoveAllJson()
		RemoveAllCSharp(genCode)
		ExportAllSheetFromCache(genCode)
	print('Excel2Json Done!')