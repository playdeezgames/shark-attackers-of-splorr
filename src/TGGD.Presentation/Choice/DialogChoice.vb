Public Class DialogChoice
    Implements IDialogChoice
    Private Sub New(enabled As Boolean, text As String, nextDialogGenerator As DialogSource)
        Me.Enabled = enabled
        Me.Text = text
        Me.nextDialogGenerator = nextDialogGenerator
    End Sub

    Public ReadOnly Property Text As String Implements IDialogChoice.Text
    Private ReadOnly nextDialogGenerator As DialogSource

    Public ReadOnly Property NextDialog As IDialog Implements IDialogChoice.NextDialog
        Get
            Return nextDialogGenerator.Invoke()
        End Get
    End Property

    Public ReadOnly Property Enabled As Boolean Implements IDialogChoice.Enabled

    Public Shared Function Create(enabled As Boolean, text As String, nextDialogGenerator As DialogSource) As IDialogChoice
        Return New DialogChoice(enabled, text, nextDialogGenerator)
    End Function

    Public Shared Function CreateEnabled(text As String, nextDialogGenerator As DialogSource) As IDialogChoice
        Return Create(True, text, nextDialogGenerator)
    End Function
End Class
