Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Friend Module CharacterBiologyExtensions
    <Extension>
    Friend Sub DoBiology(character As ICharacter, amount As Integer)
        If Not character.IsDead() Then
            Dim stomach = Math.Min(amount, character.GetCounter(Counters.STOMACH))
            character.ChangeStomach(-stomach)
            amount -= stomach

            If amount > 0 Then
                Dim satiety = Math.Min(amount, character.GetCounter(Counters.SATIETY))
                character.ChangeSatiety(-satiety)
                amount -= satiety
                If amount > 0 Then
                    Dim health = Math.Min(amount, character.GetCounter(Counters.HEALTH))
                    character.ChangeHealth(-health)
                    If character.IsDead Then
                        character.AddMessage($"{character.Name} dies.")
                    End If
                End If
            Else
                If character.IsCounterMaximum(Counters.SATIETY) Then
                    character.ChangeHealth(1)
                Else
                    character.ChangeSatiety(1)
                End If
            End If

        End If
    End Sub
End Module
