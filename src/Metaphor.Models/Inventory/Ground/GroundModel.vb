Imports Metaphor.Persistence

Friend Class GroundModel
    Implements IGroundModel

    Private ReadOnly world As IWorld

    Private Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property HasItems As Boolean Implements IGroundModel.HasItems
        Get
            Return world.Avatar.Location.Inventory.HasItems
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItemModel) Implements IGroundModel.Items
        Get
            Return world.Avatar.Location.Inventory.Items.Select(AddressOf ItemModel.Create)
        End Get
    End Property

    Public ReadOnly Property Inventory As IInventoryModel Implements IGroundModel.Inventory
        Get
            Return InventoryModel.Create(world.Avatar.Location.Inventory)
        End Get
    End Property

    Friend Shared Function Create(entity As IWorld) As IGroundModel
        Return New GroundModel(entity)
    End Function
End Class
