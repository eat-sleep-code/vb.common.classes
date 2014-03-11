Imports System
Imports System.DirectoryServices
Imports System.Web
Imports System.Web.HttpContext
Imports ActiveDs '// Active DS Type Library - Required For Handling Of UTC Time

Namespace components

	Public Class active_directory

		Protected settings As New settings '// Class That Imports Web.Config Settings As Public ReadOnly Properties



		#Region " Get Active Directory User Information "

			'// ==============================================================================================================================================
			'//
			'// Returns String or DirectoryEntry For An Active Directory User
			'// ad_filter_value:	The Active Directory user account. 
			'// ad_return:			The Active Directory LDAP value you wish to return.
			'//	ad_return_type:		"UTC":				Returns a date string value converted from an Active Directory UTC string.
			'//						"DirectoryEntry":	Returns a Active Directory DirectoryEntry object. 
			'//						"String" (Default):	Returns a string value equal to the Active Directory LDAP value.
			'// Usage: active_directory.get_ad_object("jdoe", "mail", "String")
			'//
			'// ==============================================================================================================================================

			Public Function get_ad_object(ByVal ad_filter_value As String, ByVal ad_return As String, Optional ad_return_type As String = "String") As Object
				Try
					ad_filter_value = Right(ad_filter_value, Len(ad_filter_value) - InStr(ad_filter_value, "\"))
					Dim ad_filter As String = "samaccountname=" + ad_filter_value
					Dim ad_root_forest As String
					ad_root_forest = "LDAP://" & settings.ad_server + settings.ad_connection_string
					
					Dim ad_root As New DirectoryEntry(ad_root_forest)
					ad_root.AuthenticationType = AuthenticationTypes.Delegation
					ad_root.Username = settings.ad_username
					ad_root.Password = settings.ad_password
					
					Dim ad_searcher As New DirectorySearcher(ad_root)
					ad_searcher.SearchScope = SearchScope.Subtree
					ad_searcher.ReferralChasing = ReferralChasingOption.All
					ad_searcher.Filter = ad_filter
					ad_searcher.CacheResults = True
		
					Dim ad_search As SearchResult = ad_searcher.FindOne()
					Dim ad_object As DirectoryEntry = ad_search.GetDirectoryEntry

					Select Case ad_return_type.Trim
						Case "DirectoryEntry"
							Return ad_object
						Case "UTC"
							Dim ad_property_collection as PropertyCollection = ad_object.Properties
							Dim long_integer_object As Object = ad_property_collection("lastLogon")(0) 
							Dim converted_date as Long = (CType(long_integer_object.HighPart , Long) * CType(&h100000000, Long)) + Ctype(long_integer_object.LowPart, Long)
							Return DateTime.FromFileTime(converted_date)
						Case Else 
							Return ad_object.Properties.Item(ad_return).Value.ToString
					End Select
				Catch ex As Exception
					Current.Response.Write("get_ad_object: " + ex.ToString + "<BR />")
					Return ""
				End Try
			End Function
		
		#End Region



	'// ========================================================================================



		#Region " Get Active Directory Group Membership" 

			'// ==============================================================================================================================================
			'//
			'// Returns Active Directory Group Membership For An Active Directory User
			'// ad_filter_value:	The Active Directory user account. 
			'// Usage: active_directory.get_ad_group("jdoe")
			'//
			'// ==============================================================================================================================================

			Public Function get_ad_group(ByVal ad_filter_value As String) As Collection
				Try
					ad_filter_value = Right(ad_filter_value, Len(ad_filter_value) - InStr(ad_filter_value, "\"))
					Dim ad_object As DirectoryEntry = CType(get_ad_object(ad_filter_value, "samaccountname", "DirectoryEntry"), DirectoryEntry)

					If Not ad_object Is Nothing Then
						ad_object.AuthenticationType = AuthenticationTypes.Delegation
						ad_object.Username = settings.ad_username
						ad_object.Password = settings.ad_password
						Dim ad_member_count As Integer = ad_object.Properties("MemberOf").Count
						Dim i As Integer
						Dim ad_group_collection As New Collection
						
						If ad_member_count > 0 Then
							For i = 0 To ad_member_count - 1
								Dim ad_member_of As String = ad_object.Properties("MemberOf").Item(i).ToString
								Dim ad_group As String = Left(ad_member_of, (InStr(ad_member_of, ",") - 1))
								ad_group_collection.Add(ad_group.Replace("CN=", ""))
							Next                        
						End If
				
						Return ad_group_collection
					Else
						Return Nothing
					End If
				Catch ex As Exception
					Current.Response.Write("get_ad_group: " + ex.ToString + "<BR />")
					Return Nothing
				End Try
			End Function

		#End Region



	'// ========================================================================================



		#Region " Get Active Directory User List " 

			'// ==============================================================================================================================================
			'//
			'// Returns List Of Active Directory Users
			'// Usage: active_directory.get_ad_users()
			'//
			'// ==============================================================================================================================================
			Public Function get_ad_users() As Collection
				Try
					Dim ad_root_forest As String
					ad_root_forest = "LDAP://" & settings.ad_server + settings.ad_connection_string
					
					Dim ad_root As New DirectoryEntry(ad_root_forest)
					ad_root.AuthenticationType = AuthenticationTypes.Delegation
					ad_root.Username = settings.ad_username
					ad_root.Password = settings.ad_password
					
					Dim ad_searcher As New DirectorySearcher(ad_root)
					ad_searcher.SearchScope = SearchScope.Subtree
					ad_searcher.ReferralChasing = ReferralChasingOption.All
					ad_searcher.PropertiesToLoad.Add("cn")
					ad_searcher.Filter = "(objectClass=User)"
					ad_searcher.Sort = New SortOption("sn", SortDirection.Ascending)
					ad_searcher.CacheResults = True

					Dim ad_result As SearchResult
					Dim ad_directory_entry As DirectoryEntry
					Dim ad_collection As New Collection
				
					For Each ad_result In ad_searcher.FindAll
						ad_directory_entry = ad_result.GetDirectoryEntry
						Try
							ad_collection.Add(ad_directory_entry.Properties.Item("cn").Value.ToString) 
						Catch ex As Exception
							Return Nothing
						End Try
					Next
					Return ad_collection
		
				Catch ex As Exception
					Current.Response.Write("get_ad_users: " + ex.ToString + "<BR />")
					Return Nothing
				End Try
			End Function

		#End Region
		
		

	End Class

End NameSpace
