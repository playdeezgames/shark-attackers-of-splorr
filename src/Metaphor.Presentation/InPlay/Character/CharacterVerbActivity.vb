
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class CharacterVerbActivity
    Inherits MetaphorPickerMenu

    Private ReadOnly characterModel As ICharacterModel
    Private ReadOnly verbModel As IVerbModel

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, characterModel As ICharacterModel, verbModel As IVerbModel)
        MyBase.New(context, model, previous)
        Me.characterModel = characterModel
        Me.verbModel = verbModel
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

    Private Function ChooseOk(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Ok", CharacterMenu.Launch(context, model, previous, characterModel))
    End Function

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, characterModel As ICharacterModel, verbModel As IVerbModel) As DialogSource
        Return Function()
                   verbModel.Perform()
                   Return New CharacterVerbActivity(c, m, p, characterModel, verbModel)
               End Function
    End Function
End Class
