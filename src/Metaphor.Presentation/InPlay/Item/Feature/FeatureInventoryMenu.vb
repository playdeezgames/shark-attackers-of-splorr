
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class FeatureInventoryMenu
    Inherits MetaphorPickerMenu

    Private ReadOnly featureModel As IFeatureModel

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel)
        MyBase.New(context, model, previous)
        Me.featureModel = featureModel
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return $"Items in {FeatureModel.Name}:"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Concat(featureModel.Inventory.ItemStacks.Select(AddressOf ChooseItemStack))
        End Get
    End Property

    Private Function ChooseItemStack(itemStackModel As IItemStackModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.CreateEnabled(
                        itemStackModel.Name,
                        FeatureItemStackMenu.Launch(c, m, p, featureModel, itemStackModel))
               End Function
    End Function

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel) As DialogSource
        Return Function()
                   If featureModel.Inventory.HasItems Then
                       Return New FeatureInventoryMenu(context, model, previous, featureModel)
                   End If
                   Return InPlay.Launch(context, model, previous).Invoke()
               End Function
    End Function

    Private Function ChooseNeverMind(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Never Mind", FeatureMenu.Launch(context, model, previous, featureModel))
    End Function
End Class
