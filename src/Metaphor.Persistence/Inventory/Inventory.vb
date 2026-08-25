Imports Metaphor.Provision
Imports TGGD.Provision

Friend Class Inventory
    Inherits MetaphorEntity(Of EntityData)
    Implements IInventory

    Public Sub New(world As IWorld, data As WorldData, inventoryId As Guid)
        MyBase.New(world, data, inventoryId)
    End Sub

    Public ReadOnly Property HasItems As Boolean Implements IInventory.HasItems
        Get
            Return GetYokage(Yokages.ITEMS).Any()
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of IItem) Implements IInventory.Items
        Get
            Return GetYokage(Yokages.ITEMS).Select(Function(x) Item.Create(World, _data, x))
        End Get
    End Property

    Public ReadOnly Property ItemStacks As IEnumerable(Of IItemStack) Implements IInventory.ItemStacks
        Get
            Return Items.GroupBy(Function(x) x.EntitySubtype).Select(Function(x) ItemStack.Create(Me, x.Key))
        End Get
    End Property

    Public Overrides ReadOnly Property Exists As Boolean
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Protected Overrides ReadOnly Property Data As EntityData
        Get
            Return _data.Entities(EntityId)
        End Get
    End Property

    Public Overrides Sub Remove()
        For Each item In Items
            item.Remove()
        Next
        _data.Entities.Remove(EntityId)
    End Sub

    Friend Shared Function Create(world As IWorld, data As WorldData, inventoryId As Guid?) As IInventory
        Return If(inventoryId.HasValue, New Inventory(world, data, inventoryId.Value), Nothing)
    End Function

    Public Function CreateItem(entitySubtype As String, name As String, Optional initializer As ItemInitializer = Nothing) As IItem Implements IInventory.CreateItem
        Dim itemId = Guid.NewGuid
        _data.Entities(itemId) = New TGGD.Provision.EntityData With
            {
                .EntityType = EntityTypes.ITEM_ENTITY,
                .Metadatas = New Dictionary(Of String, String) From
                {
                    {Metadatas.ENTITY_SUBTYPE, entitySubtype},
                    {Metadatas.NAME, name}
                },
                .Yokes = New Dictionary(Of String, Guid) From
                {
                    {Yokes.CONTAINER, EntityId}
                }
            }
        AddToYokage(Yokages.ITEMS, itemId)
        Dim result As IItem = Item.Create(World, _data, itemId)
        initializer?.Invoke(result)
        Return result
    End Function

    Public Function HasItemOfSubtype(entitySubtype As String) As Boolean Implements IInventory.HasItemOfSubtype
        Return Items.Any(Function(x) x.EntitySubtype = entitySubtype)
    End Function

    Public Function GetItemsOfSubtype(entitySubtype As String) As IEnumerable(Of IItem) Implements IInventory.GetItemsOfSubtype
        Return Items.Where(Function(x) x.EntitySubtype = entitySubtype)
    End Function
End Class
