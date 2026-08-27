Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module CharacterCombatExtensions
    <Extension>
    Friend Function GetEnemies(character As ICharacter) As IEnumerable(Of ICharacter)
        Return character.Location.Characters.Where(Function(x) x.HasTag(Tags.ENEMY))
    End Function
    <Extension>
    Public Function IsInCombat(character As ICharacter) As Boolean
        Return character.IsAvatar() AndAlso character.GetEnemies().Any()
    End Function
    <Extension>
    Public Sub Fight(character As ICharacter)
        Dim enemy = character.GetEnemies().First()
        character.AddMessage($"{character.Name} attacks {enemy.Name}.")
        character.AddMessage($"{character.Name} does not damage {enemy.Name} in any way.")
        character.AddMessage($"{enemy.Name} attacks {character.Name}.")
        character.AddMessage($"{enemy.Name} kills {character.Name}.")
        character.Die()
    End Sub
End Module
