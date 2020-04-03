Imports System.Windows.Input
Imports System.Windows.Threading
Imports System.Windows
Imports System.Threading

Public Class DelegateCommand
    Implements ICommand
    Implements IDisposable

    Dim _dispatcher As Dispatcher

    Private ReadOnly Property MyDispatcher As Dispatcher
        Get
            Return _dispatcher
        End Get
    End Property

    ' Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        AddHandler(ByVal value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End RemoveHandler
        RaiseEvent()
            'RaiseEvent CanExecuteChanged(Me, New EventArgs)
        End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the <see cref="CanExecuteChanged"/> event.
    ''' </summary>
    Protected Overridable Sub OnCanExecuteChanged()
        If Not _dispatcher.CheckAccess() Then
            _dispatcher.Invoke(DirectCast(AddressOf OnCanExecuteChanged, ThreadStart), DispatcherPriority.Normal)
        Else
            CommandManager.InvalidateRequerySuggested()
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private _canExecute As Predicate(Of Object)
    Private _execute As Action(Of Object)

    ''' <summary>
    ''' Constructs an instance of <c>DelegateCommand</c>.
    ''' </summary>
    ''' <remarks>
    ''' This constructor creates the command without a delegate for determining whether the command can execute. Therefore, the
    ''' command will always be eligible for execution.
    ''' </remarks>
    ''' <param name="execute">
    ''' The delegate to invoke when the command is executed.
    ''' </param>
    Public Sub New(execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub

    ''' <summary>
    ''' Constructs an instance of <c>DelegateCommand</c>.
    ''' </summary>
    ''' <param name="execute">
    ''' The delegate to invoke when the command is executed.
    ''' </param>
    ''' <param name="canExecute">
    ''' The delegate to invoke to determine whether the command can execute.
    ''' </param>
    Public Sub New(execute As Action(Of Object), canExecute As Predicate(Of Object))
        'execute.AssertNotNull("execute")
        _execute = execute
        _canExecute = canExecute
        If Application.Current IsNot Nothing Then
            _dispatcher = Application.Current.Dispatcher
        Else
            _dispatcher = Dispatcher.CurrentDispatcher
        End If
    End Sub

    Private Sub OnDispose()
        _canExecute = Nothing
        _execute = Nothing
    End Sub

    ''' <summary>
    ''' Determines whether this command can execute.
    ''' </summary>
    ''' <remarks>
    ''' If there is no delegate to determine whether the command can execute, this method will return <see langword="true"/>. If a delegate was provided, this
    ''' method will invoke that delegate.
    ''' </remarks>
    ''' <param name="parameter">
    ''' The command parameter.
    ''' </param>
    ''' <returns>
    ''' <see langword="true"/> if the command can execute, otherwise <see langword="false"/>.
    ''' </returns>
    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If disposedValue Then Return False
        If _canExecute Is Nothing Then
            Return True
        End If

        Return _canExecute(parameter)
    End Function

    ''' <summary>
    ''' Executes this command.
    ''' </summary>
    ''' <remarks>
    ''' This method invokes the provided delegate to execute the command.
    ''' </remarks>
    ''' <param name="parameter">
    ''' The command parameter.
    ''' </param>
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        If disposedValue Then Return
        _execute(parameter)
    End Sub
    ''' <summary>
    ''' Returns the name of this command
    ''' </summary>
    ''' <value>The name of the command</value>
    ''' <returns>The name of the command</returns>
    ''' <remarks>This has been added for public commands that may be shared between components.</remarks>
    Public Property CommandName As String

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
