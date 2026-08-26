Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence
Imports TGGD.Extensions

Public Module LocationExtensions
#Region "Description"
    Private Delegate Sub LocationDescriber(location As ILocation)
    Private ReadOnly describers As New Dictionary(Of String, LocationDescriber) From
        {
            {LocationSubtypes.BOAT, AddressOf DescribeBoat}
        }
    Private Sub DescribeBoat(boat As ILocation)
        boat.AddMessage($"Location: ({boat.GetX():f2},{boat.GetY():f2})")
        If boat.IsMoored() Then
            boat.AddMessage($"Moored to pier")
        Else
            Dim pier = boat.World.GetPier()
            boat.AddMessage($"Current heading: {boat.GetHeading():f2}°")
            boat.AddMessage($"Current speed: {boat.GetSpeed():f2}")
            boat.AddMessage($"Distance to pier: {boat.DistanceTo(pier):f2}")
            boat.AddMessage($"Heading to pier: {boat.HeadingTo(pier):f2}°")
        End If
    End Sub
    <Extension>
    Friend Sub Describe(location As ILocation)
        Dim describer As LocationDescriber = Nothing
        If describers.TryGetValue(location.EntitySubtype, describer) Then
            describer(location)
        End If
    End Sub
#End Region
#Region "N00b"
    <Extension>
    Friend Function CreateN00b(location As ILocation, name As String) As ICharacter
        Return location.CreateCharacter(CharacterSubtypes.N00B, name, AddressOf CharacterInitializationExtensions.InitializeN00b)
    End Function
#End Region
#Region "Moorings"
    <Extension>
    Friend Function IsMoored(location As ILocation) As Boolean
        Return location.Features.Any(Function(x) x.EntitySubtype = FeatureSubtypes.MOORING)
    End Function
    <Extension>
    Private Function CreateMooring(fromLocation As ILocation, toLocation As ILocation) As IFeature
        Return fromLocation.CreateFeature(
            FeatureSubtypes.MOORING,
            $"Mooring from {fromLocation.Name} to {toLocation.Name}",
            FeatureInitializationExtensions.InitializeMooring(toLocation))
    End Function
    <Extension>
    Friend Sub Moor(fromLocation As ILocation, toLocation As ILocation)
        Dim toMooring = fromLocation.CreateMooring(toLocation)
        Dim fromMooring = toLocation.CreateMooring(fromLocation)
        toMooring.Twin = fromMooring
        fromMooring.Twin = toMooring
    End Sub
#End Region
#Region "X and Y"
    <Extension>
    Friend Sub Move(location As ILocation)
        Dim nextXY = Utility.GetNextXY(location.GetXY(), location.GetHeading(), location.GetSpeed())
        location.SetXY(nextXY.X, nextXY.Y)
    End Sub
    <Extension>
    Friend Sub SetXY(location As ILocation, x As Double, y As Double)
        location.SetDimension(Dimensions.X, x)
        location.SetDimension(Dimensions.Y, y)
    End Sub
    <Extension>
    Friend Function GetX(location As ILocation) As Double
        Return location.GetDimension(Dimensions.X)
    End Function
    <Extension>
    Friend Function GetY(location As ILocation) As Double
        Return location.GetDimension(Dimensions.Y)
    End Function
    <Extension>
    Friend Function GetXY(location As ILocation) As (X As Double, Y As Double)
        Return (location.GetX(), location.GetY())
    End Function
    <Extension>
    Friend Function DistanceTo(fromLocation As ILocation, toLocation As ILocation) As Double
        Return Utility.Distance(fromLocation.GetXY(), toLocation.GetXY())
    End Function
    <Extension>
    Friend Function HeadingTo(fromLocation As ILocation, toLocation As ILocation) As Double
        Return Utility.HeadingTo(fromLocation.GetXY(), toLocation.GetXY())
    End Function
#End Region
#Region "Heading"
    <Extension>
    Public Sub SetHeading(location As ILocation, heading As Double)
        location.SetDimension(Dimensions.HEADING, heading)
    End Sub
    <Extension>
    Public Function GetHeading(location As ILocation) As Double
        Return location.GetDimension(Dimensions.HEADING)
    End Function
#End Region
#Region "Speed"
    <Extension>
    Public Sub SetSpeed(location As ILocation, speed As Double)
        location.SetDimension(Dimensions.SPEED, speed)
    End Sub
    <Extension>
    Public Function GetSpeed(location As ILocation) As Double
        Return location.GetDimension(Dimensions.SPEED)
    End Function
#End Region
#Region "Sharks"
    <Extension>
    Friend Function CreateShark(location As ILocation, distance As Double) As ICharacter
        Return location.CreateCharacter(CharacterSubtypes.SHARK, "Shark", CharacterInitializationExtensions.InitializeShark(distance))
    End Function
    <Extension>
    Friend Function SpawnShark(boat As ILocation) As Boolean
        Dim distance = boat.DistanceTo(boat.World.GetPier())
        If distance < Grimoire.MINIMUM_SHARK_DISTANCE Then
            Return False
        End If
        Dim generator = RNG.MakeBooleanGenerator(CInt(Grimoire.MINIMUM_SHARK_DISTANCE), CInt(distance - Grimoire.MINIMUM_SHARK_DISTANCE))
        Dim spawn = RNG.FromGenerator(generator)
        If spawn Then
            Dim shark = boat.CreateShark(distance)
            boat.AddMessage($"{shark.Name} appears!")
            Return True
        Else
            Return False
        End If
    End Function
#End Region
End Module
