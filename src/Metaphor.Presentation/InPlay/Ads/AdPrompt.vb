
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class AdPrompt
    Inherits MetaphorPickerMenu

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return String.Empty
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseOk)
        End Get
    End Property

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function()
                   model.Ad.Show()
                   Return New AdPrompt(context, model, previous)
               End Function
    End Function

    Private Function ChooseOk(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Ok", InPlay.Launch(context, model, previous))
    End Function
End Class
