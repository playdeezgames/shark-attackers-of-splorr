Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class MainMenu
    Inherits BasePickerMenu(Of IDisplayContext, IWorldModel)

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return "Main Menu:"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseEmbark).
                Append(AddressOf ChooseQuit)
        End Get
    End Property

    Private Function ChooseQuit(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.Create(model.IsQuittable, "Quit", AddressOf ConfirmQuit)
    End Function

    Private Function ConfirmQuit() As IDialog
        Return ConfirmDialog(Of IDisplayContext).
            Launch(
                Context,
                "Are you sure you want to quit?",
                Previous,
                Launch(Context, Model, Previous)).Invoke()
    End Function

    Private Function ChooseEmbark(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Embark!", ChooseNamePrompt.Launch(context, model, Previous))
    End Function

    Protected Overrides Sub Render()
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New MainMenu(context, model, previous)
    End Function
End Class
