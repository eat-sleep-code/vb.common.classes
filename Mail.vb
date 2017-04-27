Imports System.Collections.Generic
Imports System.Configuration
Imports System.Net.Configuration
Imports System.Net.Mail
Imports System.Web.Configuration
Imports SendGrid
Imports SendGrid.Helpers.Mail



Public Class Mail

	Public Function Send(Optional recipient As String = "", Optional sender As String = "", Optional body As String = "", Optional subject As String = "", Optional cc As String = "", Optional bcc As String = "", Optional replyTo As String = "", Optional priority As String = "Normal", Optional smtpServer As String = "", Optional smtpPort As Integer = 25) As String
		'// == GET PRECONFIGURED SETTINGS == 
		Dim configuredSender As String = String.Empty
		Dim configuredHost As String = String.Empty
		Dim configuredPort As Integer = 25
		Dim configuredUsername As String = String.Empty
		Dim configuredPassword As String = String.Empty

		Try
			Dim webConfigFile As Configuration = WebConfigurationManager.OpenWebConfiguration("~")
			Dim mailSettings As MailSettingsSectionGroup = DirectCast(webConfigFile.GetSectionGroup("system.net/mailSettings"), MailSettingsSectionGroup)
			If (mailSettings IsNot Nothing) Then
				configuredSender = mailSettings.Smtp.From
				configuredHost = mailSettings.Smtp.Network.Host
				configuredPort = mailSettings.Smtp.Network.Port
				configuredUsername = mailSettings.Smtp.Network.UserName
				configuredPassword = mailSettings.Smtp.Network.Password
			End If
		Catch
			'(Exception ex) 
			'// IN DEFAULT MEDIUM TRUST ENVIRONMENTS (SUCH AS GODADDY) THE ABOVE MAY FAIL, LETS GO LOOK FOR AppSettings VALUES IN THE WEB.CONFIG - NOT IDEAL, BUT CurrentLY A LIMITATION OF ASP.NET FRAMEWORK
			configuredSender = WebConfigurationManager.AppSettings("mail_from")
			configuredHost = WebConfigurationManager.AppSettings("mail_host")
			configuredPort = Convert.ToInt32(WebConfigurationManager.AppSettings("mail_port"))
			configuredUsername = WebConfigurationManager.AppSettings("mail_username")
			configuredPassword = WebConfigurationManager.AppSettings("mail_password")
		End Try
		
		'// == CREATE MAIL OBJECTS ==
		Dim mailMessage As New MailMessage()
		Dim mailClient As New SmtpClient()
		Dim mailCredentials As New System.Net.NetworkCredential()

		'// === SET THE SENDER & REPLY TO ADDRESSES === 
		If sender.Trim().Length > 0 Then
			mailMessage.From = New MailAddress(sender.Trim())
			If replyTo.Trim().Length > 0 Then
				mailMessage.ReplyToList.Add(New MailAddress(replyTo.Trim()))
			Else
				mailMessage.ReplyToList.Add(New MailAddress(sender.Trim()))
			End If
		Else
			mailMessage.From = New MailAddress(configuredSender.Trim())
			If replyTo.Trim().Length > 0 Then
				mailMessage.ReplyToList.Add(New MailAddress(replyTo.Trim()))
			Else
				mailMessage.ReplyToList.Add(New MailAddress(configuredSender.Trim()))
			End If
		End If

		'// === SET THE RECIPIENTS ===
		If recipient.Trim().Length > 0 Then
			For Each recipientAddress As String In recipient.Trim().Split(";"c)
				If recipientAddress.Trim().Length >= 5 Then
					mailMessage.[To].Add(recipientAddress.Trim())
				End If
			Next
		Else
			mailMessage.[To].Add(New MailAddress(configuredSender.Trim()))
		End If

		For Each ccAddress As String In cc.Trim().Split(";"c)
			If ccAddress.Trim().Length >= 5 Then
				mailMessage.CC.Add(ccAddress.Trim())
			End If
		Next

		For Each bccAddress As String In bcc.Trim().Split(";"c)
			If bccAddress.Trim().Length >= 5 Then
				mailMessage.Bcc.Add(bccAddress.Trim())
			End If
		Next

		'// === SET THE SUBJECT ===
		mailMessage.Subject = subject.Trim()

		'// === SET THE MESSAGE BODY TEXT ===
		mailMessage.Body = body.Trim()

		'// === SET PRIORITY OF MESSAGE (DEFAULT IS NORMAL) ===
		If priority.Trim() = "High" Then
			mailMessage.Priority = MailPriority.High
		ElseIf priority.Trim() = "Low" Then
			mailMessage.Priority = MailPriority.Low
		Else
			mailMessage.Priority = MailPriority.Normal
		End If

		'// === SET FORMAT OF THE MAIL ===
		mailMessage.IsBodyHtml = True

		'// === SET THE SMTP SERVER ===
		If smtpServer.Trim().Length > 0 Then
			mailClient.Host = smtpServer.Trim()
		Else
			mailClient.Host = configuredHost.Trim()
		End If

		'// === SET THE SMTP SERVER PORT ===
		If smtpPort > 0 Then
			mailClient.Port = smtpPort
		Else
			mailClient.Port = configuredPort
		End If

		If (configuredUsername IsNot Nothing) And (configuredPassword IsNot Nothing) Then
			If configuredUsername.Trim().Length > 0 Then
				mailCredentials.UserName = configuredUsername
				mailCredentials.Password = configuredPassword
				mailClient.Credentials = mailCredentials
			End If
		End If

		'// === SEND THE MESSAGE, RETURN ANY ERRORS ===
		Try
			mailClient.Send(mailMessage)
			Return String.Empty
		Catch mail_exception As Exception
			Return mail_exception.ToString()
		End Try

	End Function





	Public Function SendViaAPI(Optional recipient As String = "", Optional sender As String = "", Optional body As String = "", Optional subject As String = "", Optional cc As String = "", Optional bcc As String = "", Optional replyTo As String = "", Optional priority As String = "Normal") As String
		'== GET PRECONFIGURED SETTINGS == 
		Dim configuredAPISender As String = String.Empty
		Dim configuredAPIKey As String = String.Empty

		configuredAPISender = WebConfigurationManager.AppSettings("MailAPISender")
		configuredAPIKey = WebConfigurationManager.AppSettings("MailAPIKey")

		'== CREATE MAIL OBJECTS ==
		' create a Web transport, using API Key
		Dim apiClient As New SendGridAPIClient(configuredAPIKey)
		Dim mailMessage As New SendGrid.Helpers.Mail.Mail()


		'=== SET THE SENDER & REPLY TO ADDRESSES === 
		If Not String.IsNullOrWhiteSpace(sender) Then
			mailMessage.From = New Email(sender.Trim())

			If Not String.IsNullOrWhiteSpace(replyTo) Then
				mailMessage.ReplyTo = New Email(replyTo.Trim())
			Else
				mailMessage.ReplyTo = New Email(sender.Trim())
			End If
		Else
			mailMessage.From = New Email(configuredAPISender.Trim())
			If Not String.IsNullOrWhiteSpace(replyTo) Then
				mailMessage.ReplyTo = New Email(replyTo.Trim())
			Else
				mailMessage.ReplyTo = New Email(configuredAPISender.Trim())
			End If
		End If

		Dim personalization As New Personalization()

		'=== SET THE RECIPIENTS ===
		If Not String.IsNullOrWhiteSpace(recipient) Then
			For Each recipientAddress As String In recipient.Trim().Split(";"c)
				If recipientAddress.Trim().Length >= 5 Then
					personalization.AddTo(New Email(recipientAddress.Trim()))
				End If
			Next
		Else
			personalization.AddTo(New Email(configuredAPISender.Trim()))
		End If
		For Each ccAddress As String In cc.Trim().Split(";"c)
			If ccAddress.Trim().Length >= 5 Then
				personalization.AddCc(New Email(ccAddress.Trim()))
			End If
		Next

		For Each bccAddress As String In bcc.Trim().Split(";"c)
			If bccAddress.Trim().Length >= 5 Then
				personalization.AddBcc(New Email(bccAddress.Trim()))
			End If
		Next

		mailMessage.AddPersonalization(personalization)

		'=== SET PRIORITY OF MESSAGE(DEFAULT IS NORMAL) ===
		If priority.Trim() = "High" Then
			mailMessage.AddHeader("Priority", "Urgent")
			mailMessage.AddHeader("Importance", "High")
		End If

		'=== SET THE SUBJECT ===
		mailMessage.Subject = subject.Trim()

		'=== SET THE MESSAGE CONTENT ===
		Dim content As New Content()
		content.Type = "text/html"
		content.Value = body.Trim()
		mailMessage.AddContent(content)

		Try
			Dim response As Object = apiClient.client.mail.send.post(requestBody:=mailMessage.Get())
			Return String.Empty
		Catch ex As Exception
			Return ex.ToString()
		End Try
	End Function


End Class
