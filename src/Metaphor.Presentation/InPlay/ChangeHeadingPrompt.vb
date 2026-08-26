Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class ChangeHeadingPrompt
    Inherits MetaphorDialog

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New ChangeHeadingPrompt(context, model, previous)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        Return DialogPrompt.CreateDoublePrompt("New Heading?", AddressOf ChooseHeading)
    End Function

    Private Function ChooseHeading(heading As Double) As IDialog
        Model.Avatar.Navigation.SetHeading(heading)
        Return InPlay.Launch(Context, Model, Previous).Invoke()
    End Function
End Class
