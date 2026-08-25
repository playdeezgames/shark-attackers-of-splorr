Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module LocationVerbExtensions
    Private Delegate Function CanPerformHandler(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
    Private Delegate Sub PerformHandler(verb As IVerb, location As ILocation, actor As ICharacter)

    Private ReadOnly canPerformTable As New Dictionary(Of String, CanPerformHandler) From
        {
            {VerbSubtypes.MOOR, AddressOf CanMoor},
            {VerbSubtypes.UNMOOR, AddressOf CanUnmoor}
        }

    Private Function CanUnmoor(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
        Return location.IsMoored()
    End Function

    Private Function CanMoor(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
        Return Not location.IsMoored() AndAlso location.DistanceTo(location.World.GetPier()) < 1.0
    End Function

    <Extension>
    Public Function CanPerform(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
        Dim handler As CanPerformHandler = Nothing
        If canPerformTable.TryGetValue(verb.EntitySubtype, handler) Then
            Return handler.Invoke(verb, location, actor)
        End If
        Return True
    End Function

    Private ReadOnly performTable As New Dictionary(Of String, PerformHandler) From
        {
        }
    <Extension>
    Sub Perform(verb As IVerb, location As ILocation, actor As ICharacter)
        Dim handler As PerformHandler = Nothing
        If performTable.TryGetValue(verb.EntitySubtype, handler) Then
            handler.Invoke(verb, location, actor)
            Return
        End If
    End Sub

End Module
