Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module CharacterStatisticExtensions
    <Extension>
    Friend Sub Die(character As ICharacter)
        character.SetTag(Tags.DEAD)
    End Sub
    <Extension>
    Public Function IsDead(character As ICharacter) As Boolean
        Return character.HasTag(Tags.DEAD)
    End Function
    <Extension>
    Public Function IsAvatar(character As ICharacter) As Boolean
        Return character.EntityId = character.World.Avatar.EntityId
    End Function
#Region "Utility"
    Private ReadOnly counterNames As New Dictionary(Of String, String) From
        {
        }
    <Extension>
    Private Function DoChangeCounter(character As ICharacter, counterId As String, delta As Integer, Optional silent As Boolean = False) As Integer
        If delta <> 0 Then
            Dim counterName = counterNames(counterId)
            character.AddMessage($"{character.Name} {If(delta > 0, "gains", "loses")} {Math.Abs(delta)} {counterName}.", silent:=silent)
            character.ChangeCounter(counterId, delta)
            If character.GetCounterMaximum(counterId) = Integer.MaxValue Then
                character.AddMessage($"{character.Name} now has {character.GetCounter(counterId)} {counterName}.", silent:=silent)
            Else
                character.AddMessage($"{character.Name} now has {character.GetCounterStatistic(counterId)} {counterName}.", silent:=silent)
            End If
        End If
        Return character.GetCounter(counterId)
    End Function
#End Region
End Module
