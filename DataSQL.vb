Imports System.Data
Imports System.Data.SqlClient
Imports System.Web


Public Class DataSQL
	Implements IDisposable

	Private connectionString As String
	Private sqlConnection As SqlConnection
	Private sqlDataAdapter As SqlDataAdapter
	Private sqlCommand As SqlCommand
	Private sqlDatabaseType As DriverType
	Private httpContext As HttpContext
	Public Enum DriverType
		SqlClient
	End Enum

	Public Sub New(connection As String, databaseType As String)
		httpContext = HttpContext.Current
		httpContext.Trace.Write("MyDatabase", "New")
		DriverTypeFromString = databaseType
		connectionString = connection

		sqlConnection = CreateConnection(connectionString)
	End Sub

	Public Sub New(connection As String, databaseType As DriverType)
		httpContext = HttpContext.Current
		httpContext.Trace.Write("MyDatabase", "New")
		Me.DatabaseType = databaseType
		connectionString = connection
		sqlConnection = CreateConnection(connectionString)
	End Sub

	Private Sub OpenConn()
		httpContext = HttpContext.Current
		httpContext.Trace.Write("MyDatabase", "OpenConn")
		sqlConnection.Open()
	End Sub

	Private Sub CloseConn()
		httpContext = HttpContext.Current
		httpContext.Trace.Write("MyDatabase", "CloseConn")
		sqlConnection.Close()
	End Sub



	#Region "Setting The Database Driver Type"

		Public Property DatabaseType() As DriverType
			Get
				httpContext.Trace.Write("MyDatabase", "DatabaseType Get")
				Return sqlDatabaseType
			End Get
			Set
				httpContext.Trace.Write("MyDatabase", "DatabaseType Set")
				sqlDatabaseType = value
			End Set
		End Property

		Public WriteOnly Property DriverTypeFromString() As String
			Set
				sqlDatabaseType = DriverType.SqlClient
			End Set
		End Property

	#End Region



	#Region "Create Objects - Data Abstraction Items"

		Private Function CreateConnection(connection As String) As SqlConnection
			httpContext.Trace.Write("MyDatabase", "CreateConnection")
			Return New SqlConnection(connection)
		End Function

		Private Function CreateDataAdapter(command As SqlCommand) As SqlDataAdapter
			httpContext.Trace.Write("MyDatabase", "CreateDataAdapter")
			Return New SqlDataAdapter(command)
		End Function

	#End Region



	#Region "Database Methods"

		Public Sub Execute(ByRef dataSet As DataSet)
			Try
				httpContext.Trace.Write("MyDatabase", "Execute")

				sqlDataAdapter = CreateDataAdapter(sqlCommand)
				sqlCommand.Connection = sqlConnection

				httpContext.Trace.Write("Execute", sqlCommand.CommandText)

				OpenConn()
				sqlDataAdapter.Fill(dataSet)
			Catch ex As Exception
				httpContext.Trace.Write("Error", ex.ToString())
			Finally
				CloseConn()
			End Try
		End Sub

		Public Sub Execute(sql As String, ByRef dataSet As DataSet)

			CreateCommand(sql, CommandType.Text)
			Execute(dataSet)

		End Sub

		Public Sub Execute(ByRef dataTable As DataTable)

			Dim dataSet As New DataSet()
			Execute(dataSet)
			dataTable = dataSet.Tables(0)
		End Sub


		Public Sub Execute(sql As String, ByRef dataTable As DataTable)
			Dim dataSet As New DataSet()
			CreateCommand(sql, CommandType.Text)
			Execute(dataTable)
			dataTable = dataSet.Tables(0)
		End Sub

		Public Sub Execute(ByRef dataRow As DataRow)
			Dim dataSet As New DataSet()
			Execute(dataSet)
			Try
				dataRow = dataSet.Tables(0).Rows(0)
			Catch
			End Try
		End Sub


		Public Sub Execute(sql As String, ByRef dataRow As DataRow)
			CreateCommand(sql, CommandType.Text)
			Execute(dataRow)
		End Sub

		Public Function ExecuteScalar() As Object
			Dim o As Object = Nothing
			Try
				httpContext.Trace.Write("MyDatabase", "ExecuteScalar")
				sqlCommand.Connection = sqlConnection
				OpenConn()
				httpContext.Trace.Write("MyDatabase", sqlCommand.CommandText)
				o = sqlCommand.ExecuteScalar()
			Catch ex As Exception
				httpContext.Trace.Write("Error", ex.ToString())
			Finally
				CloseConn()
			End Try
			Return o
		End Function

		Public Function ExecuteScalar(sql As String) As Object
			CreateCommand(sql, CommandType.Text)
			Return ExecuteScalar()
		End Function

		Public Sub ExecuteNonQuery()
			Try
				httpContext.Trace.Write("MyDatabase", "ExecuteNonQuery")
				sqlCommand.Connection = sqlConnection
				OpenConn()

				httpContext.Trace.Write("MyDatabase", sqlCommand.CommandText)
				sqlCommand.ExecuteNonQuery()
			Catch ex As Exception
				httpContext.Trace.Write("Error", ex.ToString())
			Finally
				CloseConn()
			End Try
		End Sub

		Public Sub ExecuteNonQuery(sql As String)
			CreateCommand(sql, CommandType.Text)
			ExecuteNonQuery()
		End Sub

	#End Region



	#Region "Command Methods"

		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")> _
		Public Sub CreateCommand(name As String, type As CommandType)
			httpContext.Trace.Write("MyDatabase", "CreateCommand")
			sqlCommand = New SqlCommand(name)
			sqlCommand.CommandType = type
		End Sub

		Public Sub AddParameter(name As String, value As String)
			httpContext.Trace.Write("MyDatabase", "AddParameter")
			Dim parameter As SqlParameter = Nothing
			parameter = New SqlParameter(name, value)
			sqlCommand.Parameters.Add(parameter)
		End Sub

		Public Sub AddParameter(name As String, direction As ParameterDirection, type As String, size As Integer)
			httpContext.Trace.Write("MyDatabase", "AddParameter")
			Dim databaseType As SqlDbType = CType([Enum].Parse(GetType(SqlDbType), type, True), SqlDbType)
			Dim parameter As SqlParameter = Nothing
			parameter = New SqlParameter(name, databaseType, size, direction, False, 10, _
				0, "", DataRowVersion.Current, Nothing)
			sqlCommand.Parameters.Add(parameter)
		End Sub

		Public Sub AddParameter(parameter As IDbDataParameter)
			httpContext.Trace.Write("MyDatabase", "AddParameter")
			sqlCommand.Parameters.Add(parameter)
		End Sub

		Public Function ReadParameter(parameterName As String) As Object
			httpContext.Trace.Write("MyDatabase", "ReadParam")
			Return sqlCommand.Parameters(parameterName).Value
		End Function

	#End Region



	#Region "Transaction Methods"

		Public Sub UseTransaction()
			httpContext.Trace.Write("MyDatabase", "UseTransaction")
			Dim sqlTransaction As SqlTransaction = Nothing
			sqlCommand.Transaction = sqlTransaction
		End Sub

		Public Sub CommitTransaction()
			httpContext.Trace.Write("MyDatabase", "commitTransaction")
			sqlCommand.Transaction.Commit()
		End Sub

		Public Sub RollbackTransaction()
			httpContext.Trace.Write("MyDatabase", "rollbackTransaction")
			If sqlCommand.Transaction Is Nothing = False Then
				sqlCommand.Transaction.Rollback()
			End If
		End Sub

	#End Region



	#Region "Dispose"

		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

		Protected Overrides Sub Finalize()
			Try
				Dispose(False)
			Finally
				MyBase.Finalize()
			End Try
		End Sub

		Protected Overridable Sub Dispose(disposing As [Boolean])
			If disposing = True Then
				sqlCommand.Dispose()
				sqlCommand = Nothing
			End If
		End Sub

	#End Region

End Class
