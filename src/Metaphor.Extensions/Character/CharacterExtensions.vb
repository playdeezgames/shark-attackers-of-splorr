Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module CharacterExtensions
#Region "Show Status"
    <Extension>
    Public Sub ShowStatus(character As ICharacter)
        character.AddMessage($"Status:")
        character.AddMessage($"Cash: {character.GetCash()}")
        character.AddMessage($"Handies: {character.GetHandyCount()}")
        character.AddMessage($"Stamina: {character.GetCounterStatistic(Counters.STAMINA)}")
        character.AddMessage($"Health: {character.GetCounterStatistic(Counters.HEALTH)}")
        character.AddMessage($"Satiety: {character.GetCounterStatistic(Counters.SATIETY)}")
        character.AddMessage($"Stomach: {character.GetCounterStatistic(Counters.STOMACH)}")
        character.AddMessage($"Filth: {character.GetCounter(Counters.FILTH)}")
    End Sub
#End Region
#Region "Look"
    <Extension>
    Public Sub Look(character As ICharacter)
        If character.IsDead Then
            character.AddMessage($"{character.Name} is dead.")
            Return
        End If
        Dim location = character.Location
        character.AddMessage($"{character.Name} is in {location.Name}.")
        DescribeFeatures(location)
    End Sub

    Private Sub DescribeFeatures(location As ILocation)
        location.AddMessage($"Features:")
        For Each feature In location.Features
            location.AddMessage($"- {feature.Name}")
        Next
    End Sub
#End Region
End Module
