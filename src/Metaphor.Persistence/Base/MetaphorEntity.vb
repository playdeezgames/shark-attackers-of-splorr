Imports Metaphor.Provision
Imports TGGD.Persistence
Imports TGGD.Provision

Friend MustInherit Class MetaphorEntity(Of TData As EntityData)
    Inherits Entity(Of TData)
    Implements IMetaphorEntity

    Protected Sub New(world As IWorld, data As WorldData, entityId As Guid)
        Me.World = world
        Me._data = data
        Me.EntityId = entityId
    End Sub

    Public MustOverride Sub Remove() Implements IMetaphorEntity.Remove
    Public ReadOnly Property World As IWorld Implements IMetaphorEntity.World

    Public ReadOnly Property Name As String Implements IMetaphorEntity.Name
        Get
            Return TryGetMetadata(Metadatas.NAME)
        End Get
    End Property

    Public ReadOnly Property EntityId As Guid Implements IMetaphorEntity.EntityId

    Public ReadOnly Property EntitySubtype As String Implements IMetaphorEntity.EntitySubtype
        Get
            Return TryGetMetadata(Metadatas.ENTITY_SUBTYPE)
        End Get
    End Property

    Public MustOverride ReadOnly Property Exists As Boolean Implements IMetaphorEntity.Exists
    Protected ReadOnly _data As WorldData

    Public ReadOnly Property Verbs As IEnumerable(Of IVerb) Implements IMetaphorEntity.Verbs
        Get
            Return GetYokage(Yokages.VERBS).Select(Function(x) Verb.Create(World, _data, x))
        End Get
    End Property


    Public Function CreateVerb(
                              entitySubtype As String,
                              name As String,
                              Optional initializer As VerbInitializer = Nothing) As IVerb Implements IMetaphorEntity.CreateVerb
        Dim verbId = Guid.NewGuid
        _data.Entities(verbId) = New EntityData With
            {
                .EntityType = EntityTypes.VERB_ENTITY,
                .Metadatas = New Dictionary(Of String, String) From
                {
                    {Metadatas.ENTITY_SUBTYPE, entitySubtype},
                    {Metadatas.NAME, name}
                }
            }
        AddToYokage(Yokages.VERBS, verbId)
        Dim result As IVerb = Verb.Create(World, _data, verbId)
        initializer?.Invoke(result)
        Return result
    End Function

    Public Sub AddMessage(
                         text As String,
                         Optional hints As IDictionary(Of String, String) = Nothing,
                         Optional silent As Boolean = False) Implements IMetaphorEntity.AddMessage
        If Not silent Then
            World.AddMessage(text, hints)
        End If
    End Sub

    Public Sub InitializeCounter(counterId As String, value As Integer, minimum As Integer, maximum As Integer) Implements IMetaphorEntity.InitializeCounter
        SetCounterMaximum(counterId, maximum)
        SetCounterMinimum(counterId, minimum)
        SetCounter(counterId, value)
    End Sub

    Public Function GetCounterPercentage(counterId As String) As String Implements IMetaphorEntity.GetCounterPercentage
        Return $"{100 * GetCounter(counterId) / GetCounterMaximum(counterId)}%"
    End Function

    Public Sub InitializeDimension(dimensionId As String, value As Double, minimum As Double, maximum As Double) Implements IMetaphorEntity.InitializeDimension
        SetDimensionMaximum(dimensionId, maximum)
        SetDimensionMinimum(dimensionId, minimum)
        SetDimension(dimensionId, value)
    End Sub

    Public Function GetCounterStatistic(counterId As String) As String Implements IMetaphorEntity.GetCounterStatistic
        Return $"{GetCounter(counterId)}/{GetCounterMaximum(counterId)}"
    End Function

    Public Function GetDimensionStatistic(dimensionId As String) As String Implements IMetaphorEntity.GetDimensionStatistic
        Return $"{GetDimension(dimensionId):f2}/{GetDimensionMaximum(dimensionId):f2}"
    End Function

    Public Function GetCounterCapacity(counterId As String) As Integer Implements IMetaphorEntity.GetCounterCapacity
        Return GetCounterMaximum(counterId) - GetCounter(counterId)
    End Function

    Public Function GetDimensionCapacity(dimensionId As String) As Double Implements IMetaphorEntity.GetDimensionCapacity
        Return GetDimensionMaximum(dimensionId) - GetDimension(dimensionId)
    End Function

    Public ReadOnly Property Inventory As IInventory Implements IMetaphorEntity.Inventory
        Get
            Dim inventoryId As Guid
            If Not Data.Yokes.TryGetValue(Yokes.INVENTORY, inventoryId) Then
                inventoryId = Guid.NewGuid
                _data.Entities(inventoryId) = New EntityData
                Data.Yokes(Yokes.INVENTORY) = inventoryId
            End If
            Return Persistence.Inventory.Create(World, _data, inventoryId)
        End Get
    End Property
End Class
