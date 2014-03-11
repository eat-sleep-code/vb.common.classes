Namespace components

Public Class shared_functions

	Public Function to_proper(strText As String) As String
		to_proper = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(strText)
	End Function
	    
End Class



End NameSpace


