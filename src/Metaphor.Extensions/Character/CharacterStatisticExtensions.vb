Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module CharacterStatisticExtensions
    <Extension>
    Public Function IsDead(character As ICharacter) As Boolean
        Return character.IsCounterMinimum(Counters.HEALTH)
    End Function
#Region "Stomach"
    <Extension>
    Friend Function ChangeStomach(character As ICharacter, stomach As Integer) As Integer
        Return ChangeStatistic(character, Counters.STOMACH, stomach)
    End Function
#End Region
#Region "Satiety"
    <Extension>
    Friend Function ChangeSatiety(character As ICharacter, satiety As Integer) As Integer
        Return ChangeStatistic(character, Counters.SATIETY, satiety)
    End Function
#End Region
#Region "Health"
    <Extension>
    Friend Function ChangeHealth(character As ICharacter, health As Integer) As Integer
        Return ChangeStatistic(character, Counters.HEALTH, health)
    End Function
#End Region
#Region "Cash"
    <Extension>
    Friend Function GetCash(character As ICharacter) As Integer
        Return character.GetCounter(Counters.CASH)
    End Function
    <Extension>
    Friend Function ChangeCash(character As ICharacter, cash As Integer, Optional silent As Boolean = False) As Integer
        Return character.ChangeStatistic(Counters.CASH, cash, silent)
    End Function
#End Region
#Region "Cash Per Handy"
    <Extension>
    Friend Function GetCashPerHandy(character As ICharacter) As Integer
        Return character.GetCounter(Counters.CASH_PER_HANDY)
    End Function
#End Region
#Region "Handy Count"
    <Extension>
    Friend Sub IncrementHandyCount(character As ICharacter, target As ICharacter, Optional silent As Boolean = False)
        character.AddMessage($"{character.Name} gives {target.Name} a handy.", silent:=silent)
        character.ChangeCounter(Counters.HANDY_COUNT, 1)
        character.AddMessage($"{character.Name} has given {character.GetHandyCount()} handies.", silent:=silent)
    End Sub
    <Extension>
    Friend Function GetHandyCount(character As ICharacter) As Integer
        Return character.GetCounter(Counters.HANDY_COUNT)
    End Function
#End Region
#Region "Filth"
    <Extension>
    Friend Function ChangeFilth(character As ICharacter, filth As Integer, Optional silent As Boolean = False) As Integer
        Return character.ChangeStatistic(Counters.FILTH, filth, silent)
    End Function
#End Region
#Region "Stamina"
    <Extension>
    Friend Function ChangeStamina(character As ICharacter, stamina As Integer, Optional silent As Boolean = False) As Integer
        Return character.ChangeStatistic(Counters.STAMINA, stamina, silent)
    End Function
#End Region
#Region "Utility"
    Private ReadOnly counterNames As New Dictionary(Of String, String) From
        {
            {Counters.CASH, "cash"},
            {Counters.STAMINA, "stamina"},
            {Counters.HANDY_COUNT, "handies"},
            {Counters.FILTH, "filth"},
            {Counters.STOMACH, "stomach"},
            {Counters.SATIETY, "satiety"},
            {Counters.HEALTH, "health"}
        }
    <Extension>
    Private Function ChangeStatistic(character As ICharacter, counterId As String, delta As Integer, Optional silent As Boolean = False) As Integer
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
