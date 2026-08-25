Imports Metaphor.Persistence

Friend Class FeatureInventoryModel
    Implements IInventoryModel

    Private ReadOnly feature As IFeature

    Private Sub New(feature As IFeature)
        Me.feature = feature
    End Sub

    Public ReadOnly Property HasItems As Boolean Implements IInventoryModel.HasItems
        Get
            Return feature.Inventory.HasItems
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItemModel) Implements IInventoryModel.Items
        Get
            Return feature.Inventory.Items.Select(AddressOf ItemModel.Create)
        End Get
    End Property

    Public ReadOnly Property ItemStacks As IEnumerable(Of IItemStackModel) Implements IInventoryModel.ItemStacks
        Get
            Return feature.Inventory.ItemStacks.Select(AddressOf ItemStackModel.Create)
        End Get
    End Property

    Friend Shared Function Create(feature As IFeature) As IInventoryModel
        Return New FeatureInventoryModel(feature)
    End Function
End Class
