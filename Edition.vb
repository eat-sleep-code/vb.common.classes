Imports System.IO


Public Class Edition

	Public Function GetEdition(ByVal file_name As String) As String
		file_name = file_name.Replace("~/", "")
		file_name = HttpContext.Current.Server.MapPath(file_name)
				
		Try
			'HttpContext.Current.Response.Write(file_name)
			Dim edition_date As DateTime = System.IO.File.GetLastWriteTime(file_name)
			Dim edition_text As String = "Last Modified: " + edition_date.ToString
			
			Return edition_text
		Catch
			Return "" 
		End Try
		
	End Function
    
End Class
