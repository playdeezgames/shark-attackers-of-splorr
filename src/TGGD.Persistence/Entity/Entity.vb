Imports TGGD.Provision

Public MustInherit Class Entity(Of TData As EntityData)
    Implements IEntity

    Protected MustOverride ReadOnly Property Data As TData

    Public Overridable Sub Clear() Implements IEntity.Clear
        Data.CounterMaximums.Clear()
        Data.CounterMinimums.Clear()
        Data.Counters.Clear()
        Data.DimensionMaximums.Clear()
        Data.DimensionMinimums.Clear()
        Data.Dimensions.Clear()
        Data.Metadatas.Clear()
        Data.Tags.Clear()
        Data.Yokes.Clear()
        Data.Yokages.Clear()
    End Sub

    Public Sub SetMetadata(metadataId As String, metadataValue As String) Implements IEntity.SetMetadata
        Data.Metadatas(metadataId) = metadataValue
    End Sub

    Public Sub DefaultMetadata(metadataId As String, defaultValue As String) Implements IEntity.DefaultMetadata
        If Not Data.Metadatas.ContainsKey(metadataId) Then
            SetMetadata(metadataId, defaultValue)
        End If
    End Sub

    Public Sub SetCounter(counterId As String, counterValue As Integer) Implements IEntity.SetCounter
        Data.Counters(counterId) = Math.Clamp(counterValue, GetCounterMinimum(counterId), GetCounterMaximum(counterId))
    End Sub

    Public Sub DefaultCounter(counterId As String, defaultValue As Integer) Implements IEntity.DefaultCounter
        SetCounter(counterId, If(TryGetCounter(counterId), defaultValue))
    End Sub

    Public Sub SetCounterMinimum(counterId As String, counterMinimum As Integer) Implements IEntity.SetCounterMinimum
        Data.CounterMinimums(counterId) = counterMinimum
    End Sub

    Public Sub SetCounterMaximum(counterId As String, counterMaximum As Integer) Implements IEntity.SetCounterMaximum
        Data.CounterMaximums(counterId) = counterMaximum
    End Sub

    Public Sub SetTag(tagId As String) Implements IEntity.SetTag
        Data.Tags.Add(tagId)
    End Sub

    Public Sub SetTags(ParamArray tagIds() As String) Implements IEntity.SetTags
        For Each tagId In tagIds
            SetTag(tagId)
        Next
    End Sub

    Public Sub ClearTag(tagId As String) Implements IEntity.ClearTag
        Data.Tags.Remove(tagId)
    End Sub

    Public Sub ClearTags(ParamArray tagIds() As String) Implements IEntity.ClearTags
        For Each tagId In tagIds
            ClearTag(tagId)
        Next
    End Sub

    Public Function ToggleTag(tagId As String) As Boolean Implements IEntity.ToggleTag
        If HasTag(tagId) Then
            ClearTag(tagId)
        Else
            SetTag(tagId)
        End If
        Return HasTag(tagId)
    End Function

    Public Sub ToggleTags(ParamArray tagIds() As String) Implements IEntity.ToggleTags
        For Each tagId In tagIds
            ToggleTag(tagId)
        Next
    End Sub

    Public Sub SetDimension(dimensionId As String, dimensionValue As Double) Implements IEntity.SetDimension
        Data.Dimensions(dimensionId) = dimensionValue
    End Sub

    Public Sub DefaultDimension(dimensionId As String, defaultValue As Double) Implements IEntity.DefaultDimension
        SetDimension(dimensionId, If(TryGetDimension(dimensionId), defaultValue))
    End Sub

    Public Sub SetDimensionMinimum(dimensionId As String, dimensionMinimum As Double) Implements IEntity.SetDimensionMinimum
        Data.DimensionMinimums(dimensionId) = dimensionMinimum
    End Sub

    Public Sub SetDimensionMaximum(dimensionId As String, dimensionMaximum As Double) Implements IEntity.SetDimensionMaximum
        Data.DimensionMaximums(dimensionId) = dimensionMaximum
    End Sub

    Public Sub AssignTag(tagId As String, value As Boolean) Implements IEntity.AssignTag
        If value Then
            SetTag(tagId)
        Else
            ClearTag(tagId)
        End If
    End Sub

    Public Function GetMetadata(metadataId As String) As String Implements IEntity.GetMetadata
        Return Data.Metadatas(metadataId)
    End Function

    Public Function TryGetMetadata(metadataId As String) As String Implements IEntity.TryGetMetadata
        Dim result As String = Nothing
        If Data.Metadatas.TryGetValue(metadataId, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function GetCounter(counterId As String) As Integer Implements IEntity.GetCounter
        Return Math.Clamp(Data.Counters(counterId), GetCounterMinimum(counterId), GetCounterMaximum(counterId))
    End Function

    Public Function TryGetCounter(counterId As String) As Integer? Implements IEntity.TryGetCounter
        Dim result As Integer = 0
        If Data.Counters.TryGetValue(counterId, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function ChangeCounter(counterId As String, delta As Integer) As Integer Implements IEntity.ChangeCounter
        SetCounter(counterId, GetCounter(counterId) + delta)
        Return GetCounter(counterId)
    End Function

    Public Function GetCounterMinimum(counterId As String) As Integer Implements IEntity.GetCounterMinimum
        Dim result As Integer = 0
        If Data.CounterMinimums.TryGetValue(counterId, result) Then
            Return result
        End If
        Return Integer.MinValue
    End Function

    Public Function GetCounterMaximum(counterId As String) As Integer Implements IEntity.GetCounterMaximum
        Dim result As Integer = 0
        If Data.CounterMaximums.TryGetValue(counterId, result) Then
            Return result
        End If
        Return Integer.MaxValue
    End Function

    Public Function HasTag(tagId As String) As Boolean Implements IEntity.HasTag
        Return Data.Tags.Contains(tagId)
    End Function

    Public Function HasTags(ParamArray tagIds() As String) As Boolean Implements IEntity.HasTags
        Return tagIds.All(Function(x) HasTag(x))
    End Function

    Public Function GetDimension(dimensionId As String) As Double Implements IEntity.GetDimension
        Return Math.Clamp(
            Data.Dimensions(dimensionId),
            GetDimensionMinimum(dimensionId),
            GetDimensionMaximum(dimensionId))
    End Function

    Public Function TryGetDimension(dimensionId As String) As Double? Implements IEntity.TryGetDimension
        Dim result As Double = 0.0
        If Data.Dimensions.TryGetValue(dimensionId, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function ChangeDimension(dimensionId As String, delta As Double) As Double Implements IEntity.ChangeDimension
        SetDimension(dimensionId, GetDimension(dimensionId) + delta)
        Return GetDimension(dimensionId)
    End Function

    Public Function GetDimensionMinimum(dimensionId As String) As Double Implements IEntity.GetDimensionMinimum
        Dim result As Double = 0.0
        If Data.DimensionMinimums.TryGetValue(dimensionId, result) Then
            Return result
        End If
        Return Double.MinValue
    End Function

    Public Function GetDimensionMaximum(dimensionId As String) As Double Implements IEntity.GetDimensionMaximum
        Dim result As Double = 0.0
        If Data.DimensionMaximums.TryGetValue(dimensionId, result) Then
            Return result
        End If
        Return Double.MaxValue
    End Function

    Public Function IsCounterMinimum(counterId As String) As Boolean Implements IEntity.IsCounterMinimum
        Return GetCounter(counterId) = GetCounterMinimum(counterId)
    End Function

    Public Function IsCounterMaximum(counterId As String) As Boolean Implements IEntity.IsCounterMaximum
        Return GetCounter(counterId) = GetCounterMaximum(counterId)
    End Function

    Public Function HasMetadata(metadataId As String) As Boolean Implements IEntity.HasMetadata
        Return Data.Metadatas.ContainsKey(metadataId)
    End Function

    Public Function HasCounter(counterId As String) As Boolean Implements IEntity.HasCounter
        Return Data.Counters.ContainsKey(counterId)
    End Function

    Public Function HasDimension(dimensionId As String) As Boolean Implements IEntity.HasDimension
        Return Data.Dimensions.ContainsKey(dimensionId)
    End Function

    Public Function IsDimensionMinimum(dimensionId As String) As Boolean Implements IEntity.IsDimensionMinimum
        Return GetDimension(dimensionId) = GetDimensionMinimum(dimensionId)
    End Function

    Public Function IsDimensionMaximum(dimensionId As String) As Boolean Implements IEntity.IsDimensionMaximum
        Return GetDimension(dimensionId) = GetDimensionMaximum(dimensionId)
    End Function

    Public Sub SetYoke(yokeId As String, identifier As Guid) Implements IEntity.SetYoke
        Data.Yokes(yokeId) = identifier
    End Sub

    Public Function GetYoke(yokeId As String) As Guid? Implements IEntity.GetYoke
        Dim result As Guid
        If Data.Yokes.TryGetValue(yokeId, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Sub ClearYoke(yokeId As String) Implements IEntity.ClearYoke
        Data.Yokes.Remove(yokeId)
    End Sub

    Private ReadOnly Property Yokages(yokageId As String) As HashSet(Of Guid)
        Get
            Dim value As HashSet(Of Guid) = Nothing
            If Not Data.Yokages.TryGetValue(yokageId, value) Then
                value = New HashSet(Of Guid)
                Data.Yokages.Add(yokageId, value)
            End If
            Return value
        End Get
    End Property

    Public Sub AddToYokage(yokageId As String, identifier As Guid) Implements IEntity.AddToYokage
        Yokages(yokageId).Add(identifier)
    End Sub

    Public Sub RemoveFromYokage(yokageId As String, identifier As Guid) Implements IEntity.RemoveFromYokage
        Yokages(yokageId).Remove(identifier)
    End Sub

    Public Function GetYokage(yokageId As String) As IEnumerable(Of Guid) Implements IEntity.GetYokage
        Return Yokages(yokageId)
    End Function

    Public Function MinimizeCounter(counterId As String) As Integer Implements IEntity.MinimizeCounter
        SetCounter(counterId, GetCounterMinimum(counterId))
        Return GetCounter(counterId)
    End Function

    Public Function MaximizeCounter(counterId As String) As Integer Implements IEntity.MaximizeCounter
        SetCounter(counterId, GetCounterMaximum(counterId))
        Return GetCounter(counterId)
    End Function

    Public Function MinimizeDimension(dimensionId As String) As Double Implements IEntity.MinimizeDimension
        SetDimension(dimensionId, GetDimensionMinimum(dimensionId))
        Return GetDimension(dimensionId)
    End Function

    Public Function MaximizeDimension(dimensionId As String) As Double Implements IEntity.MaximizeDimension
        SetDimension(dimensionId, GetDimensionMaximum(dimensionId))
        Return GetDimension(dimensionId)
    End Function
End Class
