Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class ChooseNamePrompt
    Inherits MetaphorDialog

    Private Const DEFAULT_NAME = "Olen Kyrpa"

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New ChooseNamePrompt(context, model, previous)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        Return DialogPrompt.CreateStringPrompt("What is your name?", AddressOf ChooseName)
    End Function

    Private Function ChooseName(value As String) As IDialog
        Return EmbarkActivity.Launch(Context, Model, Previous, value).Invoke()
    End Function
End Class
