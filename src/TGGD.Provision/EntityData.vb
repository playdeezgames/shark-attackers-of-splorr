Public Class EntityData
    Public Property EntityType As String
    Public Property Metadatas As New Dictionary(Of String, String)(StringComparer.InvariantCultureIgnoreCase)
    Public Property Counters As New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase)
    Public Property CounterMinimums As New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase)
    Public Property CounterMaximums As New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase)
    Public Property Dimensions As New Dictionary(Of String, Double)(StringComparer.InvariantCultureIgnoreCase)
    Public Property DimensionMinimums As New Dictionary(Of String, Double)(StringComparer.InvariantCultureIgnoreCase)
    Public Property DimensionMaximums As New Dictionary(Of String, Double)(StringComparer.InvariantCultureIgnoreCase)
    Public Property Tags As New HashSet(Of String)(StringComparer.InvariantCultureIgnoreCase)
    Public Property Yokes As New Dictionary(Of String, Guid)
    Public Property Yokages As New Dictionary(Of String, HashSet(Of Guid))
End Class
