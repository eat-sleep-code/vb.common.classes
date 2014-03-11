Imports System
Imports System.Net.Mail

Namespace components

Public Class mail
    Private settings As New settings

	Public Function mail_send(recipient As String, sender As String, body As String, Optional subject As String = "", Optional cc As String = "", Optional bcc As String = "", Optional reply_to As String = "", Optional priority As String = "Normal", Optional smtp_server As String = "") As String
		
		Dim mail_object As New MailMessage
		
		'// === SET THE SENDER === 
		mail_object.From = New MailAddress(sender.Trim)


		'// === SET THE REPLY TO ADDRESS ===
		If reply_to.Trim.Length > 0 Then
			mail_object.Headers.Add("Reply-To", reply_to)
		Else
			mail_object.Headers.Add("Reply-To", sender.Trim)
		End If


		'// === SET THE RECIPIENTS ===
		Dim arr_recipient As Array = Split(recipient.Trim, ";")
		Dim arr_cc As Array = Split(cc.Trim, ";")
		Dim arr_bcc As Array = Split(bcc.Trim, ";")
		
		For Each recipient_item As String In arr_recipient
			mail_object.To.Add(New MailAddress(recipient_item))
		Next
		For Each cc_item As String In arr_cc
			mail_object.Cc.Add(New MailAddress(cc_item))
		Next
		For Each bcc_item As String In arr_bcc
			mail_object.Bcc.Add(New MailAddress(bcc_item))
		Next


		'// === SET THE SUBJECT ===
		mail_object.Subject  = subject.Trim


		'// === SET THE MESSAGE BODY TEXT ===
		mail_object.Body = body.Trim

		'// === SET PRIORITY OF MESSAGE (DEFAULT IS NORMAL) ===
		If priority.Trim = "High" Then
			mail_object.Priority = MailPriority.High
		ElseIf priority.Trim = "Low" Then
			mail_object.Priority = MailPriority.Low
		Else
			mail_object.Priority = MailPriority.Normal
		End If


		'// === SET FORMAT OF THE MAIL ===
		mail_object.IsBodyHtml = True

 		
		'// === SET THE SMTP SERVER ===
		
		If smtp_server.Trim.Length <= 0 Then
			smtp_server = settings.smtp_server.Trim
		End If
		
		
		'// === SEND THE MESSAGE, RETURN ANY ERRORS ===
		Try
			Dim smtp As New SmtpClient()
			smtp.Host = smtp_server
			smtp.Send(mail_object)
			Return ""
		Catch mail_exception as Exception
			Return mail_exception.ToString
		End Try


	End Function
        
End Class

End NameSpace


