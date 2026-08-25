
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class GroundMenu
    Inherits MetaphorPickerMenu

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return "On the ground:"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Concat(Model.Location.Ground.Inventory.ItemStacks.Select(AddressOf ChooseItemStack))
        End Get
    End Property

    Private Function ChooseItemStack(itemStackModel As IItemStackModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.CreateEnabled(
                        itemStackModel.Name,
                        GroundItemStackMenu.Launch(c, m, p, itemStackModel))
               End Function
    End Function

    Private Shared Function ChooseItem(itemModel As IItemModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.Create(True, itemModel.Name, GroundItemMenu.Launch(c, m, p, itemModel))
               End Function
    End Function

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function()
                   If model.Location.Ground.HasItems Then
                       Return New GroundMenu(context, model, previous)
                   End If
                   Return InPlay.Launch(context, model, previous).Invoke()
               End Function
    End Function

    Private Function ChooseNeverMind(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Never Mind", InPlay.Launch(context, model, previous))
    End Function
End Class
