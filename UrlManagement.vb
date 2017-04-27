Imports System.Globalization
Imports System.Linq
Imports System.Web
Imports System.Xml.Linq

Namespace framework
	Public Class UrlManagement

		Public Shared Function IsLocalizedUrl(url As Uri) As [Boolean]
			Dim subDomain As String = GetSubDomain(url).ToLower()
			If subDomain = "www" OrElse subDomain = "en-us" OrElse subDomain = "" Then
				Return False
			Else
				Return True
			End If
		End Function


		Public Shared Function GetSubDomain(url As Uri) As String
			Dim host As String = url.Host
			If host.Split(".").Length > 2 Then
				Dim lastIndex As Integer = host.LastIndexOf(".")
				Dim index As Integer = host.LastIndexOf(".", lastIndex - 1)
				Return host.Substring(0, index).ToLower()
			End If
			Return String.Empty
		End Function


		Public Shared Function GetCultureInfoFromUrl(url As Uri) As CultureInfo
			Dim subDomain As [String] = GetSubDomain(url)

			'TODO: ADD LOGIC TO MATCH SUBDOMAIN TO LOCAL IDENTIFIER IN XML FILE
			Dim xdoc As XDocument = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/localization.xml"))
			Dim cultureInfo As [String] = xdoc.Root.Elements("Language").Where(Function(i) i.Element("LocalIdentifier").ToString() = subDomain).[Select](Function(i) i.Element("CultureInfo").ToString()).FirstOrDefault()
			If [String].IsNullOrEmpty(cultureInfo) = True Then
				Return New CultureInfo("en-us")
			Else
				Return New CultureInfo(cultureInfo)
			End If
		End Function
	End Class
End Namespace
