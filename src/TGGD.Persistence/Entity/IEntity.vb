Public Interface IEntity
    Sub Clear()
    Sub SetMetadata(metadataId As String, metadataValue As String)
    Sub DefaultMetadata(metadataId As String, defaultValue As String)
    Function GetMetadata(metadataId As String) As String
    Function TryGetMetadata(metadataId As String) As String
    Function HasMetadata(metadataId As String) As Boolean
    Sub SetCounter(counterId As String, counterValue As Integer)
    Sub DefaultCounter(counterId As String, defaultValue As Integer)
    Function GetCounter(counterId As String) As Integer
    Function TryGetCounter(counterId As String) As Integer?
    Function HasCounter(counterId As String) As Boolean
    Function ChangeCounter(counterId As String, delta As Integer) As Integer
    Sub SetCounterMinimum(counterId As String, counterMinimum As Integer)
    Function GetCounterMinimum(counterId As String) As Integer
    Sub SetCounterMaximum(counterId As String, counterMaximum As Integer)
    Function GetCounterMaximum(counterId As String) As Integer
    Function HasTag(tagId As String) As Boolean
    Function HasTags(ParamArray tagIds As String()) As Boolean
    Sub SetTag(tagId As String)
    Sub SetTags(ParamArray tagIds As String())
    Sub ClearTag(tagId As String)
    Sub ClearTags(ParamArray tagIds As String())
    Function ToggleTag(tagId As String) As Boolean
    Sub ToggleTags(ParamArray tagIds As String())
    Sub AssignTag(tagId As String, value As Boolean)
    Sub SetDimension(dimensionId As String, dimensionValue As Double)
    Sub DefaultDimension(dimensionId As String, defaultValue As Double)
    Function GetDimension(dimensionId As String) As Double
    Function TryGetDimension(dimensionId As String) As Double?
    Function ChangeDimension(dimensionId As String, delta As Double) As Double
    Sub SetDimensionMinimum(dimensionId As String, dimensionMinimum As Double)
    Function GetDimensionMinimum(dimensionId As String) As Double
    Sub SetDimensionMaximum(dimensionId As String, dimensionMaximum As Double)
    Function GetDimensionMaximum(dimensionId As String) As Double
    Function HasDimension(dimensionId As String) As Boolean
    Function IsCounterMinimum(counterId As String) As Boolean
    Function IsCounterMaximum(counterId As String) As Boolean
    Function IsDimensionMinimum(dimensionId As String) As Boolean
    Function IsDimensionMaximum(dimensionId As String) As Boolean
    Sub SetYoke(yokeId As String, identifier As Guid)
    Function GetYoke(yokeId As String) As Guid?
    Sub ClearYoke(yokeId As String)
    Sub AddToYokage(yokageId As String, identifier As Guid)
    Sub RemoveFromYokage(yokageId As String, identifier As Guid)
    Function GetYokage(yokageId As String) As IEnumerable(Of Guid)
    Function MinimizeCounter(counterId As String) As Integer
    Function MaximizeCounter(counterId As String) As Integer
    Function MinimizeDimension(dimensionId As String) As Double
    Function MaximizeDimension(dimensionId As String) As Double
End Interface
