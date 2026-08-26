Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module LocationVerbExtensions
    Private Delegate Function CanPerformHandler(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
    Private Delegate Sub PerformHandler(verb As IVerb, location As ILocation, actor As ICharacter)

    Private ReadOnly canPerformTable As New Dictionary(Of String, CanPerformHandler) From
        {
            {VerbSubtypes.MOOR, AddressOf CanMoor},
            {VerbSubtypes.UNMOOR, AddressOf CanUnmoor},
            {VerbSubtypes.SET_HEADING, AddressOf CanSetHeading},
            {VerbSubtypes.SET_SPEED, AddressOf CanSetSpeed},
            {VerbSubtypes.MOVE, AddressOf CanMove}
        }

    Private Function CanMove(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
        Return Not location.IsMoored()
    End Function

    Private Function CanSetSpeed(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
        Return Not location.IsMoored()
    End Function

    Private Function CanSetHeading(verb As IVerb, location As ILocation, actor As ICharacter) As Boolean
        Return Not location.IsMoored()
    End Function

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
            {VerbSubtypes.UNMOOR, AddressOf HandleUnmoor},
            {VerbSubtypes.MOOR, AddressOf HandleMoor},
            {VerbSubtypes.SET_HEADING, AddressOf HandleSetHeading},
            {VerbSubtypes.SET_SPEED, AddressOf HandleSetSpeed},
            {VerbSubtypes.MOVE, AddressOf HandleMove}
        }

    Private Sub HandleMove(verb As IVerb, location As ILocation, actor As ICharacter)
        actor.AddMessage($"{location.Name} moves.")
        location.Move()
        location.SpawnShark()
        actor.Look()
    End Sub

    Private Sub HandleSetSpeed(verb As IVerb, location As ILocation, actor As ICharacter)
        actor.DialogMode = DialogModes.CHANGE_SPEED
    End Sub

    Private Sub HandleSetHeading(verb As IVerb, location As ILocation, actor As ICharacter)
        actor.DialogMode = DialogModes.CHANGE_HEADING
    End Sub

    Private Sub HandleMoor(verb As IVerb, location As ILocation, actor As ICharacter)
        Dim pier = actor.World.GetPier()
        actor.AddMessage($"{actor.Name} moors {location.Name} to {pier.Name}.")
        location.Moor(pier)
        actor.Look()
    End Sub

    Private Sub HandleUnmoor(verb As IVerb, location As ILocation, actor As ICharacter)
        actor.AddMessage($"{actor.Name} unmoors {location.Name}.")
        location.Features.Single(Function(x) x.EntitySubtype = FeatureSubtypes.MOORING).Remove()
        actor.Look()
    End Sub

    <Extension>
    Sub Perform(verb As IVerb, location As ILocation, actor As ICharacter)
        Dim handler As PerformHandler = Nothing
        If performTable.TryGetValue(verb.EntitySubtype, handler) Then
            handler.Invoke(verb, location, actor)
            Return
        End If
    End Sub

End Module
