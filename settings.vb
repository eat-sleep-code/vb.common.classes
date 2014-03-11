Imports System.Configuration

Namespace components

	Public Class settings



		# Region " Database Settings "
		
			Public ReadOnly Property db_connection_string(Optional ByVal connection_string_ID As String = "") As String
				Get
					If connection_string_ID = "" Then
						Return System.Configuration.ConfigurationManager.ConnectionStrings(0).ConnectionString
					Else
						Return System.Configuration.ConfigurationManager.ConnectionStrings(connection_string_ID).ConnectionString
					End If
				End Get
			End Property



			Public ReadOnly Property db_connection_type(Optional ByVal connection_string_ID As String = "") As String
				Get
					If connection_string_ID = "" Then
						Return System.Configuration.ConfigurationManager.ConnectionStrings(0).ProviderName
					Else
						Return System.Configuration.ConfigurationManager.ConnectionStrings(connection_string_ID).ProviderName
					End If
				End Get
			End Property
			
		#End Region
		




		# Region " Active Directory Settings "
		
			Public ReadOnly Property ad_server() As String
				Get
					Return ConfigurationManager.AppSettings.Get("ad_server")
				End Get
			End Property



			Public ReadOnly Property ad_connection_string() As String
				Get
					Return ConfigurationManager.AppSettings.Get("ad_connection_string")
				End Get
			End Property



			Public ReadOnly Property ad_username() As String
				Get
					Return ConfigurationManager.AppSettings.Get("ad_username")
				End Get
			End Property



			Public ReadOnly Property ad_password() As String
				Get
					Return ConfigurationManager.AppSettings.Get("ad_password")
				End Get
			End Property
			
		#End Region
		
		
		


		# Region " Mail Server Settings "

			Public ReadOnly Property smtp_server() As String
				Get
					Return ConfigurationManager.AppSettings.Get("smtp_server")
				End Get
			End Property
			
			
			
			Public ReadOnly Property smtp_port() As String
				Get
					Return ConfigurationManager.AppSettings.Get("smtp_port")
				End Get
			End Property



			Public ReadOnly Property smtp_authentication() As Boolean
				Get
					Return CType(ConfigurationManager.AppSettings.Get("smtp_authentication"), Boolean)
				End Get
			End Property
			
		#End Region

		



		# Region " Index Server Settings "
		
			Public ReadOnly Property index_catalog() As String
				Get
					Return CType(ConfigurationManager.AppSettings.Get("index_catalog"), String)
				End Get
			End Property



			Public ReadOnly Property index_ignored_extensions() As String
				Get
					Return CType(ConfigurationManager.AppSettings.Get("index_ignored_extensions"), String)
				End Get
			End Property



			Public ReadOnly Property index_ignored_paths() As String
				Get
					Return CType(ConfigurationManager.AppSettings.Get("index_ignored_paths"), String)
				End Get
			End Property



			Public ReadOnly Property index_rank_image() As String
				Get
					Return CType(ConfigurationManager.AppSettings.Get("index_rank_image"), String)
				End Get
			End Property



			Public ReadOnly Property file_path() As String
				Get
					Return CType(ConfigurationManager.AppSettings.Get("file_path"), String)
				End Get
			End Property
				
		#End Region


		
	End Class  

End Namespace