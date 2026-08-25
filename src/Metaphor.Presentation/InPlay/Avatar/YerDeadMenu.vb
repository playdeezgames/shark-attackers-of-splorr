
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class YerDeadMenu
    Inherits MetaphorPickerMenu

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return "Now What?"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseStatus).
                Append(AddressOf ChooseLook).
                Append(AddressOf InPlay.ChooseWatchAd).
                Append(AddressOf InPlay.ChooseGameMenu)
        End Get
    End Property

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New YerDeadMenu(context, model, previous)
    End Function

    Private Function ChooseStatus(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Status...", StatusActivity.LaunchStatusActivity(context, model, previous))
    End Function

    Private Function ChooseLook(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Look", LookActivity.Launch(context, model, previous))
    End Function
End Class
