
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class GroundItemMenu
    Inherits MetaphorPickerMenu

    Private ReadOnly itemModel As IItemModel

    Private Sub New(
                   context As IDisplayContext,
                   model As IWorldModel,
                   previous As DialogSource,
                   itemModel As IItemModel)
        MyBase.New(context, model, previous)
        Me.itemModel = itemModel
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return $"Do what with {itemModel.Name}?"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Append(AddressOf ChooseTake)
        End Get
    End Property

    Private Function ChooseTake(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Take", GroundTakeItemActivity.Launch(context, model, previous, itemModel))
    End Function

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, itemModel As IItemModel) As DialogSource
        Return Function() New GroundItemMenu(c, m, p, itemModel)
    End Function

    Private Function ChooseNeverMind(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Never Mind", GroundMenu.Launch(context, model, previous))
    End Function
End Class
