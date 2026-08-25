Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module CharacterExtensions
#Region "Show Status"
    <Extension>
    Public Sub ShowStatus(character As ICharacter)
        character.AddMessage($"Status:")
    End Sub
#End Region
#Region "Look"
    <Extension>
    Public Sub Look(character As ICharacter)
        Dim location = character.Location
        character.AddMessage($"{character.Name} is at {location.Name}.")
        location.Describe()
        DescribeFeatures(location)
    End Sub

    Private Sub DescribeFeatures(location As ILocation)
        If location.HasFeatures Then
            location.AddMessage($"Features:")
            For Each feature In location.Features
                location.AddMessage($"- {feature.Name}")
            Next
        End If
    End Sub
#End Region
End Module
