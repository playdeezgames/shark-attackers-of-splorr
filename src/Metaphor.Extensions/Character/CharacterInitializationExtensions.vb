Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Friend Module CharacterInitializationExtensions
#Region "N00b"
    <Extension>
    Friend Sub InitializeN00b(character As ICharacter)
        character.World.Avatar = character
    End Sub
#End Region
#Region "Shark"
    Friend Function InitializeShark(distance As Double) As CharacterInitializer
        Return Sub(shark)
                   shark.SetTag(Tags.ENEMY)
               End Sub
    End Function
#End Region
End Module
