Imports TGGD.Presentation
Friend Delegate Sub DialogPredicate(dialog As IDialog)
Friend Class WrappedPrompt
    Implements IDialogPrompt

    Private _wrapped As IDialogPrompt
    Private predicate As DialogPredicate

    Public Sub New(dialogPrompt As IDialogPrompt, predicate As DialogPredicate)
        Me._wrapped = dialogPrompt
        Me.predicate = predicate
    End Sub

    Public ReadOnly Property PromptType As DialogPromptType Implements IDialogPrompt.PromptType
        Get
            Return _wrapped.PromptType
        End Get
    End Property

    Public ReadOnly Property Choices As String() Implements IDialogPrompt.Choices
        Get
            Return _wrapped.Choices
        End Get
    End Property

    Public ReadOnly Property Title As String Implements IDialogPrompt.Title
        Get
            Return _wrapped.Title
        End Get
    End Property

    Public Function Respond(
                           Optional text As String = Nothing,
                           Optional counter As Integer? = Nothing,
                           Optional dimension As Double? = Nothing) As IDialog Implements IDialogPrompt.Respond
        Dim nextDialog = _wrapped.Respond(text, counter, dimension)
        predicate(nextDialog)
        Return nextDialog
    End Function
End Class
