Imports System.Security.Cryptography

Public Class Crypto

	Public Function EncryptSHA512(ByVal string_to_encrypt As String, ByVal salt As String) As String
		'// ENCRYPT THE SPECIFIED STRING USING SHA512 USING THE SPECIFIED SALT
		Dim utf8Encoder As New UTF8Encoding()
		Dim hashedString As Byte()
		
		Try
			Dim crypto_service As New System.Security.Cryptography.SHA512CryptoServiceProvider()
			hashedString = crypto_service.ComputeHash(utf8Encoder.GetBytes(salt.Trim() + string_to_encrypt.Trim()))
		Catch ex As Exception
			Dim crypto_service As New System.Security.Cryptography.SHA512Managed
			hashedString = crypto_service.ComputeHash(utf8Encoder.GetBytes(salt.Trim() + string_to_encrypt.Trim()))
		End Try
		
		Return Convert.ToBase64String(hashedString).Trim
	End Function

End Class
