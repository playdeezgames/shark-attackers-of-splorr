Public Delegate Function DialogSource() As IDialog
Public Interface IDialog
    Function Run() As IDialogPrompt
End Interface
