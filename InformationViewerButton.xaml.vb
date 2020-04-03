Imports System.ComponentModel
Imports System.IO

Public Class InformationViewerButton
    Protected Overrides Sub OnPropertyChanged(e As DependencyPropertyChangedEventArgs)
        MyBase.OnPropertyChanged(e)
        If e.Property Is Base64DocumentProperty Then
            If String.IsNullOrWhiteSpace(Base64Document) Then Return
            Dim range As New TextRange(FlowDocument.Document.ContentStart, FlowDocument.Document.ContentEnd)
            If ComponentModel.DesignerProperties.GetIsInDesignMode(Me) Then
                range.Text = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Qua igitur re ab deo vincitur, si aeternitate non vincitur? Quicquid enim a sapientia proficiscitur, id continuo debet expletum esse omnibus suis partibus; Te ipsum, dignissimum maioribus tuis, voluptasne induxit, ut adolescentulus eriperes P. Sed quid attinet de rebus tam apertis plura requirere? Atqui reperies, inquit, in hoc quidem pertinacem; Hoc dixerit potius Ennius: Nimium boni est, cui nihil est mali. Sin autem est in ea, quod quidam volunt, nihil impedit hanc nostram comprehensionem summi boni. Duo Reges: constructio interrete. </p>

<p>An me, inquis, tam amentem putas, ut apud imperitos isto modo loquar? Re mihi non aeque satisfacit, et quidem locis pluribus. Nullus est igitur cuiusquam dies natalis. Deinde dolorem quem maximum? De vacuitate doloris eadem sententia erit. Quid turpius quam sapientis vitam ex insipientium sermone pendere? Et quidem, Cato, hanc totam copiam iam Lucullo nostro notam esse oportebit; At ille pellit, qui permulcet sensum voluptate. Praeclare enim Plato: Beatum, cui etiam in senectute contigerit, ut sapientiam verasque opiniones assequi possit. Cum autem in quo sapienter dicimus, id a primo rectissime dicitur. <a href=""http://loripsum.net/"" target=""_blank"">Age sane, inquam.</a> Quippe: habes enim a rhetoribus; </p>

<p><b>Conferam avum tuum Drusum cum C.</b> <a href=""http://loripsum.net/"" target=""_blank"">Paria sunt igitur.</a> <b>Sed quid sentiat, non videtis.</b> Multa sunt dicta ab antiquis de contemnendis ac despiciendis rebus humanis; Est enim effectrix multarum et magnarum voluptatum. <mark>Igitur ne dolorem quidem.</mark> </p>

"
                Return
            End If

            Using ms = New MemoryStream(Convert.FromBase64String(Base64Document))
                range.Load(ms, DataFormats.Rtf)
            End Using
            SubscribeToHyperlinks()
        End If
    End Sub

    Private Sub SubscribeToHyperlinks()
        Dim hyperlinks = GetVisuals(FlowDocument.Document).OfType(Of Hyperlink)
        For Each hyperlink In hyperlinks
            hyperlink.Command = OpenLinkCommand
            hyperlink.CommandParameter = hyperlink.NavigateUri
        Next
    End Sub

    Private Iterator Function GetVisuals(ByVal root As DependencyObject) As IEnumerable(Of DependencyObject)
        For Each child In LogicalTreeHelper.GetChildren(root).OfType(Of DependencyObject)()
            Yield child

            For Each descendants In GetVisuals(child)
                Yield descendants
            Next
        Next
    End Function
#Region "OpenLinkCommand"

    Dim _OpenLinkCommand As DelegateCommand

    Protected Sub OpenLink(link As Object)
        Dim uri = CType(link, Uri)
        Process.Start(New ProcessStartInfo(uri.AbsoluteUri))
    End Sub

    Protected Function CanOpenLink(link As Object) As Boolean

        Return True
    End Function

    <DebuggerBrowsable(DebuggerBrowsableState.Never)>
    Public ReadOnly Property OpenLinkCommand As DelegateCommand
        Get
            If _OpenLinkCommand Is Nothing Then
                Dim newAction As New Action(Of Object)(AddressOf OpenLink)
                _OpenLinkCommand = New DelegateCommand(newAction, AddressOf CanOpenLink)
            End If
            Return _OpenLinkCommand
        End Get
    End Property

    Public Sub DoSomething()
        Task.Run(Sub()
                     BackgroundProcess()
                 End Sub)
    End Sub

    Private Sub BackgroundProcess()
        Dim ControlValue As Object
        Application.Current.Dispatcher.Invoke(Sub()
                                                  ControlValue = FlowDocument.ToString
                                              End Sub)

        Application.Current.Dispatcher.Invoke(Sub()
                                                  ControlValue = FlowDocument.ToString
                                              End Sub)
    End Sub
#End Region

    Private Sub flowDocument_IsVisibleChanged(sender As Object, e As DependencyPropertyChangedEventArgs) Handles flowDocument.IsVisibleChanged
        If FlowDocument.IsVisible Then SubscribeToHyperlinks()
    End Sub

    Private Sub toggleButton_Checked(sender As Object, e As RoutedEventArgs) Handles toggleButton.Checked
        SubscribeToHyperlinks()
    End Sub

    Public Property Base64Document As String
        Get
            Return GetValue(Base64DocumentProperty)
        End Get

        Set(ByVal value As String)
            SetValue(Base64DocumentProperty, value)
        End Set
    End Property

    Public Shared ReadOnly Base64DocumentProperty As DependencyProperty =
                           DependencyProperty.Register("Base64Document",
                           GetType(String), GetType(InformationViewerButton),
                           New PropertyMetadata(Nothing))

End Class
