Imports Metaphor.Processing
Imports TGGD.Presentation

Friend NotInheritable Class ChangeSpeedPrompt
    Inherits MetaphorDialog

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New ChangeSpeedPrompt(context, model, previous)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        Return DialogPrompt.CreateDoublePrompt("New Speed?", AddressOf ChooseSpeed)
    End Function

    Private Function ChooseSpeed(speed As Double) As IDialog
        Model.Avatar.Navigation.SetSpeed(speed)
        Return InPlay.Launch(Context, Model, Previous).Invoke()
    End Function
End Class
