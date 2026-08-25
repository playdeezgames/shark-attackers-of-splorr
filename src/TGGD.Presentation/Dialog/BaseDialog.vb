Public MustInherit Class BaseDialog(Of TContext As IDisplayContext)
    Implements IDialog
    Protected ReadOnly Context As TContext
    Protected Sub New(context As TContext)
        Me.Context = context
    End Sub

    Public MustOverride Function Run() As IDialogPrompt Implements IDialog.Run
End Class
