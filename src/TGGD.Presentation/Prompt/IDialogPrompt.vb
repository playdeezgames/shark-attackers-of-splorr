Public Delegate Function StringToDialogDelegate(value As String) As IDialog
Public Delegate Function IntegerToDialogDelegate(value As Integer) As IDialog
Public Delegate Function DoubleToDialogDelegate(value As Double) As IDialog
Public Interface IDialogPrompt
    ReadOnly Property PromptType As DialogPromptType
    ReadOnly Property Choices As String()
    ReadOnly Property Title As String
    Function Respond(
               Optional text As String = Nothing,
               Optional counter As Integer? = Nothing,
               Optional dimension As Double? = Nothing) As IDialog
End Interface
