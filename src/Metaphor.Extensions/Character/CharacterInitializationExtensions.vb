Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Friend Module CharacterInitializationExtensions
#Region "N00b"
    <Extension>
    Friend Sub InitializeN00b(character As ICharacter)
#If DEBUG Then
        character.InitializeCounter(Counters.SATIETY, 100, 0, 100)
        character.InitializeCounter(Counters.HEALTH, 5, 0, 100)
        character.InitializeCounter(Counters.STOMACH, 5, 0, 50)
#Else
        character.InitializeCounter(Counters.SATIETY, 100, 0, 100)
        character.InitializeCounter(Counters.HEALTH, 100, 0, 100)
        character.InitializeCounter(Counters.STOMACH, 0, 0, 50)
#End If
        character.InitializeCounter(Counters.HANDY_COUNT, 0, 0, Integer.MaxValue)
        character.InitializeCounter(Counters.CASH, 0, 0, Integer.MaxValue)
        character.InitializeCounter(Counters.CASH_PER_HANDY, 5, 5, Integer.MaxValue)
        character.InitializeCounter(Counters.STAMINA, 10, 0, 10)
        character.InitializeCounter(Counters.FILTH, 0, 0, Integer.MaxValue)
        character.World.Avatar = character
    End Sub
#End Region
#Region "John"
    Friend Sub InitializeJohn(character As ICharacter)
        character.CreateVerb(VerbSubtypes.GIVE_HANDY, "Give Handy")
    End Sub
#End Region
End Module
