Imports Microsoft.AspNet.FriendlyUrls
Imports System.IO
Imports System.Threading
Imports System.Web
Imports System.Web.UI


Public Class Localization

	Public Function LoadMasterPage(masterPagePath As String) As String
		Dim localIdentifier As String = Thread.CurrentThread.CurrentUICulture.Name
		Dim masterPage As String = String.Empty
		If localIdentifier <> "en-US" And localIdentifier.Length > 0 Then
			localIdentifier = "." & localIdentifier
		End If
		'''/ CHECK TO SEE WHAT SKINS EXIST AND APPLY CORRECT SKIN
		Dim localizedSkinExists As Boolean = File.Exists(HttpContext.Current.Server.MapPath(masterPagePath.Replace(".master", localIdentifier & ".master")))
		If localizedSkinExists = True Then
			masterPage = masterPagePath.Replace(".master", localIdentifier & ".master")
		Else
			masterPage = masterPagePath
		End If
		' HttpContext.Current.Response.Write(masterPage + "<br />")
		Return masterPage
	End Function




	Public Function LocalizeText(CurrentPage As Page, resourceKey As String) As String
		Dim localizedText As String = String.Empty

		' LOOK FOR LOCALIZED TEXT
		Dim filePath As [String] = String.Empty
		If Not [String].IsNullOrEmpty(CurrentPage.Request.GetFriendlyUrlFileVirtualPath()) Then
				' FOR FRIENDLY URLS
			filePath = CurrentPage.Request.GetFriendlyUrlFileVirtualPath()
		Else
				' FOR "UNFRIENDLY" URLS (THOSE WITH A FILE EXTENSION VISIBLE)
			filePath = CurrentPage.Request.CurrentExecutionFilePath
		End If

		Try
			localizedText = Convert.ToString(HttpContext.GetLocalResourceObject(filePath, resourceKey, System.Globalization.CultureInfo.CurrentCulture)).Trim()
		Catch
				'HttpContext.Current.Response.Write(ex.ToString() + "<br />" + filePath);
			'(Exception ex)
		End Try

		If PublishContent(Convert.ToString(HttpContext.GetLocalResourceObject(filePath, resourceKey & ".PublishDate", System.Globalization.CultureInfo.CurrentCulture)).Trim(), Convert.ToString(HttpContext.GetLocalResourceObject(filePath, resourceKey & ".ExpirationDate", System.Globalization.CultureInfo.CurrentCulture)).Trim()) = True Then
			Return localizedText
		Else
			Return String.Empty
		End If
	End Function



	Public Function PublishContent(publishDateString As String, expirationDateString As String) As [Boolean]

		' HANDLE DATES FOR NAVIGATION MENU INCLUSION
		Dim CurrentDate As DateTime = DateTime.Now
		Dim publishDate As New DateTime()
		Dim expirationDate As New DateTime()

		' SET PUBLISH DATE, IF STRING IS EMPTY OR CAN'T BE CONVERTED TO DATE TIME SET TO MINIMUM DATE
		Try
			publishDate = Convert.ToDateTime(publishDateString)
		Catch
			publishDate = Convert.ToDateTime(DateTime.MinValue)
		End Try

		' SET PUBLISH DATE, IF STRING IS EMPTY OR CAN'T BE CONVERTED TO DATE TIME SET TO MAXIMUM DATE, WATCH OUT FOR THE Y10K BUG! :-) 
		Try
			expirationDate = Convert.ToDateTime(expirationDateString)
		Catch
			expirationDate = Convert.ToDateTime(DateTime.MaxValue)
		End Try

		If CurrentDate.Ticks > publishDate.Ticks AndAlso CurrentDate.Ticks < expirationDate.Ticks Then
			Return True
		Else
			Return False
		End If
	End Function


End Class