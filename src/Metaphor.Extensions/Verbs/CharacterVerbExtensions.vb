Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module CharacterVerbExtensions
    Private Delegate Function CanPerformHandler(verb As IVerb, character As ICharacter, actor As ICharacter) As Boolean
    Private Delegate Sub PerformHandler(verb As IVerb, character As ICharacter, actor As ICharacter)

    Private ReadOnly canPerformTable As New Dictionary(Of String, CanPerformHandler) From
        {
            {VerbSubtypes.GIVE_HANDY, AddressOf CanGiveHandy}
        }

    Private Function CanGiveHandy(verb As IVerb, character As ICharacter, actor As ICharacter) As Boolean
        Return Not actor.IsCounterMinimum(Counters.STAMINA)
    End Function

    <Extension>
    Public Function CanPerform(verb As IVerb, character As ICharacter, actor As ICharacter) As Boolean
        Dim handler As CanPerformHandler = Nothing
        If canPerformTable.TryGetValue(verb.EntitySubtype, handler) Then
            Return handler.Invoke(verb, character, actor)
        End If
        Return True
    End Function

    Private ReadOnly performTable As New Dictionary(Of String, PerformHandler) From
        {
            {VerbSubtypes.GIVE_HANDY, AddressOf HandleGiveHandy}
        }

    Private Sub HandleGiveHandy(verb As IVerb, character As ICharacter, actor As ICharacter)
        actor.ChangeStamina(-1)
        actor.ChangeFilth(1)
        actor.IncrementHandyCount(character)
        actor.ChangeCash(actor.GetCashPerHandy())
        character.Remove()
        actor.DoBiology(1)
    End Sub

    <Extension>
    Sub Perform(verb As IVerb, character As ICharacter, actor As ICharacter)
        Dim handler As PerformHandler = Nothing
        If performTable.TryGetValue(verb.EntitySubtype, handler) Then
            handler.Invoke(verb, character, actor)
            Return
        End If
    End Sub
End Module
