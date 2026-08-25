Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class EmbarkActivity
    Inherits MetaphorDialog

    Private ReadOnly chosenName As String
    Private ReadOnly chosenPronouns As String

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, chosenName As String)
        MyBase.New(context, model, previous)
        Me.chosenName = chosenName
        Me.chosenPronouns = chosenPronouns
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, chosenName As String) As DialogSource
        Return Function() New EmbarkActivity(context, model, previous, chosenName)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        Model.Embark(chosenName, chosenPronouns)
        Return InPlay.Launch(Context, Model, Previous).Invoke().Run()
    End Function
End Class
