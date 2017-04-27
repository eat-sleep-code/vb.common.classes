Imports System.IO
Imports System.Data

Public Class Navigation



	#Region " Declare: Shared Classes "

		Private xmlData As New DataXML()
		
	#End Region
	
	
	
	#Region " Navigation: Generate Main Menu "
	
		Public Sub generate_main_menu(ByVal NavigationMenuHeader As Telerik.Web.UI.RadMenu, ByVal Optional region As String = "")
			NavigationMenuHeader.Items.Clear
			If region Is Nothing Then
				region = "us"
			End If
			
			
			Dim bln_text As Boolean = False
			Dim dt_navigation As DataTable
			dt_navigation = xmlData.GetDataTable(HttpContext.Current.Server.MapPath("~/App_Data/navigation.xml"), "item")
			
			For Each dr_navigation As DataRow In dt_navigation.Select("parent_ID = 0")
				Dim menu_item As New Telerik.Web.UI.RadMenuItem
				If dr_navigation.Item("main_inclusion").ToString().ToLower <> "false" Then
					bln_text = False
					If region.EndsWith("_mobile") And File.Exists(HttpContext.Current.Server.MapPath("~/style/" + region + ".master")) = True Then
						menu_item.Text = dr_navigation.Item("text")
						menu_item.NavigateUrl = dr_navigation.Item("url").Replace("region=" + region.Replace("_mobile",""), "region=" + region.Replace("_mobile","") + "_mobile")
					Else
						menu_item.NavigateUrl = dr_navigation.Item("url")
						Try
						menu_item.ImageUrl = dr_navigation.Item("image")
						menu_item.HoveredImageUrl = dr_navigation.Item("image_hover")
						menu_item.Height = System.Web.UI.WebControls.Unit.Parse(dr_navigation.Item("image_height"))
						menu_item.Width = System.Web.UI.WebControls.Unit.Parse(dr_navigation.Item("image_width"))
						Catch
							menu_item.Text = dr_navigation.Item("text")
							NavigationMenuHeader.Flow = Telerik.Web.UI.ItemFlow.Horizontal
							bln_text = True
						End Try
					End If
					menu_item.Target = dr_navigation.Item("target")
					menu_item.ToolTip = dr_navigation.Item("title")
				
				
					'// CURRENTLY ONLY GOING ONE LEVEL DOWN (COULD RECURSE, BUT SEEM TO BE RUNNING INTO PROBLEMS WITH THAT NOW)
					For Each dr_navigation_sub As DataRow In dt_navigation.Select("parent_ID = " + dr_navigation.Item("item_id").ToString)
						Dim menu_item_sub As New Telerik.Web.UI.RadMenuItem
						If dr_navigation_sub.Item("main_inclusion").ToString().ToLower <> "false" Then
							If region.EndsWith("_mobile") And File.Exists(HttpContext.Current.Server.MapPath("~/style/" + region + ".master")) = True Then
								menu_item_sub.Text = dr_navigation_sub.Item("text")
								menu_item_sub.NavigateUrl = dr_navigation_sub.Item("url").Replace("region=" + region.Replace("_mobile",""), "region=" + region.Replace("_mobile","") + "_mobile")
							Else
								menu_item_sub.NavigateUrl = dr_navigation_sub.Item("url")
								Try
								menu_item_sub.ImageUrl = dr_navigation_sub.Item("image")
								menu_item_sub.HoveredImageUrl = dr_navigation_sub.Item("image_hover")
								menu_item_sub.Height = System.Web.UI.WebControls.Unit.Parse(dr_navigation_sub.Item("image_height"))
								menu_item_sub.Width = System.Web.UI.WebControls.Unit.Parse(dr_navigation_sub.Item("image_width"))
								Catch
									menu_item_sub.Text = dr_navigation_sub.Item("text")
								End Try
							End If
							menu_item_sub.Target = dr_navigation_sub.Item("target")
							menu_item_sub.ToolTip = dr_navigation_sub.Item("title")
							menu_item.Items.Add(menu_item_sub)
						End If
					Next
				
					NavigationMenuHeader.Items.Add(menu_item)
					
					If bln_text = True And dt_navigation.Rows.IndexOf(dr_navigation) + 1 <> dt_navigation.Rows.Count Then
						Dim menu_separator As New Telerik.Web.UI.RadMenuItem
						menu_separator.Text = "|"
						menu_separator.IsSeparator = True
						NavigationMenuHeader.Items.Add(menu_separator)
					End If

				End If
			Next	
			
			If NavigationMenuHeader.Items.Item(NavigationMenuHeader.Items.Count - 1).IsSeparator = True
				NavigationMenuHeader.Items.RemoveAt(NavigationMenuHeader.Items.Count - 1)
			End If
			dt_navigation.Clear
			dt_navigation = Nothing

		End Sub
		
		
	
	#End Region
	
	
	
	
	#Region " Navigation: Generate Footer Menu "
	
		Public Sub generate_text_menu(ByVal NavigationMenuFooter As Panel, ByVal Optional region As String = "")
			If region Is Nothing Then
				region = "us"
			End If
			
			Dim dt_navigation As DataTable
			dt_navigation = xmlData.GetDataTable(HttpContext.Current.Server.MapPath("~/App_Data/navigation.xml"), "item")
			NavigationMenuFooter.Controls.Clear()
			
			For Each dr_navigation As DataRow In dt_navigation.Rows()
				If dr_navigation.Item("footer_inclusion").ToString().ToLower <> "false" And dr_navigation.Item("parent_ID").ToString().ToLower = "0" Then
					Dim menu_item As New HyperLink
					Dim menu_seperator As New Label
					Try
						menu_seperator.Text = System.Web.Configuration.WebConfigurationManager.AppSettings.Item("footer_navigation_separator")
					Catch
						menu_seperator.Text = " "
					End Try
					menu_item.Text = dr_navigation.Item("text")
					
					If region.EndsWith("_mobile") And File.Exists(HttpContext.Current.Server.MapPath("~/style/" + region + ".master")) = True Then
						menu_item.NavigateUrl = dr_navigation.Item("url").Replace("region=" + region.Replace("_mobile",""), "region=" + region.Replace("_mobile","") + "_mobile")
					Else
						menu_item.NavigateUrl = dr_navigation.Item("url")
					End If
					menu_item.Target = dr_navigation.Item("target")
					menu_item.ToolTip = dr_navigation.Item("title")
					NavigationMenuFooter.Controls.Add(menu_item)
					NavigationMenuFooter.Controls.Add(menu_seperator)
				End If
			Next
			
			dt_navigation.Clear
			dt_navigation = Nothing

			NavigationMenuFooter.Controls.RemoveAt(NavigationMenuFooter.Controls.Count - 1)		
		End Sub
		
	#End Region
		
		
		
End Class
