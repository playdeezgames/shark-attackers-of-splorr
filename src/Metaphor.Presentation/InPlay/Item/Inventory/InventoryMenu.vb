
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class InventoryMenu
    Inherits MetaphorPickerMenu

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return "Inventory:"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Concat(Model.Avatar.Inventory.ItemStacks.Select(AddressOf ChooseItemStack))
        End Get
    End Property

    Private Function ChooseItemStack(itemStackModel As IItemStackModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.CreateEnabled(
                        itemStackModel.Name,
                        InventoryItemStackMenu.Launch(c, m, p, itemStackModel))
               End Function
    End Function

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function()
                   If model.Avatar.Inventory.HasItems Then
                       Return New InventoryMenu(context, model, previous)
                   End If
                   Return InPlay.Launch(context, model, previous).Invoke()
               End Function
    End Function

    Private Function ChooseNeverMind(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Never Mind", InPlay.Launch(context, model, previous))
    End Function
End Class
