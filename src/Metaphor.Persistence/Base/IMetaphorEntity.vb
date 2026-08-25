Imports TGGD.Persistence
Public Delegate Sub EntityInitializer(entity As IMetaphorEntity)
Public Interface IMetaphorEntity
    Inherits IEntity
    ReadOnly Property World As IWorld
    Sub Remove()
    ReadOnly Property Name As String
    ReadOnly Property EntityId As Guid
    ReadOnly Property EntitySubtype As String
    ReadOnly Property Exists As Boolean
    Function CreateVerb(verbSubtype As String, name As String, Optional initializer As VerbInitializer = Nothing) As IVerb
    ReadOnly Property Verbs As IEnumerable(Of IVerb)
    ReadOnly Property Inventory As IInventory
    Sub AddMessage(text As String, Optional hints As IDictionary(Of String, String) = Nothing, Optional silent As Boolean = False)
    Sub InitializeCounter(counterId As String, value As Integer, minimum As Integer, maximum As Integer)
    Function GetCounterPercentage(counterId As String) As String
    Sub InitializeDimension(
                            dimensionId As String,
                            value As Double,
                            minimum As Double,
                            maximum As Double)
    Function GetCounterStatistic(counterId As String) As String
    Function GetDimensionStatistic(dimensionId As String) As String
    Function GetCounterCapacity(counterId As String) As Integer
    Function GetDimensionCapacity(dimensionId As String) As Double
End Interface
