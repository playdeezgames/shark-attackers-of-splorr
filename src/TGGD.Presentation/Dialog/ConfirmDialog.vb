Public Class ConfirmDialog(Of TContext As IDisplayContext)
    Inherits BaseDialog(Of TContext)

    Public ReadOnly Property Text As String
    Public ReadOnly Property OnConfirmDialog As DialogSource
    Public ReadOnly Property OnCancelDialog As DialogSource

    Private Sub New(
                   context As TContext,
                   text As String,
                   onConfirmDialog As DialogSource,
                   onCancelDialog As DialogSource)
        MyBase.New(context)
        Me.Text = text
        Me.OnConfirmDialog = onConfirmDialog
        Me.OnCancelDialog = onCancelDialog
    End Sub

    Public Shared Function Launch(
                                 context As TContext,
                                 text As String,
                                 onConfirmDialog As DialogSource,
                                 onCancelDialog As DialogSource) As DialogSource
        Return Function() New ConfirmDialog(Of TContext)(context, text, onConfirmDialog, onCancelDialog)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        Return DialogPrompt.CreateChoicePrompt(
            Text,
            DialogChoice.CreateEnabled("No", OnCancelDialog),
            DialogChoice.CreateEnabled("Yes", OnConfirmDialog))
    End Function
End Class
