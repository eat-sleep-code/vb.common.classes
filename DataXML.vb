Imports System.Collections
Imports System.Data
Imports System.IO
Imports System.Xml

Public Class DataXML

	Public Function GetDataTable(xmlFile As String, topLevelElement As String, Optional filterType As String = "", Optional filterValue As String = "") As DataTable
		'// CHECK TO SEE IF LOCALIZED XML FILE EXISTS, IF SO USE THAT INSTEAD
		Dim currentCulture As String = System.Globalization.CultureInfo.CurrentCulture.ToString().ToLower()
		If (currentCulture <> "en-us" And Not [String].IsNullOrEmpty(currentCulture)) Then
			Dim currentFileNameLength As Integer = xmlFile.Trim().Length
			Dim localizedXmlFile As String = xmlFile.Substring(0, currentFileNameLength - 4) & "." & currentCulture & ".xml"
			If File.Exists(localizedXmlFile) = True Then
				xmlFile = localizedXmlFile
			End If
		End If

		topLevelElement = topLevelElement.Trim()
		Dim xmlReaderSettings As New XmlReaderSettings()
		Dim xmlReader__1 As XmlReader = XmlReader.Create(xmlFile, xmlReaderSettings)
		xmlReader__1.ReadToFollowing(topLevelElement)

		'// CREATE A DATATABLE TO CONTAINS ONLY THE DATE WE NEED
		Dim dataTableXml As New DataTable()
		Dim dataColumnID As New DataColumn("id", GetType(Integer))
		dataTableXml.Columns.Add(dataColumnID)


		Dim tempID As Integer = 0
		'// CREATE A HASHTABLE, TO HOLD TEMPORARY VALUES AS WE WORK THROUGH THE XML FILE
		Dim hashtable As New System.Collections.Hashtable()

		'// READ THROUGH THE XML DATASOURCE AND ADD TITLES AND DESCRIPTIONS TO DATATABLE	
		While xmlReader__1.Read()
			If xmlReader__1.NodeType <> XmlNodeType.Whitespace Then
				Try
					Dim dataRow As DataRow = dataTableXml.NewRow()


					If xmlReader__1.NodeType = XmlNodeType.EndElement And xmlReader__1.LocalName.ToString() = topLevelElement Then
						'// REACHED THE END OF THE PARENT ELEMENT, ADD THE ROW TO THE DATATABLE
						tempID = tempID + 1
						'// PULL TEMPORARY VALUES FROM HASHTABLE
								'// DO NOTHING, RECORD SHOULD BE FILTERED OUT
						If (filterType = "id" And filterValue <> tempID.ToString()) Then
						Else
							Dim isFilter As Boolean = False
							For Each dictionaryEntry As DictionaryEntry In hashtable
								If filterType = dictionaryEntry.Key.ToString() And filterValue <> dictionaryEntry.Value.ToString() Then
									isFilter = True
									Exit For
								Else
									dataRow(dictionaryEntry.Key.ToString()) = dictionaryEntry.Value.ToString()
								End If
							Next
							If isFilter = False Then
								dataRow("id") = tempID
								dataTableXml.Rows.Add(dataRow)
							End If
						End If
						'// EMPTY OUT THE HASHTABLE

						hashtable.Clear()
					ElseIf xmlReader__1.NodeType <> XmlNodeType.EndElement And xmlReader__1.LocalName.ToString() <> topLevelElement Then
						'// IF FIRST TIME THIS ELEMENT ENCOUNTERED, ADD IT TO THE DATABASE COLUMN
						If dataTableXml.Columns.Contains(xmlReader__1.LocalName.ToString()) = False Then
							Dim dataColumn As New DataColumn(xmlReader__1.LocalName.ToString(), GetType(String))
							dataTableXml.Columns.Add(dataColumn)
							dataColumn = Nothing
						End If

						'Current.Response.Write("<B>tempID " + xmlReader.LocalName.ToString + "(" + xmlReader.NodeType.ToString + ")" + ": </B>")
						If xmlReader__1.NodeType <> XmlNodeType.CDATA Then
							'Current.Response.Write("<I>" + xmlReader.ReadElementContentAsString.ToString + "</I><br />")
							'dataRow(xmlReader.LocalName.ToString) = xmlReader.ReadElementContentAsString.ToString
							hashtable.Add(xmlReader__1.LocalName.ToString(), xmlReader__1.ReadElementContentAsString().ToString())
						End If
					End If
				Catch
						'// SUPPRESS ERRORS
					'(Exception ex)
				End Try
			End If
		End While

		xmlReader__1.Close()
		Return dataTableXml

	End Function

End Class
